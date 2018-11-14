using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeApprovalService : IRegisterByConvention
    {
        Task<TimeApprovalDTO> Get(int yearId, int weekId, string employeeName);
        Task Save(TimeApprovalDTO timeApprovalDTO);
        Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus);
    }
    public class TimeApprovalService : ITimeApprovalService
    {
        private readonly OrionDbContext db;
        private readonly IWeekService weekService;

        public TimeApprovalService(OrionDbContext db, IWeekService weekService)
        {
            this.db = db;
            this.weekService = weekService;
        }
        public async Task<TimeApprovalDTO> Get(int yearId, int weekId, string employeeName)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == employeeName)?.EmployeeId ?? -1;

            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.Year == yearId && x.WeekId == weekId && x.EmployeeId == empId);
            if(match == null)
            {
                return new TimeApprovalDTO()
                {
                    EmployeeName = employeeName,
                    WeekId = weekId,
                    Year = yearId,
                    TimeApprovalStatus = TimeApprovalStatus.Unkown
                };
            }
            else
            {
                var approver = db.Employees.FirstOrDefault(x => x.EmployeeId == match.ApproverEmployeeId)?.Name ?? "";
                Enum.TryParse<TimeApprovalStatus>(match.TimeApprovalStatus, out var mapped);
                return new TimeApprovalDTO()
                {
                    ApprovalDate = match.ApprovalDate,
                    ApproverName = approver,
                    EmployeeName = employeeName,
                    ResponseReason = match.ResponseReason,
                    TimeApprovalStatus = mapped,
                    WeekId = match.WeekId,
                    Year = match.Year
                };
            }
        }

        public async Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus)
        {
            var statusAsString = withTimeApprovalStatus.Select(z => z.ToString()).ToArray();
            var baseQuery = db.TimeSheetApprovals
                                    .Include(z => z.Employee)
                                    .Where(x => statusAsString.Contains(x.TimeApprovalStatus));

            if(beginDateInclusive.HasValue)
            {
                var week = weekService.Get(beginDateInclusive.Value);
                baseQuery = baseQuery.Where(x => x.Year > week.Year || (x.Year == week.Year &&  x.WeekId >= week.WeekId ));
            }

            if(endDateInclusive.HasValue)
            {
                var week = weekService.Get(endDateInclusive.Value);
                baseQuery = baseQuery.Where(x => x.Year < week.Year || (x.Year == week.Year && x.WeekId <= week.WeekId ));
            }

            var match = await baseQuery.ToListAsync();

            var approverIds = match.Select(x => x.ApproverEmployeeId).Where(x => x.HasValue).Distinct().ToArray();
            var approverNames = await db.Employees.Where(x => approverIds.Contains(x.EmployeeId)).ToListAsync();
            return match.Select(x => new TimeApprovalDTO()
            {
                ApprovalDate = x.ApprovalDate,
                ApproverName = approverNames.FirstOrDefault(z => z.EmployeeId == x.ApproverEmployeeId)?.Name,
                EmployeeName = x.Employee.Name,
                ResponseReason = x.ResponseReason,
                TimeApprovalStatus = string.IsNullOrWhiteSpace(x.TimeApprovalStatus) ? TimeApprovalStatus.Unkown : Enum.Parse<TimeApprovalStatus>(x.TimeApprovalStatus),
                WeekId = x.WeekId,
                Year = x.Year,
                 WeekStartDate = weekService.GetWeekDate(x.Year,x.WeekId, WeekIdentifier.WEEK_START),
                SubmittedDate = x.SubmittedDate,
                 TotalOverTimeHours = x.TotalOverTimeHours,
                  TotalRegularHours = x.TotalRegularHours
            }).ToList();
        }

        public async Task Save(TimeApprovalDTO timeApprovalDTO)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == timeApprovalDTO.EmployeeName)?.EmployeeId ?? -1;

            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.Year == timeApprovalDTO.Year && x.WeekId == timeApprovalDTO.WeekId && x.EmployeeId == empId);
            if(match == null)
            {
                match = new TimeSheetApproval()
                {
                    EmployeeId = empId,
                    WeekId = timeApprovalDTO.WeekId,
                    Year = timeApprovalDTO.Year,
                     
                };
                db.TimeSheetApprovals.Add(match);
            }
            var approver = db.Employees.FirstOrDefault(x => x.Name == timeApprovalDTO.ApproverName)?.EmployeeId ?? 0;
            if(timeApprovalDTO.TimeApprovalStatus == TimeApprovalStatus.Approved)
            {
                match.ApprovalDate = DateTime.Now;
                if(match.SubmittedDate == null)
                {
                    match.SubmittedDate = DateTime.Now;
                }
            }
            else
            {
                match.ApprovalDate = null;
            }
            if(timeApprovalDTO.TimeApprovalStatus == TimeApprovalStatus.Submitted)
            {
                match.SubmittedDate = DateTime.Now;
            }
            match.TotalOverTimeHours = timeApprovalDTO.TotalOverTimeHours;
            match.TotalRegularHours = timeApprovalDTO.TotalRegularHours;
            match.ApproverEmployeeId = approver;
            match.ResponseReason = match.ResponseReason ?? "" + timeApprovalDTO.ResponseReason;
            match.TimeApprovalStatus = timeApprovalDTO.TimeApprovalStatus.ToString();
            await db.SaveChangesAsync();
        }
    }
}
