using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.DataAccess.EF
{
    public class WeekOfHours
    {
        public int EmployeeId { get; set; }
        public int WeekId { get; set; }
        public decimal TotalRegularHours { get; set; }
        public decimal TotalOverTimeHours { get; set; }
    }
}
