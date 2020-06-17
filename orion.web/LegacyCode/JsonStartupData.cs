using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace orion.web.ApplicationStartup
{
    public class JsonStartupData
    {
        private readonly string folder;

        public JsonStartupData(string folder = null)
        {
            this.folder = folder ?? @"C:\temp\project-data\";
        }

        public void WriteIt<T>(IEnumerable<T> items)
        {
            var sites = $"{typeof(T).Name}s.json";
            var stringSites = Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
            var fileName = Path.Combine(folder, sites);
            if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            System.IO.File.WriteAllText(fileName, stringSites);
        }

        public IEnumerable<T> ReadIt<T>()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePrefix = $"{typeof(Program).Namespace}.project_data";
            var file = $"{typeof(T).Name}s.json";
            var resourceName = $"{resourcePrefix}.{file}";
            var resoureces = assembly.GetManifestResourceNames();
            using(Stream stream = assembly.GetManifestResourceStream(resourceName))
            using(StreamReader reader = new StreamReader(stream))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(reader.ReadToEnd());
            }
        }
    }
}
