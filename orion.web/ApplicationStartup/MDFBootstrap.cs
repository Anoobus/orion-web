using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.ApplicationStartup
{
    public class MDFBootstrap
    {
        public static string CreateDbFileIfNotPresent(string filePath, string dbName)
        {
            var fullFileName = Path.Combine(filePath, $"{dbName}.mdf");
            if (!File.Exists(fullFileName))
            {
                var connection = new SqlConnection(@"server=(localdb)\mssqllocaldb");
                using (connection)
                {
                    connection.Open();

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
            }
            return fullFileName;
        }
    }
}
