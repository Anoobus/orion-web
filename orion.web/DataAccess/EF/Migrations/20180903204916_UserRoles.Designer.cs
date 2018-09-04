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
    [Migration("20180903204916_UserRoles")]
    partial class UserRoles
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
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

                    b.Property<string>("Name");

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

                    b.Property<int?>("EmployeeId");

                    b.Property<int>("JobId");

                    b.HasKey("EmployeeJobId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("JobId");

                    b.ToTable("EmployeeJob");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("JobCode");

                    b.Property<string>("JobName");

                    b.Property<int>("SiteId");

                    b.Property<decimal>("TargetHours");

                    b.Property<int>("TaskCategoryId");

                    b.HasKey("JobId");

                    b.HasIndex("ClientId");

                    b.HasIndex("SiteId");

                    b.HasIndex("TaskCategoryId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.Property<int>("JobTaskId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("ShortName");

                    b.Property<int>("TaskCategoryId");

                    b.HasKey("JobTaskId");

                    b.HasIndex("TaskCategoryId");

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
                        .HasForeignKey("EmployeeId");

                    b.HasOne("orion.web.DataAccess.EF.Job", "Job")
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

                    b.HasOne("orion.web.DataAccess.EF.Site", "Site")
                        .WithMany("Jobs")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("orion.web.DataAccess.EF.TaskCategory", "TaskCategory")
                        .WithMany()
                        .HasForeignKey("TaskCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("orion.web.DataAccess.EF.JobTask", b =>
                {
                    b.HasOne("orion.web.DataAccess.EF.TaskCategory", "TaskCategory")
                        .WithMany()
                        .HasForeignKey("TaskCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
