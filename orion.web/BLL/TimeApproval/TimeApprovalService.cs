using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeApprovalService
    {
        Task<TimeApprovalDTO> GetAsync(int weekId, int employeeId);
        Task Save(TimeApprovalDTO timeApprovalDTO);
        Task Hide(int weekId, int employeeId);
        Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus);
    }
    public class TimeApprovalService : ITimeApprovalService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public TimeApprovalService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<TimeApprovalDTO> GetAsync(int weekId, int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var emp = await db.Employees.SingleAsync(x => x.EmployeeId == employeeId);

                var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.WeekId == weekId && x.EmployeeId == employeeId);
                var hours = await db.WeeklyData.SingleOrDefaultAsync(x => x.EmployeeId == employeeId && x.WeekId == weekId);
                if(match == null)
                {
                    return new TimeApprovalDTO()
                    {
                        EmployeeName = emp.UserName,
                        EmployeeId = emp.EmployeeId,
                        WeekId = weekId,
                        TimeApprovalStatus = TimeApprovalStatus.Unkown,
                        IsHidden = false,
                        TotalOverTimeHours = 0.00m,
                        TotalRegularHours = 0.00m
                    };
                }
                else
                {
                    var approver = db.Employees.FirstOrDefault(x => x.EmployeeId == match.ApproverEmployeeId)?.UserName ?? "";
                    Enum.TryParse<TimeApprovalStatus>(match.TimeApprovalStatus, out var mapped);
                    return new TimeApprovalDTO()
                    {
                        ApprovalDate = match.ApprovalDate,
                        ApproverName = approver,
                        EmployeeName = emp.UserName,
                        EmployeeId = emp.EmployeeId,
                        ResponseReason = match.ResponseReason,
                        TimeApprovalStatus = mapped,
                        WeekId = match.WeekId,
                        TotalOverTimeHours = hours.TotalOverTimeHours,
                        TotalRegularHours = hours.TotalRegularHours,
                        SubmittedDate = match.SubmittedDate,
                        IsHidden = match.IsHidden
                    };
                }
            }
        }

        public async Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus)
        {
            using(var db = _contextFactory.CreateDb())
            {

                var statusAsString = withTimeApprovalStatus.Select(z => z.ToString()).ToArray();
                var baseQuery = db.TimeSheetApprovals
                                        .Include(z => z.Employee)
                                        .Where(x => statusAsString.Contains(x.TimeApprovalStatus));

                if(beginDateInclusive.HasValue)
                {
                    var week = WeekDTO.CreateWithWeekContaining(beginDateInclusive.Value);
                    baseQuery = baseQuery.Where(x => x.WeekId >= week.WeekId.Value);
                }

                if(endDateInclusive.HasValue)
                {
                    var week = WeekDTO.CreateWithWeekContaining(endDateInclusive.Value);
                    baseQuery = baseQuery.Where(x => x.WeekId <= week.WeekId.Value);
                }



                var match = await baseQuery.ToListAsync();

                var approverIds = match.Select(x => x.ApproverEmployeeId).Where(x => x.HasValue).Distinct().ToArray();
                var approverNames = await db.Employees.Where(x => approverIds.Contains(x.EmployeeId)).ToListAsync();
                var matches =  match.Select(x => new TimeApprovalDTO()
                {
                    ApprovalDate = x.ApprovalDate,
                    ApproverName = approverNames.FirstOrDefault(z => z.EmployeeId == x.ApproverEmployeeId)?.UserName,
                    EmployeeName = x.Employee.UserName,
                    EmployeeId = x.EmployeeId,
                    ResponseReason = x.ResponseReason,
                    TimeApprovalStatus = string.IsNullOrWhiteSpace(x.TimeApprovalStatus) ? TimeApprovalStatus.Unkown : Enum.Parse<TimeApprovalStatus>(x.TimeApprovalStatus),
                    WeekId = x.WeekId,
                    WeekStartDate = WeekDTO.CreateForWeekId(x.WeekId).WeekStart,
                    SubmittedDate = x.SubmittedDate,
                    TotalOverTimeHours = 0.00m,
                    TotalRegularHours = 0.00m,
                    IsHidden = x.IsHidden
                }).ToList();


                var weeks = matches.Select(x => x.WeekId).Distinct();
                var allWeeks = await db.WeeklyData.Where(x => weeks.Contains(x.WeekId)).ToListAsync();
                var employeeWeeks = allWeeks.ToDictionary(x => (x.EmployeeId, x.WeekId), x => (x.TotalOverTimeHours, x.TotalRegularHours));

                foreach(var emp in matches)
                {
                    if(employeeWeeks.TryGetValue((emp.EmployeeId, emp.WeekId), out var fromDb))
                    {
                        emp.TotalOverTimeHours = fromDb.TotalOverTimeHours;
                        emp.TotalRegularHours = fromDb.TotalRegularHours;
                    }
                }
                return matches;
            }
        }

        public async Task Save(TimeApprovalDTO timeApprovalDTO)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.WeekId == timeApprovalDTO.WeekId && x.EmployeeId == timeApprovalDTO.EmployeeId);
                if(match == null)
                {
                    match = new TimeSheetApproval()
                    {
                        EmployeeId = timeApprovalDTO.EmployeeId,
                        WeekId = timeApprovalDTO.WeekId,
                    };
                    db.TimeSheetApprovals.Add(match);
                }
                var approver = db.Employees.FirstOrDefault(x => x.UserName == timeApprovalDTO.ApproverName)?.EmployeeId ?? 0;
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
                match.ApproverEmployeeId = approver;
                match.ResponseReason = match.ResponseReason ?? "" + timeApprovalDTO.ResponseReason;
                match.TimeApprovalStatus = timeApprovalDTO.TimeApprovalStatus.ToString();
                match.IsHidden = timeApprovalDTO.IsHidden;
                await db.SaveChangesAsync();
            }
        }

        public async Task Hide(int weekId, int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var rec = await GetAsync(weekId, employeeId);
                rec.IsHidden = true;
                await Save(rec);
            }
        }
    }
}
