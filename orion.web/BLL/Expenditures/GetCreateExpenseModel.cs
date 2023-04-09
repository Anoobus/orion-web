using System;
using System.Linq;
using System.Threading.Tasks;
using orion.web.BLL.Core;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.UI.Models;
using orion.web.Util.IoC;

namespace orion.web.BLL.Expenditures
{
    public class GetCreateExpenseModelMessage : IProduces<ExpenseViewModel>
    {
        public ExpenditureTypeEnum TargetExpenseType { get; }
        public GetCreateExpenseModelMessage(string targetExpenseType)
        {
            TargetExpenseType = Enum.Parse<ExpenditureTypeEnum>(targetExpenseType);
        }
    }

    public interface IGetCreateExpenseModel
    {
        public Task<IProcessResult<ExpenseViewModel>> Process(GetCreateExpenseModelMessage msg);

    }
    public class GetCreateExpenseModel : Orchastrator<GetCreateExpenseModelMessage, ExpenseViewModel>,
        IGetCreateExpenseModel, IAutoRegisterAsSingleton
    {
        private readonly IEmployeeRepository empRepo;
        private readonly IJobsRepository jobsRepository;

        public GetCreateExpenseModel(IEmployeeRepository empRepo,
            IJobsRepository jobsRepository)
        {
            this.empRepo = empRepo;
            this.jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<ExpenseViewModel>> Handle(GetCreateExpenseModelMessage msg)
        {
            var emps = (await empRepo.GetAllEmployees()).Where(x => x.EmployeeId != 1).ToList();
            var jobs = await jobsRepository.GetAsync();

            var mdl = msg.TargetExpenseType switch
            {
                ExpenditureTypeEnum.ArcFlashLabelExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    ArcFlashLabelExpenditure = new Expense<api.expenditures.Models.ArcFlashLabelExpenditure>()
                    {
                        Detail = new api.expenditures.Models.ArcFlashLabelExpenditure()
                        {
                            DateOfInvoice = DateTimeOffset.UtcNow,
                        }

                    }
                },
                ExpenditureTypeEnum.MiscExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs =  jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    MiscExpenditure = new Expense<api.expenditures.Models.MiscExpenditure>()
                    {
                        Detail = new api.expenditures.Models.MiscExpenditure()
                        {
                            ExpensedOn = DateTimeOffset.UtcNow,

                        }

                    }
                },
                ExpenditureTypeEnum.ContractorExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs =  jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    ContractorExpenditure = new Expense<api.expenditures.Models.ContractorExpenditure>()
                    {
                        Detail = new api.expenditures.Models.ContractorExpenditure()
                        {
                            ExpensedOn = DateTimeOffset.UtcNow
                        }
                    }
                },
                ExpenditureTypeEnum.TimeAndExpenceExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs =  jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    TimeAndExpenceExpenditure = new Expense<api.expenditures.Models.TimeAndExpenceExpenditure>()
                    {
                        Detail = new api.expenditures.Models.TimeAndExpenceExpenditure()
                        {
                            ExpenseOnDate = DateTimeOffset.UtcNow
                        }
                    }
                },
                ExpenditureTypeEnum.CompanyVehicleExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs =  jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    CompanyVehicleExpenditure = new Expense<api.expenditures.Models.CompanyVehicleExpenditure>()
                    {
                        Detail = new api.expenditures.Models.CompanyVehicleExpenditure()
                        {
                            DateVehicleFirstUsed = DateTimeOffset.UtcNow,
                            LastModified = DateTimeOffset.UtcNow,
                            Vehicle = api.expenditures.Models.CompanyVehicleDescriptor.Enclave
                        }
                    }
                },
                _ => throw new NotImplementedException()
            };

            return Success(mdl);
        }
    }
}

