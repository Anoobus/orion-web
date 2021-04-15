using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Util
{
    public static class DataDump
    {
        public static string Dump<T>(this T forDump)
        {
            try
            {
                return JsonConvert.SerializeObject(forDump, Formatting.Indented);
            }
            catch(Exception e)
            {
                return forDump.ToString();
            }

        }
    }
}
