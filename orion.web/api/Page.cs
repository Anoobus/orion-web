using Microsoft.AspNetCore.Mvc;
using orion.web.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    public class Page<T> 
    {

        public T[] Data { get; set; }
        public Meta Meta { get; set; }

        public ActionResult AsActionResult()
        {
            return new OkObjectResult(this);
        }
    }
    public class Meta
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public long? Total { get; set; }
    }
}
