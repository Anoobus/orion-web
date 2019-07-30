using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using orion.web.Expense;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{

    public interface IExpenseService : IRegisterByConvention
    {
        Task<IEnumerable<ExpenseDTO>> GetExpensesForEmployee(int employeeId, int weekId);
        Task SaveExpensesForEmployee(ExpenseDTO expense);
        Task<ExpenseDTO> GetExpenseById(int ExpenseItemId);
    }
    public class ExpenseService : IExpenseService
    {
        private readonly OrionDbContext db;
        private readonly IJobService _jobService;

        public ExpenseService(OrionDbContext db, IJobService jobService)
        {
            this.db = db;
            _jobService = jobService;
        }

        public async Task<ExpenseDTO> GetExpenseById(int ExpenseItemId)
        {
            var x = await db.Expenses.Where(z => z.ExpenseItemId == ExpenseItemId).SingleOrDefaultAsync();
            if (x == null)
                return null;

            return  new ExpenseDTO()
            {
                AddtionalNotes = x.AdditionalNotes,
                Amount = x.Amount,
                AttatchmentName = x.AttachmentName,
                Classification = x.Classification,
                RelatedJob = await _jobService.GetForJobId(x.JobId),
                SaveDate = x.CreateDate,
                AttachmentId = Guid.TryParse(x.AttachmentUploadId, out var temp) ? temp : new Guid?(),
                EmployeeId = x.EmployeeId,
                WeekId = x.WeekId,
                 ExpenseItemId = x.ExpenseItemId
            };
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesForEmployee(int employeeId, int weekId)
        {
            var entries = await db.Expenses.Where(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToListAsync();
            return await Task.WhenAll(entries.Select(async x => new ExpenseDTO()
            {
                AddtionalNotes = x.AdditionalNotes,
                Amount = x.Amount,
                AttatchmentName = x.AttachmentName,
                Classification = x.Classification,
                RelatedJob = await _jobService.GetForJobId(x.JobId),
                SaveDate = x.CreateDate,
                AttachmentId = Guid.TryParse(x.AttachmentUploadId, out var temp) ? temp : new Guid?(),
                EmployeeId = x.EmployeeId,
                WeekId = x.WeekId,
                ExpenseItemId = x.ExpenseItemId
            }));
        }

        public async Task SaveExpensesForEmployee(ExpenseDTO expense)
        {
            db.Expenses.Add(new ExpenseItem()
            {
                AdditionalNotes = expense.AddtionalNotes,
                Amount = expense.Amount,
                AttachmentName = expense.AttatchmentName,
                AttachmentUploadId = expense.AttachmentId.HasValue ? expense.AttachmentId.Value.ToString() : null,
                Classification = expense.Classification,
                CreateDate = DateTimeOffset.Now,
                EmployeeId = expense.EmployeeId,
                JobId = expense.RelatedJob.JobId,
                WeekId = expense.WeekId
            });
            await db.SaveChangesAsync();
        }
    }
}
