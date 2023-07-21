using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Employees;
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

                var matches = await db.TimeSheetApprovals.Where(x => x.WeekId == weekId && x.EmployeeId == employeeId).ToListAsync();
                var hours = await db.WeeklyData.SingleOrDefaultAsync(x => x.EmployeeId == employeeId && x.WeekId == weekId);
                if(!(matches?.Any() ?? false))
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
                else if(matches.Count > 1)
                {
                    var matchToKeep = matches.OrderByDescending(x => x.TimeSheetApprovalId).First();
                    var toClean = matches.Where(x => x.TimeSheetApprovalId != matchToKeep.TimeSheetApprovalId);
                    foreach(var deleteMe in toClean)
                    {
                        db.TimeSheetApprovals.Remove(deleteMe);
                    }
                    await db.SaveChangesAsync();
                }

                var match = matches.First();
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
                    TotalOverTimeHours = hours?.TotalOverTimeHours ?? 0.00m,
                    TotalRegularHours = hours?.TotalRegularHours ?? 0.00m,
                    SubmittedDate = match.SubmittedDate,
                    IsHidden = match.IsHidden
                };
            }
        }
        public const int admin_employee_id = 1;

        private async Task<Dictionary<(int employeeId, int weekId), TimeSheetApproval>> SearchForSavedApprovals(OrionDbContext db, int startWeek, int endWeek, params TimeApprovalStatus[] withTimeApprovalStatus)
        {
            var statusAsString = withTimeApprovalStatus.Select(z => z.ToString()).ToArray();
            var baseQuery = db.TimeSheetApprovals
                                    .Include(z => z.Employee)
                                    .Where(x => x.WeekId >= startWeek && x.WeekId <= endWeek && statusAsString.Contains(x.TimeApprovalStatus));

            var approvals = await baseQuery.ToListAsync();
            var approverIds = approvals.Select(x => x.ApproverEmployeeId).Where(x => x.HasValue).Distinct().ToArray();
            return approvals.ToDictionary(x => (x.EmployeeId, x.WeekId), x => x);
        }

        public async Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var allEmps = await db.Employees.Include(x => x.UserRole)
                                                .Where(x => x.EmployeeId != admin_employee_id && x.UserRole.Name != UserRoleName.Disabled).ToListAsync();
                

                var startWeek = beginDateInclusive.HasValue ? WeekDTO.CreateWithWeekContaining(beginDateInclusive.Value).WeekId.Value : WeekDTO.CreateWithWeekContaining(DateTime.Now.AddDays(-7)).WeekId.Value;
                var endWeek = endDateInclusive.HasValue ? WeekDTO.CreateWithWeekContaining(endDateInclusive.Value).WeekId.Value : WeekDTO.CreateWithWeekContaining(DateTime.Now.AddDays(31)).WeekId.Value;

                if(startWeek > endWeek)
                    endWeek = startWeek;

                var approvals = await SearchForSavedApprovals(db, startWeek, endWeek, withTimeApprovalStatus);

                var approverNames = approvals.Values.Select(x => x.ApproverEmployeeId).Where(x => x.HasValue).Distinct().ToDictionary(x => x.Value, x => allEmps.FirstOrDefault(a => a.EmployeeId == x.Value)?.UserName);




                var weeklyHourTotals = await db.WeeklyData.Where(x => x.WeekId >= startWeek && x.WeekId <= endWeek).ToListAsync();
                var hourTotalsByEmpAndWeek = weeklyHourTotals.ToDictionary(x => (x.EmployeeId, x.WeekId), x => (x.TotalOverTimeHours, x.TotalRegularHours));


                var matches = new List<TimeApprovalDTO>();
                for(int weekid = startWeek; weekid <= endWeek; weekid++)
                {
                    foreach(var emp in allEmps)
                    {
                        if(!approvals.TryGetValue((emp.EmployeeId, weekid), out var x))
                        {
                            x = new TimeSheetApproval()
                            {
                                Employee = emp,
                                EmployeeId = emp.EmployeeId,
                                IsHidden = false,
                                WeekId = weekid
                            };
                        }

                        var temp = new TimeApprovalDTO()
                        {
                            ApprovalDate = x.ApprovalDate,
                            ApproverName = approverNames.TryGetValue(x.ApproverEmployeeId ?? 0, out var approverName) ? approverName : null,
                            EmployeeName = $"{x.Employee.Last}, {x.Employee.First}",
                            EmployeeId = x.EmployeeId,
                            ResponseReason = x.ResponseReason,
                            TimeApprovalStatus = string.IsNullOrWhiteSpace(x.TimeApprovalStatus) ? TimeApprovalStatus.Unkown : Enum.Parse<TimeApprovalStatus>(x.TimeApprovalStatus),
                            WeekId = x.WeekId,
                            WeekStartDate = WeekDTO.CreateForWeekId(x.WeekId).WeekStart,
                            SubmittedDate = x.SubmittedDate,
                            TotalOverTimeHours = 0.00m,
                            TotalRegularHours = 0.00m,
                            IsHidden = x.IsHidden
                        };

                        if(hourTotalsByEmpAndWeek.TryGetValue((emp.EmployeeId, weekid), out var fromDb))
                        {
                            temp.TotalOverTimeHours = fromDb.TotalOverTimeHours;
                            temp.TotalRegularHours = fromDb.TotalRegularHours;
                        }
                        matches.Add(temp);
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
