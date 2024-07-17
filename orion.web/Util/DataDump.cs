using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orion.Web.Util
{
    public static class DataDump
    {
        public static string Dump<T>(this T forDump)
        {
            try
            {
                return JsonConvert.SerializeObject(forDump, Formatting.Indented);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return forDump?.ToString();
            }
        }
    }
}
