﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using orion.web.DataAccess.EF;

#nullable disable

namespace orion.web.DataAccessEFMigrations
{
    [DbContext(typeof(OrionDbContext))]
    [Migration("20220305040050_AddExpenseTables")]
    partial class AddExpenseTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("orion.web.DataAccess.EF.ArcFlashlabelExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTimeOffset>("DateOfInvoice")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalLabelsCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPostageCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("ArcFlashlabelExpenditures");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClientId"), 1L, 1);

                    b.Property<string>("ClientName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClientId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.CompanyVehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CompanyVehicles");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.CompanyVehicleExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CompanyVehicleId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("DateVehicleFirstUsed")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("TotalMiles")
                        .HasColumnType("int");

                    b.Property<int>("TotalNumberOfDaysUsed")
                        .HasColumnType("int");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompanyVehicleId");

                    b.HasIndex("JobId");

                    b.ToTable("CompanyVehicleExpenditures");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ContractorExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("OrionPONumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPOContractAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("ContractorExpenditures");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeId"), 1L, 1);

                    b.Property<Guid>("ExternalEmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("First")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsExempt")
                        .HasColumnType("bit");

                    b.Property<string>("Last")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRoleId")
                        .HasColumnType("int");

                    b.HasKey("EmployeeId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.EmployeeJob", b =>
                {
                    b.Property<int>("EmployeeJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeJobId"), 1L, 1);

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.HasKey("EmployeeJobId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("JobId");

                    b.ToTable("EmployeeJobs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ExpenseItem", b =>
                {
                    b.Property<int>("ExpenseItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseItemId"), 1L, 1);

                    b.Property<string>("AdditionalNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("AttachmentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AttachmentUploadId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Classification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("ExpenseItemId");

                    b.HasIndex("JobId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("JobId"), 1L, 1);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("JobCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("JobStatusId")
                        .HasColumnType("int");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.Property<decimal>("TargetHours")
                        .HasColumnType("decimal(18,2)");

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
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("JobStatusId"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("JobStatusId");

                    b.ToTable("JobStatuses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.Property<int>("JobTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("JobTaskId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LegacyCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReportingClassificationId")
                        .HasColumnType("int");

                    b.Property<int>("TaskCategoryId")
                        .HasColumnType("int");

                    b.Property<int>("UsageStatusId")
                        .HasColumnType("int");

                    b.HasKey("JobTaskId");

                    b.HasIndex("TaskCategoryId");

                    b.HasIndex("UsageStatusId");

                    b.ToTable("JobTasks");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.MiscExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("MiscExpenditures");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ScheduleTask", b =>
                {
                    b.Property<int>("ScheduleTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleTaskId"), 1L, 1);

                    b.Property<DateTimeOffset?>("EndDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("OnFriday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnMonday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnSaturday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnSunday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnThursday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnTuesday")
                        .HasColumnType("bit");

                    b.Property<bool>("OnWednesday")
                        .HasColumnType("bit");

                    b.Property<int>("RecurEveryNWeeks")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("TaskName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ScheduleTaskId");

                    b.ToTable("ScheduleTasks");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ScheduleTaskRunLog", b =>
                {
                    b.Property<int>("ScheduleTaskRunLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleTaskRunLogId"), 1L, 1);

                    b.Property<int>("ScheduleTaskId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("TaskCompletionDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("ScheduleTaskRunLogId");

                    b.HasIndex("ScheduleTaskId");

                    b.ToTable("ScheduleTaskRunLogs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Site", b =>
                {
                    b.Property<int>("SiteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SiteID"), 1L, 1);

                    b.Property<string>("SiteName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SiteID");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TaskCategory", b =>
                {
                    b.Property<int>("TaskCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskCategoryId"), 1L, 1);

                    b.Property<bool>("IsInternalOnly")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskCategoryId");

                    b.ToTable("TaskCategories");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeAndExpenceExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("TimeAndExpenceExpenditures");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeEntry", b =>
                {
                    b.Property<int>("TimeEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TimeEntryId"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<decimal>("Hours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<decimal>("OvertimeHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("TimeEntryId");

                    b.ToTable("TimeEntries");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeSheetApproval", b =>
                {
                    b.Property<int>("TimeSheetApprovalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TimeSheetApprovalId"), 1L, 1);

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ApproverEmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("bit");

                    b.Property<string>("ResponseReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TimeApprovalStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.HasKey("TimeSheetApprovalId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("TimeSheetApprovals");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.UsageStatus", b =>
                {
                    b.Property<int>("UsageStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsageStatusId"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UsageStatusId");

                    b.ToTable("UsageStatuses");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoleId"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserRoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.WeekOfHours", b =>
                {
                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalOverTimeHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalRegularHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("WeekId")
                        .HasColumnType("int");

                    b.ToView("vEmployeeeWeeklySummary");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ArcFlashlabelExpenditure", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.CompanyVehicleExpenditure", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.CompanyVehicle", "CompanyVehicle")
                        .WithMany()
                        .HasForeignKey("CompanyVehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CompanyVehicle");

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ContractorExpenditure", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Employee", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.EmployeeJob", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Employee", null)
                        .WithMany("EmployeeJobs")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ExpenseItem", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "RelatedJob")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelatedJob");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Job", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.Employee", "ProjectManager")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.JobStatus", "JobStatus")
                        .WithMany()
                        .HasForeignKey("JobStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.Site", "Site")
                        .WithMany("Jobs")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("JobStatus");

                    b.Navigation("ProjectManager");

                    b.Navigation("Site");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.TaskCategory", "TaskCategory")
                        .WithMany()
                        .HasForeignKey("TaskCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("orion.web.DataAccess.EF.UsageStatus", "UsageStatus")
                        .WithMany()
                        .HasForeignKey("UsageStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaskCategory");

                    b.Navigation("UsageStatus");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.MiscExpenditure", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.ScheduleTaskRunLog", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.ScheduleTask", "ScheduleTask")
                        .WithMany()
                        .HasForeignKey("ScheduleTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleTask");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeAndExpenceExpenditure", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.TimeSheetApproval", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Employee", b =>
                {
                    b.Navigation("EmployeeJobs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Site", b =>
                {
                    b.Navigation("Jobs");
                });
#pragma warning restore 612, 618
        }
    }
}
