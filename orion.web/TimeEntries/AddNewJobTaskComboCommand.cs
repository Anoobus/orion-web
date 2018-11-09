using orion.web.Common;
using orion.web.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface IAddNewJobTaskComboCommand : IRegisterByConvention
    {
        Task<CommandResult> AddNewJobTaskCombo(string employeeName, int year, int weekId, int newTaskId, int newJobId);
    }
    public class AddNewJobTaskComboCommand : IAddNewJobTaskComboCommand
    {
        private readonly IEmployeeService employeeService;
        private readonly IWeekService weekService;
        private readonly ITimeService timeService;
        private readonly ITimeSpentRepository timeSpentRepository;

        public AddNewJobTaskComboCommand(IEmployeeService employeeService,
            IWeekService weekService,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            this.employeeService = employeeService;
            this.weekService = weekService;
            this.timeService = timeService;
            this.timeSpentRepository = timeSpentRepository;
        }
        public async Task<CommandResult> AddNewJobTaskCombo(string employeeName, int year, int weekId, int newTaskId, int newJobId)
        {
            
            var employeeId = employeeService.GetSingleEmployee(employeeName).EmployeeId;
            var entryForEveryDayOfWeek = timeSpentRepository.CreateEmptyWeekForCombo(year, weekId, newTaskId, newJobId, employeeId).ToList();
                foreach(var day in entryForEveryDayOfWeek)
                {
                    await timeService.SaveAsync(year, weekId, employeeName, day);
                }
            return new CommandResult(true);
    
        }
    }
}
