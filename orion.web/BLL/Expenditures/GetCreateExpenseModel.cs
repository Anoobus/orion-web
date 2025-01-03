using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Orion.Web.BLL.Core;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.UI.Models;
using Orion.Web.Util.IoC;

namespace Orion.Web.BLL.Expenditures
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

        public GetCreateExpenseModel(
            IEmployeeRepository empRepo,
            IJobsRepository jobsRepository)
        {
            this.empRepo = empRepo;
            this.jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<ExpenseViewModel>> Handle(GetCreateExpenseModelMessage msg)
        {
            var emps = (await empRepo.GetAllEmployees())
            .Where(x => x.EmployeeId != 1 && x.Role != "Disabled")
            .OrderBy(x => x.Last).ToList();

            var jobs = await jobsRepository.GetAsync();

            var mdl = msg.TargetExpenseType switch
            {
                ExpenditureTypeEnum.ArcFlashLabelExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    ArcFlashLabelExpenditure = new Expense<Api.Expenditures.Models.ArcFlashLabelExpenditure>()
                    {
                        Detail = new Api.Expenditures.Models.ArcFlashLabelExpenditure()
                        {
                            DateOfInvoice = DateTimeOffset.UtcNow,
                        }
                    }
                },
                ExpenditureTypeEnum.MiscExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    MiscExpenditure = new Expense<Api.Expenditures.Models.MiscExpenditure>()
                    {
                        Detail = new Api.Expenditures.Models.MiscExpenditure()
                        {
                            ExpensedOn = DateTimeOffset.UtcNow,
                        }
                    }
                },
                ExpenditureTypeEnum.ContractorExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    ContractorExpenditure = new Expense<Api.Expenditures.Models.ContractorExpenditure>()
                    {
                        Detail = new Api.Expenditures.Models.ContractorExpenditure()
                        {
                            ExpensedOn = DateTimeOffset.UtcNow
                        }
                    }
                },
                ExpenditureTypeEnum.TimeAndExpenceExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    TimeAndExpenceExpenditure = new Expense<Api.Expenditures.Models.TimeAndExpenceExpenditure>()
                    {
                        Detail = new Api.Expenditures.Models.TimeAndExpenceExpenditure()
                        {
                            ExpenseOnDate = DateTimeOffset.UtcNow
                        }
                    }
                },
                ExpenditureTypeEnum.CompanyVehicleExpenditure => new ExpenseViewModel()
                {
                    AvailableEmployees = emps,
                    AvailableJobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToArray(),
                    ExpenseType = msg.TargetExpenseType,
                    IsBrandNewExpenditureCreation = true,
                    CompanyVehicleExpenditure = new Expense<Api.Expenditures.Models.CompanyVehicleExpenditure>()
                    {
                        Detail = new Api.Expenditures.Models.CompanyVehicleExpenditure()
                        {
                            DateVehicleFirstUsed = DateTimeOffset.UtcNow,
                            LastModified = DateTimeOffset.UtcNow,
                            Vehicle = Api.Expenditures.Models.CompanyVehicleDescriptor.ChevyBlazer
                        }
                    }
                },
                _ => throw new NotImplementedException()
            };

            return Success(mdl);
        }
    }
}
