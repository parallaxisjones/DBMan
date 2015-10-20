using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace Playverse.Data
{
    public class DBManager
    {
        public static DBManager Instance = new DBManager();

        private IDatabaseManager pvprodGeneral = new ProductionDatabase(Datastore.General);

#if LOCAL || DEBUG
        private IDatabaseManager Monitoring = new LocalDatabase(Datastore.Monitoring);
        //private IDatabaseManager Monitoring = new ProductionDatabase(Datastore.Monitoring);

#elif STAGING
        private IDatabaseManager Monitoring = new StagingDatabase(Datastore.Monitoring);
#else
        private IDatabaseManager Monitoring = new ProductionDatabase(Datastore.Monitoring);
#endif

        public DBEnvironment GetEnvironment()
        {
            return Monitoring.GetEnvironment();
        }

        public IEnumerable<T> Query<T>(Datastore ds, string sql, object param = null)
        {
            IDbConnection mySqlConnection = GetDbConnection(ds);

            IEnumerable<T> results = mySqlConnection.Query<T>(sql, param);
            
            return results;
        }

        public DataTable Query(Datastore ds, string query, Dictionary<string, string> parameters = null)
        {
            switch (ds)
            {
                case Datastore.General:
                    return pvprodGeneral.Query(query, parameters);
                case Datastore.Monitoring:
                    return Monitoring.Query(query, parameters);
                default:
                    throw new Exception("No Such datastore available");
            }
        }

        public int QueryForCount(Datastore ds, string query, Dictionary<string, string> parameters = null)
        {
            switch (ds)
            {
                case Datastore.General:
                    return pvprodGeneral.Count(query, parameters);
                case Datastore.Monitoring:
                    return Monitoring.Count(query, parameters);
                default:
                    throw new Exception("No Such datastore available");
            }
        }

        public int Insert(Datastore ds, string query, Dictionary<string, string> parameters = null)
        {
            switch (ds)
            {
                case Datastore.General:
                    return pvprodGeneral.Insert(query, parameters);
                case Datastore.Monitoring:
                    return Monitoring.Insert(query, parameters);
                default:
                    throw new Exception("No Such datastore available");
            }
        }

        public int Update(Datastore ds, List<string> queries, Dictionary<string, string> parameters = null)
        {
            switch (ds)
            {
                case Datastore.General:
                    return pvprodGeneral.Update(queries, parameters);
                case Datastore.Monitoring:
                    return Monitoring.Update(queries, parameters);
                default:
                    throw new Exception("No Such datastore available");
            }
        }

        public void Stream(Datastore ds, string query, Action<DataRow> action)
        {
            switch (ds)
            {
                case Datastore.General:
                     pvprodGeneral.Stream(query, action);
                     break;
                case Datastore.Monitoring:
                     Monitoring.Stream(query, action);
                     break;
                default:
                    throw new Exception("No Such datastore available");
            }
        }

        private IDbConnection GetDbConnection(Datastore ds)
        {
            switch (ds)
            {
                case Datastore.General:
                    return pvprodGeneral.GetConnection();
                case Datastore.Monitoring:
                    return Monitoring.GetConnection();
                default:
                    throw new Exception("Unable to get db connection. No such datastore available.");
            }
        }
    }
}