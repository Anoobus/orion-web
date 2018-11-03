using orion.web.Common;
using orion.web.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface IRemoveRowCommand : IRegisterByConvention
    {
        Task<CommandResult> RemoveRow(string employeeName, int year, int weekId, int newTaskId, int newJobId);
    }
    public class RemoveRowCommand : IRemoveRowCommand
    {
        private readonly IEmployeeService employeeService;
        private readonly IWeekService weekService;
        private readonly ITimeService timeService;
        private readonly ITimeSpentRepository timeSpentRepository;

        public RemoveRowCommand(IEmployeeService employeeService,
            IWeekService weekService,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            this.employeeService = employeeService;
            this.weekService = weekService;
            this.timeService = timeService;
            this.timeSpentRepository = timeSpentRepository;
        }
        public async Task<CommandResult> RemoveRow(string employeeName, int year, int weekId, int taskId, int jobId)
        {
            await timeService.DeleteAllEntries(year, weekId, taskId, jobId, employeeName);
           
            return new CommandResult(true);
    
        }
    }
}
