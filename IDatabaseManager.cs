using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Playverse.Data
{
    public enum Datastore
    {
        Local,
        General,
        Monitoring,
        Validation
    }

    public enum DBEnvironment
    {
        Local,
        Production,
        Staging

    }

    public interface IDatabaseManager
    {
        MySqlConnection GetConnection();
        DataTable Query(string query, Dictionary<string, string> parameters = null);
        int Insert(string query, Dictionary<string, string> parameters = null);
        int Update(List<string> queries, Dictionary<string, string> parameters = null);
        int Delete(string query, Dictionary<string, string> parameters = null);
        int Count(string query, Dictionary<string, string> parameters = null);
        void Stream(string query, Action<DataRow> action);
        DBEnvironment GetEnvironment();
    }

    public class DBConnection : IDatabaseManager
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string SecInfo { get; set; }
        public string Database { get; set; }
        public string DBConnectionString { get; set; }
        public Datastore DataStore { get; set; }
        protected DBEnvironment Environment { get; set; }
        private int Timeout = 0;

        protected void SetConnectionString()
        {
            this.DBConnectionString = String.Format(@"server={0};User Id={1};password={2};Persist Security Info=True;database={3};Allow User Variables=True;Convert Zero Datetime=True;", this.Server, this.User, this.Password, this.Database);
        }

        public DBEnvironment GetEnvironment()
        {
            return Environment;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(DBConnectionString);
        }

        public DataTable Query(string query, Dictionary<string, string> parameters = null)
        {
            DataTable result = new DataTable();
            try
            {
                using (MySqlConnection conn = GetConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandTimeout = Timeout;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, string> entry in parameters)
                            cmd.Parameters.Add(new MySqlParameter(entry.Key, entry.Value));
                    }

                    conn.Open();
                    result.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("query error: {0}", ex.Message));
            }

            return result;
        }

        public int Count(string query, Dictionary<string, string> parameters = null)
        {
            int result = 0;
            DataTable dt = Query(query, parameters);
            if (HasRows(dt))
            {
                Int32.TryParse(dt.Rows[0][0].ToString(), out result);
            }
            return result;
        }
        private int TryInsert(string query, Dictionary<string, string> parameters = null)
        {

            using (MySqlConnection conn = GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                int result = 0;
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, string> entry in parameters)
                            cmd.Parameters.Add(new MySqlParameter(entry.Key, entry.Value));
                    }

                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (MySqlException x)
                {
                    throw x;
                }
                finally
                {
                    conn.Close();
                }
                return result;
            };

        }
        public int Insert(string query, Dictionary<string, string> parameters = null)
        {
            int result = 0;
            try
            {
                TryInsert(query, parameters);
            }
            catch (MySqlException x)
            {
                int mysqlErrorCode = x.Number;
                /* These are the Mysql error codes http://dev.mysql.com/doc/refman/5.6/en/error-messages-server.html */
                switch (mysqlErrorCode)
                {
                    case 1213:
                        {
                            try
                            {
                                Retry.Do(() => TryInsert(query, parameters), TimeSpan.FromSeconds(10), 5);
                            }
                            catch (AggregateException xxx)
                            {
                                foreach (Exception innerEx in xxx.InnerExceptions)
                                {
                                    Console.WriteLine(String.Format("Message: {0}", innerEx.Message));
                                }
                            }                    
                        }
                        break;
                    default:
                        throw x;
                }

            }
            return result;
        }

        public int Update(List<string> queries, Dictionary<string, string> parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(null, conn))
            {
                int result = 0;
                try
                {

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, string> entry in parameters)
                            cmd.Parameters.Add(new MySqlParameter(entry.Key, entry.Value));
                    }
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;

                    int BatchSize = 1000;
                    foreach (List<string> Batch in queries.Batch<string>(BatchSize))
                    {

                        Retry.Do(() =>
                        {
                            conn.Open();
                            MySqlTransaction t = conn.BeginTransaction(
                               IsolationLevel.ReadUncommitted);
                            cmd.Transaction = t;
                            try
                            {
                                string massiveBlackHole = string.Join("", Batch);

                                cmd.CommandText = massiveBlackHole;
                                cmd.ExecuteNonQuery();
                                t.Commit();
                            }
                            catch (Exception ex)
                            {
                                lock (new object())
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(ex.Message);
                                    Console.ResetColor();
                                }
                                t.Rollback();
                            }
                            finally
                            {
                                conn.Close();
                            }

                        }, TimeSpan.FromSeconds(5), 5);

                    };

                }
                catch (AggregateException xxx)
                {
                    foreach (Exception innerEx in xxx.InnerExceptions)
                    {
                        Console.WriteLine(innerEx.Message);
                    }
                }

                return result;
            };
        }

        public int Delete(string query, Dictionary<string, string> parameters = null)
        {
            return 0;
        }

        public static int lastSecondLogged = 0;
        public void Stream(string query, Action<DataRow> action)
        {
            using (MySqlConnection conn = GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.CommandTimeout = Timeout;
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    DataTable table = new DataTable();

                    var schemaTable = reader.GetSchemaTable();
                    foreach (DataRowView row in schemaTable.DefaultView)
                    {
                        var columnName = (string) row["ColumnName"];
                        var type = (Type) row["DataType"];
                        table.Columns.Add(columnName, type);
                    }

                    while (reader.Read())
                    {
                        double startMemory = GC.GetTotalMemory(false) / 1024D / 1024D;

                        DataRow row = table.NewRow();
                        
                        foreach (DataColumn col in table.Columns)
                        {
                            row[col.ColumnName] = reader[col.ColumnName];
                        }

                        double afterColumns = GC.GetTotalMemory(false) / 1024D / 1024D;

                        Task.Factory.StartNew(() =>
                        {
                            action(row);
                        });

                        double afterAction = GC.GetTotalMemory(false) / 1024D / 1024D;

                        //if (DateTime.UtcNow.Second != lastSecondLogged)
                        {
                            Console.WriteLine("{3} Action: Start: {0}mb, Columns: {1}mb, Action: {2}mb", startMemory, afterColumns, afterAction, DateTime.UtcNow);

                            lastSecondLogged = DateTime.UtcNow.Second;
                        }
                    }

                    reader.Close();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private bool HasRows(DataTable dt)
        {
            bool hasRows = false;
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                hasRows = true;
            }
            return hasRows;
        }
    }
}
