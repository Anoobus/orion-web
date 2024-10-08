﻿// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using NPOI.SS.UserModel;
// using NPOI.XSSF.UserModel;
// using Orion.Web.Common;
// using Orion.Web.DataAccess.EF;
// using System;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;

// namespace Orion.Web.Reports
// {

// public interface IReportservice : IRegisterByConvention
//    {
//        //ReportDTO GetJobDetailReport(DateTime start, DateTime end, int jobId, string reportDisplayName);
//        //ReportDTO GetJobsSummaryReport(DateTime start, DateTime end, bool showEmptyJobs,string reportDisplayName);
//        Task<ReportDTO> GetPayPeriodReportAsync(DateTime payPeriodEnd, string reportDisplayName);
//    }

// public class ReportService : IReportservice
//    {
//        private readonly OrionDbContext db;
//        private readonly IConfiguration configuration;
//        private readonly IJobSummaryQuery jobSummaryQuery;
//        private readonly ISingleJobDetailQuery singleJobDetailQuery;

// public ReportService(OrionDbContext db,
//            IConfiguration configuration, IJobSummaryQuery jobSummaryQuery,
//            ISingleJobDetailQuery singleJobDetailQuery)
//        {

// this.db = db;
//            this.configuration = configuration;
//            this.jobSummaryQuery = jobSummaryQuery;
//            this.singleJobDetailQuery = singleJobDetailQuery;
//        }
//        public ReportDTO GetJobDetailReport(DateTime start, DateTime end, int jobId, string reportDisplayName)
//        {
//            var jobMatch = db.Jobs
//                .Include(x => x.Client)
//                .First(x => x.JobId == jobId);
//            return singleJobDetailQuery.Run(start, end, jobId, $"{jobMatch.Client.ClientCode}-{jobMatch.JobCode} {jobMatch.JobName}", reportDisplayName);
//        }

// public ReportDTO GetJobsSummaryReport(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName)
//        {
//            return jobSummaryQuery.Run(start, end, showEmptyJobs, reportDisplayName);
//        }

// public Task<ReportDTO> GetPayPeriodReportAsync(DateTime payPeriodEnd, string reportDisplayName)
//        {
//            throw new NotImplementedException();
//        }
//    }
// }
