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
            var logFileName = Path.Combine(filePath, $"{dbName}.ldf");
            Serilog.Log.Information($"Opening connection to [server=(localdb)\\mssqllocaldb;Initial Catalog=master] to ensure {dbFileName} is mounted");
            using (var connection = new SqlConnection(@"server=(localdb)\mssqllocaldb;Initial Catalog=master"))
            {
                connection.Open();
                //detatch only when the file is mounted currently from a different location
                DetatchExistingDb(dbName,dbFileName, connection);
                if (!File.Exists(dbFileName))
                {
                    //orion.web_Data
                    Serilog.Log.Information($"{dbFileName} does not exist yet");
                    if (File.Exists(backupFileName))
                    {
                        Serilog.Log.Information($"creating {dbFileName} from {backupFileName}");
                        string restore = string.Format(@"RESTORE DATABASE [{1}] FROM DISK='{0}\{1}.bak'
                        WITH FILE = 1,
                        MOVE N'{1}_Data' TO N'{0}\{1}.mdf',
                        MOVE N'{1}_Log' TO N'{0}\{1}.ldf',
                        NOUNLOAD,  REPLACE,  STATS = 1", filePath, dbName);
                        var command = new SqlCommand(restore, connection);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        //Serilog.Log.Information($"creating {dbFileName} using an empty db");
                        //CreateNewEmptyMdf(filePath, dbName, connection);
                    }
                }
                else
                {
                    Serilog.Log.Information($"Skipping creat/mount of {dbFileName} as it should already be good to go");
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
                        EXEC master.dbo.sp_detach_db @dbname = N'" + dbName + @"', @skipchecks = 'true'
                    end";

            var command = new SqlCommand(detatch, connection);
            command.CommandType = System.Data.CommandType.Text;
            command.ExecuteNonQuery();
            return command;
        }


    }
}
