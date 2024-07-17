using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeEntries
{
    public interface IRemoveRowCommand
    {
        Task<Result> RemoveRow(int employeeId, int weekId, int newTaskId, int newJobId);
    }

    public class RemoveRowCommand : IRemoveRowCommand, IAutoRegisterAsSingleton
    {
        private readonly IEmployeeRepository employeeService;

        // private readonly IWeekService weekService;
        private readonly ITimeService timeService;
        private readonly ITimeSpentRepository timeSpentRepository;

        public RemoveRowCommand(
            IEmployeeRepository employeeService,

            // IWeekService weekService,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            this.employeeService = employeeService;

            // this.weekService = weekService;
            this.timeService = timeService;
            this.timeSpentRepository = timeSpentRepository;
        }

        public async Task<Result> RemoveRow(int employeeId, int weekId, int taskId, int jobId)
        {
            await timeService.DeleteAllEntries(weekId, taskId, jobId, employeeId);

            return new Result(true);
        }
    }
}
