using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.BLL.JobTasks
{
    public enum TaskReportingType
    {
        Regular = 0,
        PTO = 1,
        Holiday = 2,
        ExcusedWithPay = 3,
        ExcusedNoPay = 4
    }
    public class TaskReportingTypeDto
    {
        public int Id { get; set; }
        public string Name => Enum.ToString();
        public TaskReportingType Enum { get; set; }
    }
}
