﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using orion.web.DataAccess.EF;

namespace orion.web.DataAccess.EF.Migrations
{
    [DbContext(typeof(OrionDbContext))]
    [Migration("20190730025046_AddExpenses3")]
    partial class AddExpenses3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("orion.web.DataAccess.EF.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientCode");

                    b.Property<string>("ClientName");

                    b.HasKey("ClientId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("First");

                    b.Property<bool>("IsExempt");

                    b.Property<string>("Last");

                    b.Property<string>("UserName");

                    b.Property<int>("UserRoleId");

                    b.HasKey("EmployeeId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.EmployeeJob", b =>
                {
                    b.Property<int>("EmployeeJobId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmployeeId");

                    b.Property<int>("JobId");

                    b.HasKey("EmployeeJobId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("JobId");

                    b.ToTable("EmployeeJobs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ExpenseItem", b =>
                {
                    b.Property<int>("ExpenseItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalNotes");

                    b.Property<decimal>("Amount");

                    b.Property<string>("AttachmentName");

                    b.Property<string>("AttachmentUploadId");

                    b.Property<string>("Classification");

                    b.Property<DateTimeOffset>("CreateDate");

                    b.Property<int>("EmployeeId");

                    b.Property<int>("JobId");

                    b.Property<int>("WeekId");

                    b.HasKey("ExpenseItemId");

                    b.HasIndex("JobId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<int>("EmployeeId");

                    b.Property<string>("JobCode");

                    b.Property<string>("JobName");

                    b.Property<int>("JobStatusId");

                    b.Property<int>("SiteId");

                    b.Property<decimal>("TargetHours");

                    b.HasKey("JobId");

                    b.HasIndex("ClientId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("JobStatusId");

                    b.HasIndex("SiteId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobStatus", b =>
                {
                    b.Property<int>("JobStatusId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("JobStatusId");

                    b.ToTable("JobStatuses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.Property<int>("JobTaskId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("LegacyCode");

                    b.Property<string>("Name");

                    b.Property<int>("TaskCategoryId");

                    b.Property<int>("UsageStatusId");

                    b.HasKey("JobTaskId");

                    b.HasIndex("TaskCategoryId");

                    b.HasIndex("UsageStatusId");

                    b.ToTable("JobTasks");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Site", b =>
                {
                    b.Property<int>("SiteID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SiteName");

                    b.HasKey("SiteID");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TaskCategory", b =>
                {
                    b.Property<int>("TaskCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("TaskCategoryId");

                    b.ToTable("TaskCategories");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeEntry", b =>
                {
                    b.Property<int>("TimeEntryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int>("EmployeeId");

                    b.Property<decimal>("Hours");

                    b.Property<int>("JobId");

                    b.Property<decimal>("OvertimeHours");

                    b.Property<int>("TaskId");

                    b.Property<int>("WeekId");

                    b.HasKey("TimeEntryId");

                    b.ToTable("TimeEntries");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeSheetApproval", b =>
                {
                    b.Property<int>("TimeSheetApprovalId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ApprovalDate");

                    b.Property<int?>("ApproverEmployeeId");

                    b.Property<int>("EmployeeId");

                    b.Property<string>("ResponseReason");

                    b.Property<DateTime?>("SubmittedDate");

                    b.Property<string>("TimeApprovalStatus");

                    b.Property<decimal>("TotalOverTimeHours");

                    b.Property<decimal>("TotalRegularHours");

                    b.Property<int>("WeekId");

                    b.HasKey("TimeSheetApprovalId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("TimeSheetApprovals");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.UsageStatus", b =>
                {
                    b.Property<int>("UsageStatusId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("UsageStatusId");

                    b.ToTable("UsageStatuses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("UserRoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Employee", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.EmployeeJob", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Employee")
                        .WithMany("EmployeeJobs")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ExpenseItem", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "RelatedJob")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Job", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.Employee", "ProjectManager")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.JobStatus", "JobStatus")
                        .WithMany()
                        .HasForeignKey("JobStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.Site", "Site")
                        .WithMany("Jobs")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.TaskCategory", "TaskCategory")
                        .WithMany()
                        .HasForeignKey("TaskCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.UsageStatus", "UsageStatus")
                        .WithMany()
                        .HasForeignKey("UsageStatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeSheetApproval", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}