using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.PayPeriod;

namespace Orion.Web.Controllers
{
    [Authorize]
    [Route("api/PayPeriod")]
    public class PayPeriodController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string startDateIdentifier = null, [FromQuery] int pageSize = 15)
        {
            var midDatePoint = DateTime.Now;
            var allEntries = new List<PayPeriodDTO>();

            if (!string.IsNullOrWhiteSpace(startDateIdentifier))
            {
                var expr = @"(?<year>[0-9]{4})\.(?<month>[0-9]{2})\.(?<day>[0-9]{2})";
                var parts = Regex.Match(startDateIdentifier, expr);
                midDatePoint = new DateTime(int.Parse(parts.Groups["year"].Value), int.Parse(parts.Groups["month"].Value), int.Parse(parts.Groups["day"].Value));
            }

            var all = PayPeriodRepository.GetPPRange(midDatePoint, pageSize);

            return Json(all);
        }
    }
}
