using System.Data;

namespace Playverse.Data
{
    public static class DataRowExtensions
    {
        public static T GetValue<T>(this DataRow row, string columnName) where T : class
        {
            if (row.IsNull(columnName))
                return null;

            return row[columnName] as T;
        }

        public static string GetString(this DataRow row, string columnName)
        {
            if (row.IsNull(columnName))
                return string.Empty;

            return row[columnName] as string ?? string.Empty;
        }
    }
}