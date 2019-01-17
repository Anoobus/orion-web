
namespace orion.web.DataAccess.EF
{
    public class JobTask
    {
        public int JobTaskId { get; set; }
        public string LegacyCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TaskCategoryId { get; set; }
        public int UsageStatusId { get; set; }
        public virtual TaskCategory TaskCategory { get; set; }
        public virtual UsageStatus UsageStatus { get; set; }
    }
}
