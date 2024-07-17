using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Orion.Web.Common;
using Orion.Web.Util.IoC;

namespace Orion.Web.Expense
{
    public interface IUploadLocationResolver
    {
        string GetUploadPath();
    }

    public class UploadLocationResolver : IUploadLocationResolver, IAutoRegisterAsSingleton
    {
        private readonly IConfiguration _config;

        public UploadLocationResolver(IConfiguration config)
        {
            _config = config;
        }

        public string GetUploadPath()
        {
            var currentLocation = new FileInfo(this.GetType().Assembly.Location);
            var overridePath = _config.GetValue<string>("OverrideUploadDataPath");
            var dir = Path.Combine(overridePath ?? currentLocation.DirectoryName, "upload-data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }
}
