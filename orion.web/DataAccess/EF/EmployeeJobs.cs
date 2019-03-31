
namespace orion.web.DataAccess.EF
{
    public class EmployeeJob
    {
        public int EmployeeJobId { get; set; }
        public int JobId { get; set; }
        public int EmployeeId { get; set; }
        public virtual Job Job { get; set; }
    }
}
