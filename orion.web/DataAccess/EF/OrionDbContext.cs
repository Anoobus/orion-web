using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace orion.web.DataAccess.EF
{
    public class MigrationContextFactory : IDesignTimeDbContextFactory<OrionDbContext>
    {
        public OrionDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrionDbContext>();
            optionsBuilder.UseSqlServer(@"Server=tnas.local,56000;User Id=sa;Password=Orion1234!;Database=orion.web;");

            return new OrionDbContext(optionsBuilder.Options);
        }
    }

    public class OrionDbContext : DbContext
    {
        public const string CONN_STRING_NAME = "SiteConnection";
        public OrionDbContext()
            : base(new DbContextOptionsBuilder<OrionDbContext>()
                  .UseSqlServer(@"Server=localhost\\sql2017;Integrated Security=true;Database=orion.web;")
                  .Options)
        { }
        public OrionDbContext(DbContextOptions<OrionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WeekOfHours>().ToView("vEmployeeeWeeklySummary").HasNoKey();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<JobTask> JobTasks { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<UsageStatus> UsageStatuses { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<TimeSheetApproval> TimeSheetApprovals { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<EmployeeJob> EmployeeJobs { get; set; }
        public DbSet<ExpenseItem> Expenses { get; set; }
        public DbSet<WeekOfHours> WeeklyData { get; set; }

        public DbSet<ScheduleTask> ScheduleTasks { get; set; }
        public DbSet<ScheduleTaskRunLog> ScheduleTaskRunLogs { get; set; }

        public DbSet<ContractorExpenditure> ContractorExpenditures { get; set; }
        public DbSet<CompanyVehicle> CompanyVehicles { get; set; }
        public DbSet<CompanyVehicleExpenditure> CompanyVehicleExpenditures { get; set; }
        public DbSet<ArcFlashLabelExpenditure> ArcFlashlabelExpenditures { get; set; }
        public DbSet<MiscExpenditure> MiscExpenditures { get; set; }
        public DbSet<TimeAndExpenceExpenditure> TimeAndExpenceExpenditures { get; set; }


    }
}
