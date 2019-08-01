using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using orion.web.Expense;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{

    public interface IExpenseService : IRegisterByConvention
    {
        Task<IEnumerable<ExpenseDTO>> GetExpensesForEmployee(int employeeId, int weekId);
        Task SaveExpensesForEmployee(ExpenseDTO expense);
        Task<ExpenseDTO> DeleteExpense(int expenseItemId);
        Task<ExpenseDTO> GetExpenseById(int expenseItemId);
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

        public async Task<ExpenseDTO> DeleteExpense(int expenseItemId)
        {
            var match = await db.Expenses.SingleOrDefaultAsync(x => x.ExpenseItemId == expenseItemId);
            if(match != null)
            {
                db.Expenses.Remove(match);
                await db.SaveChangesAsync();
                return await MapToExpenseDTO(match);
            }
            return null;
        }

        public async Task<ExpenseDTO> GetExpenseById(int ExpenseItemId)
        {
            var x = await db.Expenses.Where(z => z.ExpenseItemId == ExpenseItemId).SingleOrDefaultAsync();
            if (x == null)
                return null;
            return await MapToExpenseDTO(x);
        }

        private async Task<ExpenseDTO> MapToExpenseDTO(ExpenseItem x)
        {
            return new ExpenseDTO()
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
            var existing = await db.Expenses.SingleOrDefaultAsync(x => x.ExpenseItemId == expense.ExpenseItemId);
            if (existing == null)
            {
                existing = new ExpenseItem();
                db.Expenses.Add(existing);
            }

            existing.AdditionalNotes = expense.AddtionalNotes;
            existing.Amount = expense.Amount;
            existing.AttachmentName = expense.AttatchmentName;
            existing.AttachmentUploadId = expense.AttachmentId.HasValue ? expense.AttachmentId.Value.ToString() : null;
            existing.Classification = expense.Classification;
            existing.CreateDate = DateTimeOffset.Now;
            existing.EmployeeId = expense.EmployeeId;
            existing.JobId = expense.RelatedJob.JobId;
            existing.WeekId = expense.WeekId;

            await db.SaveChangesAsync();
        }
    }
}
