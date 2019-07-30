﻿using Microsoft.EntityFrameworkCore;

namespace orion.web.DataAccess.EF
{
    public class OrionDbContext : DbContext
    {

        public OrionDbContext(DbContextOptions<OrionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
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

    }
}
