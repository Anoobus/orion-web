namespace orion.web.DataAccess.EF
{

    public class Job
    {
        public int JobId { get; set; }       
        public string JobCode { get; set; }
        public string JobName { get; set; }

        public int TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }


        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int SiteId { get; set; }
        public Site Site { get; set; }

        public decimal TargetHours { get; set; }

    }
}
