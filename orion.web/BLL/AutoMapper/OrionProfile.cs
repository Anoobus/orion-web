using System;
using AutoMapper;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.Expenditures;
using orion.web.BLL.Jobs;
using orion.web.BLL.ScheduledTasks;
using orion.web.Clients;
using orion.web.Common;
using orion.web.DataAccess.EF;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.UI.Models;

namespace orion.web.BLL.AutoMapper
{
    public class OrionProfile : Profile
    {
        public OrionProfile()
        {
            CreateMap<ClientDTO, DataAccess.EF.Client>().ReverseMap();
            CreateMap<ClientModel, ClientDTO>().ReverseMap();
            CreateMap<SiteModel, SiteDTO>().ReverseMap();
            CreateMap<JobStatusModel, JobStatusDTO>().ReverseMap();
            CreateMap<ProjectManagerModel, ProjectManagerDTO>().ReverseMap();
            CreateMap<DataAccess.EF.Site, SiteDTO>().ReverseMap();
            CreateMap<CreateJobDto, DataAccess.EF.Job>()
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.ProjectManagerEmployeeId))
                .ForMember(x => x.JobStatusId, opt => opt.MapFrom(x => (int)x.JobStatusId))
                .ForMember(x => x.Client, opt => opt.Ignore())
                .ForMember(x => x.JobStatus, opt => opt.Ignore())
                .ForMember(x => x.ProjectManager, opt => opt.Ignore())
                .ForMember(x => x.Site, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.Ignore());

            CreateMap<UpdateJobDto, DataAccess.EF.Job>()
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.ProjectManagerEmployeeId))
                .ForMember(x => x.JobStatusId, opt => opt.MapFrom(x => (int)x.JobStatusId))
                .ForMember(x => x.Client, opt => opt.Ignore())
                .ForMember(x => x.JobStatus, opt => opt.Ignore())
                .ForMember(x => x.ProjectManager, opt => opt.Ignore())
                .ForMember(x => x.Site, opt => opt.Ignore());
            CreateMap<DataAccess.EF.Job, CoreJobDto>()
                .ForMember(x => x.ProjectManagerEmployeeId, opt => opt.MapFrom(z => z.EmployeeId))
                .ForMember(x => x.FullJobCodeWithName, opt => opt.Ignore());

            CreateMap<EmployeeDTO, ProjectManagerModel>()
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
                .ForMember(x => x.EmployeeName, opt => opt.MapFrom(x => $"{x.Last}, {x.First}"));

            CreateMap<NewScheduledTask, ScheduleTask>()
                .ForMember(x => x.ScheduleTaskId, opt => opt.Ignore());


            //api model <=> ef model           
            CreateMap<UpdateMessage<UpdateArcFlashLabelExpenditureMessage, DataAccess.EF.ArcFlashLabelExpenditure>,DataAccess.EF.ArcFlashLabelExpenditure>()
                .ForMember(x => x.DateOfInvoice, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.NewValue.model.DateOfInvoice)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.model.EmployeeId ))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Quantity, opt => opt.MapFrom(x => x.NewValue.model.Quantity))
                .ForMember(x => x.TotalLabelsCost, opt => opt.MapFrom(x => x.NewValue.model.TotalLabelsCost))
                .ForMember(x => x.TotalPostageCost, opt => opt.MapFrom(x => x.NewValue.model.TotalPostageCost))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.model.DateOfInvoice).WeekId.Value));

            CreateMap<UpdateMessage<UpdateMiscExpenditureMessage, DataAccess.EF.MiscExpenditure>,DataAccess.EF.MiscExpenditure>()
                .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.NewValue.Model.ExpensedOn)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId ))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.NewValue.Model.Amount))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.NewValue.Model.Description))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpensedOn).WeekId.Value));

            CreateMap<UpdateMessage<UpdateContractorMessage, DataAccess.EF.ContractorExpenditure>,DataAccess.EF.ContractorExpenditure>()
                .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.NewValue.Model.ExpensedOn)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId ))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.TotalPOContractAmount, opt => opt.MapFrom(x => x.NewValue.Model.TotalPOContractAmount))
                .ForMember(x => x.OrionPONumber, opt => opt.MapFrom(x => x.NewValue.Model.OrionPONumber))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpensedOn).WeekId.Value));

            CreateMap<UpdateMessage<UpdateTimeAndExpenseExpenditureMessage, DataAccess.EF.TimeAndExpenceExpenditure>,DataAccess.EF.TimeAndExpenceExpenditure>()
                .ForMember(x => x.ExpenseOnDate, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.NewValue.Model.ExpenseOnDate)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId ))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.NewValue.Model.Amount))                
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpenseOnDate).WeekId.Value));

             CreateMap<UpdateMessage<UpdateCompanyVehicleExpenditureMessage, DataAccess.EF.CompanyVehicleExpenditure>,DataAccess.EF.CompanyVehicleExpenditure>()
                .ForMember(x => x.DateVehicleFirstUsed, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.NewValue.Model.DateVehicleFirstUsed)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId ))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.CompanyVehicle, opt => opt.Ignore())
                .ForMember(x => x.CompanyVehicleId, opt => opt.MapFrom(x => (int)x.NewValue.Model.Vehicle))
                .ForMember(x => x.TotalMiles, opt => opt.MapFrom(x => x.NewValue.Model.TotalMiles))
                .ForMember(x => x.TotalNumberOfDaysUsed, opt => opt.MapFrom(x => x.NewValue.Model.TotalNumberOfDaysUsed))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.Existing.WeekId));


            CreateMap<DataAccess.EF.ArcFlashLabelExpenditure, orion.web.api.expenditures.Models.ArcFlashLabelExpenditure>()
                .ForMember(x => x.DateOfInvoice, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.DateOfInvoice)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => x.LastModified))
                .ForMember(x => x.Quantity, opt => opt.MapFrom(x => x.Quantity))
                .ForMember(x => x.TotalLabelsCost, opt => opt.MapFrom(x => x.TotalLabelsCost))
                .ForMember(x => x.TotalPostageCost, opt => opt.MapFrom(x => x.TotalPostageCost))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

            CreateMap<DataAccess.EF.CompanyVehicleExpenditure, orion.web.api.expenditures.Models.CompanyVehicleExpenditure>()
               .ForMember(x => x.DateVehicleFirstUsed, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.DateVehicleFirstUsed)))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
               .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.ExternalId))
               .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
               .ForMember(x => x.LastModified, opt => opt.MapFrom(x => x.LastModified))
               .ForMember(x => x.TotalMiles, opt => opt.MapFrom(x => x.TotalMiles))
               .ForMember(x => x.TotalNumberOfDaysUsed, opt => opt.MapFrom(x => x.TotalNumberOfDaysUsed))
               .ForMember(x => x.Vehicle, opt => opt.MapFrom(x => (CompanyVehicleDescriptor)x.CompanyVehicleId))
               .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

             CreateMap<DataAccess.EF.ContractorExpenditure, orion.web.api.expenditures.Models.ContractorExpenditure>()
               .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.CompanyName))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
               .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.ExternalId))
               .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
               .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.LastModified)))
               .ForMember(x => x.OrionPONumber, opt => opt.MapFrom(x => x.OrionPONumber))
               .ForMember(x => x.TotalPOContractAmount, opt => opt.MapFrom(x => x.TotalPOContractAmount))
               .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

              CreateMap<DataAccess.EF.MiscExpenditure, orion.web.api.expenditures.Models.MiscExpenditure>()
               .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
               .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amount))
               .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
               .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.LastModified)))
               .ForMember(x => x.Description, opt => opt.MapFrom(x => x.Description))
               .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.ExpensedOn)))
               .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

              CreateMap<DataAccess.EF.TimeAndExpenceExpenditure, orion.web.api.expenditures.Models.TimeAndExpenceExpenditure>()
               .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
               .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amount))
               .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
               .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.LastModified)))
               .ForMember(x => x.ExpenseOnDate, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST( x.ExpenseOnDate)))
               .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

        }
    }
}
