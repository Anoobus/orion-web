using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUglify.Html;
using Orion.Web.Api;
using Orion.Web.Api.Expenditures.Models;
using Orion.Web.BLL.Core;
using Orion.Web.Common;
using Orion.Web.DataAccess;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.UI.Models;
using Orion.Web.Util.IoC;

namespace Orion.Web.BLL.ArcFlashExpenditureExpenses
{
    public class GetAllExpendituresRequest : IProduces<AllExpendituresModel>
    {
        public GetAllExpendituresRequest(
            bool includeArcFlashlabels = true,
            bool includeCompanyVehicles = true,
            bool includeMiscExpenditure = true,
            bool includeContractorExpenditures = true,
            bool includeTimeAndExpenseExpenditures = true,
            Guid? limitToSingleExpense = null)
        {
            IncludeArcFlashlabels = includeArcFlashlabels;
            IncludeCompanyVehicles = includeCompanyVehicles;
            IncludeMiscExpenditure = includeMiscExpenditure;
            IncludeContractorExpenditures = includeContractorExpenditures;
            IncludeTimeAndExpenseExpenditures = includeTimeAndExpenseExpenditures;
            LimitToSingleExpense = limitToSingleExpense;
        }

        public bool IncludeArcFlashlabels { get; }
        public bool IncludeCompanyVehicles { get; }
        public bool IncludeMiscExpenditure { get; }
        public bool IncludeContractorExpenditures { get; }
        public bool IncludeTimeAndExpenseExpenditures { get; }
        public Guid? LimitToSingleExpense { get; }
    }

    public interface IGetListOfExpenditures
    {
        public Task<IProcessResult<AllExpendituresModel>> Process(GetAllExpendituresRequest msg);
    }

    public class GetListOfExpenditures
        : Orchastrator<GetAllExpendituresRequest, AllExpendituresModel>,
        IGetListOfExpenditures, IAutoRegisterAsSingleton

    {
        private readonly IArcFlashLabelExpenditureRepo arcFlashLabelsRepo;
        private readonly ICompanyVehicleExpenditureRepo companyVehicleRepo;
        private readonly IMiscExpenditureRepo miscRepo;
        private readonly IContractorExpenditureRepo contractorExpRepo;
        private readonly ITimeAndExpenceExpenditureRepo timeAndExpenseExpRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly IJobsRepository jobsRepository;
        private readonly IMapper _mapper;

        public GetListOfExpenditures(
            IArcFlashLabelExpenditureRepo arcFlashLabelsRepo,
            ICompanyVehicleExpenditureRepo companyVehicleRepo,
            IMiscExpenditureRepo miscRepo,
            IContractorExpenditureRepo contractorExpRepo,
            ITimeAndExpenceExpenditureRepo timeAndExpenseExpRepo,
            IEmployeeRepository empRepo,
            IJobsRepository jobsRepository,
            IMapper mapper)
        {
            this.arcFlashLabelsRepo = arcFlashLabelsRepo;
            this.companyVehicleRepo = companyVehicleRepo;
            this.miscRepo = miscRepo;
            this.contractorExpRepo = contractorExpRepo;
            this.timeAndExpenseExpRepo = timeAndExpenseExpRepo;
            this.empRepo = empRepo;
            this.jobsRepository = jobsRepository;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<AllExpendituresModel>> Handle(GetAllExpendituresRequest msg)
        {
            var employeeDetails = (await empRepo.GetAllEmployees())
                .Where(x => x.EmployeeId != 1) // exclude Admin here
                .ToDictionary(x => x.EmployeeId);
            var jobs = (await jobsRepository.GetAsync()).ToDictionary(x => x.JobId);
            var result = new AllExpendituresModel();

            if (msg.IncludeArcFlashlabels)
            {
                var temp = await arcFlashLabelsRepo.SearchForEntity(x => msg.LimitToSingleExpense.HasValue ?
                                                                                x.ExternalId == msg.LimitToSingleExpense.Value :
                                                                                true);
                LoadSection(
                    temp,
                    (ArcFlashLabelExpenditure x) =>
                    {
                        var hmm = new Expense<ArcFlashLabelExpenditure>()
                        {
                            Detail = x,
                            Amount = x.TotalLabelsCost + x.TotalPostageCost,
                            EmployeeName = $"{employeeDetails[x.EmployeeId].Last}, {employeeDetails[x.EmployeeId].First}",
                            FullJobNameWithCode = jobs[x.JobId].FullJobCodeWithName,
                            Id = x.Id,
                            ShortExpenseName = $"Arc Flash Label(s): ({DateTimeWithZone.ConvertToEST(x.DateOfInvoice.UtcDateTime).ToShortDateString()})",
                            JobId = x.JobId,
                            LastModifiedDateEst = DateTimeWithZone.ConvertToEST(x.LastModified.UtcDateTime),
                            ExpensedOnDateEst = DateTimeWithZone.ConvertToEST(x.DateOfInvoice.UtcDateTime)
                        };
                        return hmm;
                    },
                    mapped => result.ArcFlashLabelExpenditures = mapped);
            }

            if (msg.IncludeCompanyVehicles)
            {
                LoadSection(
                    await companyVehicleRepo.SearchForEntity(x => msg.LimitToSingleExpense.HasValue ?
                                                                                x.ExternalId == msg.LimitToSingleExpense.Value :
                                                                                true),
                    (CompanyVehicleExpenditure x) => new Expense<CompanyVehicleExpenditure>()
                    {
                        Detail = x,
                        Amount = x.GetTotalCost(),
                        EmployeeName = $"{employeeDetails[x.EmployeeId].Last}, {employeeDetails[x.EmployeeId].First}",
                        FullJobNameWithCode = jobs[x.JobId].FullJobCodeWithName,
                        Id = x.ExternalId,
                        ShortExpenseName = $"Company Vehicle: {ToHumanReadable(x.Vehicle)} ({x.TotalMiles} miles)",
                        JobId = x.JobId,
                        LastModifiedDateEst = DateTimeWithZone.ConvertToEST(x.LastModified.UtcDateTime),
                        ExpensedOnDateEst = DateTimeWithZone.ConvertToEST(x.DateVehicleFirstUsed.UtcDateTime)
                    },
                    mapped => result.CompanyVehicleExpenditures = mapped);
            }

            if (msg.IncludeContractorExpenditures)
            {
                LoadSection(
                    await contractorExpRepo.SearchForEntity(x => msg.LimitToSingleExpense.HasValue ?
                                                                                x.ExternalId == msg.LimitToSingleExpense.Value :
                                                                                true),
                    (ContractorExpenditure x) => new Expense<ContractorExpenditure>()
                    {
                        Detail = x,
                        Amount = x.TotalPOContractAmount,
                        EmployeeName = $"{employeeDetails[x.EmployeeId].Last}, {employeeDetails[x.EmployeeId].First}",
                        FullJobNameWithCode = jobs[x.JobId].FullJobCodeWithName,
                        Id = x.ExternalId,
                        ShortExpenseName = $"Contrator: {x.CompanyName} (PO: {x.OrionPONumber})",
                        JobId = x.JobId,
                        LastModifiedDateEst = DateTimeWithZone.ConvertToEST(x.LastModified.UtcDateTime),
                        ExpensedOnDateEst = DateTimeWithZone.ConvertToEST(x.ExpensedOn.UtcDateTime)
                    },
                    mapped => result.ContractorExpenditures = mapped);
            }

            if (msg.IncludeMiscExpenditure)
            {
                LoadSection(
                    await miscRepo.SearchForEntity(x => msg.LimitToSingleExpense.HasValue ?
                                                                                x.ExternalId == msg.LimitToSingleExpense.Value :
                                                                                true),
                    (MiscExpenditure x) => new Expense<MiscExpenditure>()
                    {
                        Detail = x,
                        Amount = x.Amount,
                        EmployeeName = $"{employeeDetails[x.EmployeeId].Last}, {employeeDetails[x.EmployeeId].First}",
                        FullJobNameWithCode = jobs[x.JobId].FullJobCodeWithName,
                        Id = x.Id,
                        ShortExpenseName = $"Misc: {x.Description}",
                        JobId = x.JobId,
                        LastModifiedDateEst = DateTimeWithZone.ConvertToEST(x.LastModified.UtcDateTime),
                        ExpensedOnDateEst = DateTimeWithZone.ConvertToEST(x.ExpensedOn.UtcDateTime)
                    },
                    mapped => result.MiscExpenditures = mapped);
            }

            if (msg.IncludeTimeAndExpenseExpenditures)
            {
                LoadSection(
                    await timeAndExpenseExpRepo.SearchForEntity(x => msg.LimitToSingleExpense.HasValue ?
                                                                                x.ExternalId == msg.LimitToSingleExpense.Value :
                                                                                true),
                    (TimeAndExpenceExpenditure x) => new Expense<TimeAndExpenceExpenditure>()
                    {
                        Detail = x,
                        Amount = x.Amount,
                        EmployeeName = $"{employeeDetails[x.EmployeeId].Last}, {employeeDetails[x.EmployeeId].First}",
                        FullJobNameWithCode = jobs[x.JobId].FullJobCodeWithName,
                        Id = x.Id,
                        ShortExpenseName = $"T&E: ({DateTimeWithZone.ConvertToEST(x.ExpenseOnDate.UtcDateTime).ToShortDateString()})",
                        JobId = x.JobId,
                        LastModifiedDateEst = DateTimeWithZone.ConvertToEST(x.LastModified.UtcDateTime),
                        ExpensedOnDateEst = DateTimeWithZone.ConvertToEST(x.ExpenseOnDate.UtcDateTime)
                    },
                    mapped => result.TimeAndExpenceExpenditures = mapped);
            }

            result.AvailableEmployees = employeeDetails.Select(e => new CoreEmployeeDto()
            {
                EmployeeId = e.Key,
                First = e.Value.First,
                Last = e.Value.Last
            }).ToList();

            var inUseJobs = result.AsFullList().Select(x => x.JobId).ToHashSet();
            result.AvailableJobs = jobs.Values.Where(x => x.JobStatusId == JobStatus.Enabled || inUseJobs.Contains(x.JobId)).ToList();

            return Success(result);
        }

        private string ToHumanReadable(CompanyVehicleDescriptor vehicle)
        {
            if (vehicle == CompanyVehicleDescriptor.Unknown)
                return vehicle.ToString();

            var temp = vehicle.ToString();
            var final = string.Empty;
            foreach (var character in temp)
            {
                if (character.IsAlphaUpper() || final.Length <= 1)
                {
                    final += character;
                }
                else
                {
                    var eow = final.Last();
                    if (eow.IsAlphaUpper())
                    {
                        final = final.Substring(0, final.Length - 1) + " " + eow + character;
                    }
                    else
                    {
                        final += character;
                    }
                }
            }
            return $"2024 {final}";
        }
        private void LoadSection<TResult, TEFModel>(
            List<TEFModel> inDb,
            Func<TResult, Expense<TResult>> map,
            Action<List<Expense<TResult>>> onMapped)
        {
            if (inDb == null || !inDb.Any())
            {
                onMapped(new List<Expense<TResult>>());
            }
            else
            {
                var mappedModel = inDb.Select(x => _mapper.Map<TResult>(x)).ToList();
                onMapped(mappedModel
                            .Select(map)
                            .ToList());
            }
        }
    }
}
