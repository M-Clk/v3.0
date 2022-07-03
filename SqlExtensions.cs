using System.Data.SqlClient;

namespace Otomasyon
{
    public static class SqlExtensions
    {
        public static bool IsAvailable(this SqlConnection connection)
        {
            try
            {
                connection.OpenSafe();
                connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }

        public static void EnsureDbCreated(this SqlConnection connection)
        {
            var command = connection.CreateCommand();
            connection.OpenSafe();
            command.CommandText = "select Count(*) from sys.databases WHERE name = 'OtomasyonDB'";
            var dbCount = (int)command.ExecuteScalar();
            if (dbCount > 0)
            {
                DbOperations.Connection.ConnectionString = connection.ConnectionString.Replace("master", "OtomasyonDb");
                connection.Close();
                return;
            }

            command.CommandText = "Create Database OtomasyonDb";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static bool IsDatabaseExist(this SqlConnection connection)
        {
            var dbCount = 0;
            connection.OpenSafe();
            var command = connection.CreateCommand();
            try
            {
                command.CommandText = "select Count(*) from sys.databases WHERE name = 'OtomasyonDB'";
                dbCount = (int)command.ExecuteScalar();
                command.Dispose();
                connection.Close();
            }
            catch
            {
            }

            return dbCount > 0;
        }

        public static void OpenSafe(this SqlConnection connection)
        {
            connection.Close();
            connection.Open();
        }
    }
}