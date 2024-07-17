using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Orion.Web.DataAccess.EF
{
    [Index(nameof(SupervisorEmployeeId), nameof(ReportEmployeeId), IsUnique = true)]
    public class EmployeeDirectReport
    {
        public int EmployeeDirectReportId { get; set; }
        public int SupervisorEmployeeId { get; set; }
        public int ReportEmployeeId { get; set; }

        [ForeignKey(nameof(SupervisorEmployeeId))]
        public virtual Employee Supervisor { get; set; }
    }
}
