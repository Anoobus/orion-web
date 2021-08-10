using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface IAddNewJobTaskComboCommand
    {
        Task<CommandResult> AddNewJobTaskCombo(int employeeId,  int weekId, int newTaskId, int newJobId);
    }
    public class AddNewJobTaskComboCommand : IAddNewJobTaskComboCommand, IAutoRegisterAsSingleton
    {
        private readonly IEmployeeRepository _employeeService;
        private readonly IJobsRepository _jobsRepository;

        private readonly ITimeService _timeService;
        private readonly ITimeSpentRepository _timeSpentRepository;

        public AddNewJobTaskComboCommand(IEmployeeRepository employeeService,
            IJobsRepository jobsRepository,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            _employeeService = employeeService;
            _jobsRepository = jobsRepository;
            _timeService = timeService;
            _timeSpentRepository = timeSpentRepository;
        }
        public async Task<CommandResult> AddNewJobTaskCombo(int employeeId,  int weekId, int newTaskId, int newJobId)
        {
            throw new Exception("Testing logger here!");
            var j = await _jobsRepository.GetForJobId(newJobId);
            if(j.JobStatusId != JobStatus.Enabled)
            {
                return new CommandResult(false, new[] { $"Job {j.FullJobCodeWithName} has been closed. In order to use it, an administrator must open it." });
            }
            var entryForEveryDayOfWeek = _timeSpentRepository.CreateEmptyWeekForCombo( weekId, newTaskId, newJobId, employeeId).ToList();
            foreach (var day in entryForEveryDayOfWeek)
            {
                await _timeService.SaveAsync( weekId, employeeId, day);
            }
            return new CommandResult(true);

        }
    }
}
