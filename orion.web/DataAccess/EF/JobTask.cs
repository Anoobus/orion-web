
namespace orion.web.DataAccess.EF
{
    public class JobTask
    {
        public int JobTaskId { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int TaskCategoryId { get; set; }
        public virtual TaskCategory TaskCategory { get; set; }
    }
}
