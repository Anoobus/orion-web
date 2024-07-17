using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeEntries
{
    public interface IAddNewJobTaskComboCommand
    {
        Task<Result> AddNewJobTaskCombo(int employeeId, int weekId, int newTaskId, int newJobId);
    }

    public class AddNewJobTaskComboCommand : IAddNewJobTaskComboCommand, IAutoRegisterAsSingleton
    {
        private readonly IEmployeeRepository _employeeService;
        private readonly IJobsRepository _jobsRepository;

        private readonly ITimeService _timeService;
        private readonly ITimeSpentRepository _timeSpentRepository;

        public AddNewJobTaskComboCommand(
            IEmployeeRepository employeeService,
            IJobsRepository jobsRepository,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            _employeeService = employeeService;
            _jobsRepository = jobsRepository;
            _timeService = timeService;
            _timeSpentRepository = timeSpentRepository;
        }

        public async Task<Result> AddNewJobTaskCombo(int employeeId, int weekId, int newTaskId, int newJobId)
        {
            var j = await _jobsRepository.GetForJobId(newJobId);
            if (j.CoreInfo.JobStatusId != JobStatus.Enabled)
            {
                return new Result(false, new[] { $"Job {j.CoreInfo.FullJobCodeWithName} has been closed. In order to use it, an administrator must open it." });
            }

            var entryForEveryDayOfWeek = _timeSpentRepository.CreateEmptyWeekForCombo(weekId, newTaskId, newJobId, employeeId).ToList();
            foreach (var day in entryForEveryDayOfWeek)
            {
                await _timeService.SaveAsync(employeeId, day);
            }

            return new Result(true);
        }
    }
}
