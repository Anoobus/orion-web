using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.Common
{
    public class Result
    {
        public Result(bool successful, params string[] errors)
        {
            Successful = successful;
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
