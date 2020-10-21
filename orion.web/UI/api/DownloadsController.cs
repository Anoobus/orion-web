using Ionic.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace orion.web.UI.api
{

    public class DownloadModel
    {
        public string ZipFileName { get; set; }
        public string Password { get; set; }
    }

    [Authorize]
    [Route("api/v1/downloads")]
    [ApiController]
    public class DownloadsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DownloadsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("current-db-zip")]
        public async Task<ActionResult> DownloadDb([FromBody] DownloadModel dl)
        {
            if(string.IsNullOrWhiteSpace(dl?.Password) || string.IsNullOrWhiteSpace(dl?.ZipFileName))
            {
                return BadRequest(new
                {
                    Error = "Both password and zipFilename are required!",
                    Supplied = dl
                }); ;
            }

            try
            {
                //bin dir
                var currentDir = new DirectoryInfo(Path.GetFullPath(Assembly.GetExecutingAssembly().Location));
                //app dir
                var parent = currentDir.Parent;
                var backupDir = Path.Combine(parent.FullName, "db-backups");
                if(!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                CleanUpOldData(backupDir);

                var thisBackupId = Guid.NewGuid().ToString();
                var backUpPath = Path.Combine(backupDir, thisBackupId);
                Directory.CreateDirectory(backUpPath);

                var thisUsersBackUpId = Path.Combine(backUpPath, "orion.web.aspnet.identity.bak");
                var thisWebBackUpId = Path.Combine(backUpPath, "orion.web.bak");


                using(var conn = new SqlConnection(_configuration.GetConnectionString("SiteConnection")))
                using(var cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();

                    cmd.CommandText = $"BACKUP DATABASE [orion.web] TO DISK='{thisWebBackUpId}'";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"BACKUP DATABASE [orion.web.aspnet.identity] TO DISK='{thisUsersBackUpId}'";
                    cmd.ExecuteNonQuery();
                }

                var finalZip = Path.Combine(backUpPath, dl.ZipFileName);

                CreatePasswordProtectedZip(finalZip, dl.Password, new[] { thisUsersBackUpId, thisWebBackUpId });

                var bytes = System.IO.File.ReadAllBytes(finalZip);
                return File(bytes, "application/octet-stream", dl.ZipFileName);
            }
            catch(Exception e)
            {
                var res = new ObjectResult(new
                {
                    Error = e,
                    Request = dl
                });
                res.StatusCode = 500;
                return res;
            }


        }

        private void CleanUpOldData(string backUpDir)
        {
            var dirInfo = new DirectoryInfo(backUpDir);
            var toDelete = new List<DirectoryInfo>();
            foreach(var item in dirInfo.GetDirectories())
            {
                if(DateTime.UtcNow.Subtract(item.CreationTimeUtc).TotalDays > 30)
                {
                    toDelete.Add(item);
                }
            }

            foreach(var item in toDelete)
            {
                item.Delete(recursive: true);
            }
        }

        public void CreatePasswordProtectedZip(string zipFullFileName, string password, string[] sourcefiles)
        {
            using(var zip = new ZipFile())
            {
                zip.Password = password;
                foreach(var file in sourcefiles)
                {
                    zip.AddFile(file, string.Empty);
                }
                zip.Save(zipFullFileName);
            }
        }
    }
}
