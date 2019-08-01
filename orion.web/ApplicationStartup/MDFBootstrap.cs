using System.Data.SqlClient;
using System.IO;

namespace orion.web.ApplicationStartup
{
    public class MDFBootstrap
    {
        public static string SetupLocalDbFile(string filePath, string dbName)
        {
            var dbFileName = Path.Combine(filePath, $"{dbName}.mdf");
            var backupFileName = Path.Combine(filePath, $"{dbName}.bak");
            using (var connection = new SqlConnection(@"server=(localdb)\mssqllocaldb;Initial Catalog=master"))
            {
                connection.Open();
                //detatch only when the file is mounted currently from a different location
                DetatchExistingDb(dbName,dbFileName, connection);
                if (!File.Exists(dbFileName))
                {
                    if (File.Exists(backupFileName))
                    {
                        string restore = string.Format("RESTORE DATABASE [" + dbName + @"] FROM DISK='{0}\{1}.bak'", filePath, dbName);
                        var command = new SqlCommand(restore, connection);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        CreateNewEmptyMdf(filePath, dbName, connection);
                    }
                }
            }

            return dbFileName;
        }
        private static void CreateNewEmptyMdf(string filePath, string dbName, SqlConnection connection)
        {
            string sql = string.Format(@"
                    CREATE DATABASE
                        [" + dbName + @"]
                    ON PRIMARY (
                       NAME=Test_data,
                       FILENAME = '{0}\{1}.mdf'
                    )
                    LOG ON (
                        NAME=Test_log,
                        FILENAME = '{0}\{1}.ldf'
                    )",
                filePath,
                dbName
            );

            SqlCommand command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static SqlCommand DetatchExistingDb(string dbName, string dbFileName, SqlConnection connection)
        {
            string detatch = @"
                    if exists(select * from sys.databases where [name] = '" + dbName + @"')
                    begin
	                    if not exists(select * from sys.master_files where physical_name = '" + dbFileName + @"')
	                    begin
		                    ALTER DATABASE [" + dbName + @"] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
                            EXEC master.dbo.sp_detach_db @dbname = N'" + dbName + @"', @skipchecks = 'false'
	                    end
                    end";

            var command = new SqlCommand(detatch, connection);
            command.CommandType = System.Data.CommandType.Text;
            command.ExecuteNonQuery();
            return command;
        }


    }
}
