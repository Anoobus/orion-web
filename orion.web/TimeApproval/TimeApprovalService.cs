﻿using Microsoft.EntityFrameworkCore;
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
        Task<TimeApprovalDTO> GetAsync(int weekId, int employeeId);
        Task Save(TimeApprovalDTO timeApprovalDTO);
        Task Hide(int weekId, int employeeId);
        Task<IEnumerable<TimeApprovalDTO>> GetByStatus(DateTime? beginDateInclusive = null, DateTime? endDateInclusive = null, params TimeApprovalStatus[] withTimeApprovalStatus);
        Task UpdateTimeTotals(int weekId, int employeeId, decimal overtime, decimal regular);
    }
    public class TimeApprovalService : ITimeApprovalService
    {
        private readonly OrionDbContext db;

        public TimeApprovalService(OrionDbContext db)
        {
            this.db = db;
        }
        public async Task<TimeApprovalDTO> GetAsync(int weekId, int employeeId)
        {
            var emp = await db.Employees.SingleAsync(x => x.EmployeeId == employeeId);

            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.WeekId == weekId && x.EmployeeId == employeeId);
            if(match == null)
            {
                return new TimeApprovalDTO()
                {
                    EmployeeName = emp.UserName,
                    EmployeeId = emp.EmployeeId,
                    WeekId = weekId,
                    TimeApprovalStatus = TimeApprovalStatus.Unkown,
                    IsHidden = false
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
                    TotalOverTimeHours = match.TotalOverTimeHours,
                    TotalRegularHours = match.TotalRegularHours,
                    SubmittedDate = match.SubmittedDate,
                    IsHidden = match.IsHidden
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
            return match.Select(x => new TimeApprovalDTO()
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
                TotalOverTimeHours = x.TotalOverTimeHours,
                TotalRegularHours = x.TotalRegularHours,
                IsHidden = x.IsHidden
            }).ToList();
        }

        public async Task UpdateTimeTotals(int weekId, int employeeId, decimal overtime, decimal regular)
        {
            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.WeekId == weekId && x.EmployeeId == employeeId);
            if(match != null)
            {
                match.TotalOverTimeHours = overtime;
                match.TotalRegularHours = regular;
                await db.SaveChangesAsync();
            }
            else
            {
                await Save(new TimeApprovalDTO()
                {
                    EmployeeId = employeeId,
                    TimeApprovalStatus = TimeApprovalStatus.Unkown,
                    TotalOverTimeHours = overtime,
                    TotalRegularHours = regular,
                    WeekId = weekId,
                    WeekStartDate = WeekDTO.CreateForWeekId(weekId).WeekStart,
                });
            }
        }

        public async Task Save(TimeApprovalDTO timeApprovalDTO)
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
            match.TotalOverTimeHours = timeApprovalDTO.TotalOverTimeHours;
            match.TotalRegularHours = timeApprovalDTO.TotalRegularHours;
            match.ApproverEmployeeId = approver;
            match.ResponseReason = match.ResponseReason ?? "" + timeApprovalDTO.ResponseReason;
            match.TimeApprovalStatus = timeApprovalDTO.TimeApprovalStatus.ToString();
            match.IsHidden = timeApprovalDTO.IsHidden;
            await db.SaveChangesAsync();
        }

        public async Task Hide(int weekId, int employeeId)
        {
            var rec = await GetAsync(weekId, employeeId);
            rec.IsHidden = true;
            await Save(rec);
        }
    }
}
