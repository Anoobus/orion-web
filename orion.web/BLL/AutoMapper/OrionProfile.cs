﻿using AutoMapper;
using Orion.Web.Api;
using Orion.Web.BLL.Expenditures;
using Orion.Web.BLL.Jobs;
using Orion.Web.BLL.ScheduledTasks;
using Orion.Web.Clients;
using Orion.Web.Common;
using Orion.Web.DataAccess.EF;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.UI.Models;

namespace Orion.Web.BLL.AutoMapper
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

            // api model <=> ef model
            CreateMap<UpdateMessage<UpdateArcFlashLabelExpenditureMessage, DataAccess.EF.ArcFlashLabelExpenditure>, DataAccess.EF.ArcFlashLabelExpenditure>()
                .ForMember(x => x.DateOfInvoice, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.NewValue.Model.DateOfInvoice.Year, x.NewValue.Model.DateOfInvoice.Month, x.NewValue.Model.DateOfInvoice.Day)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Quantity, opt => opt.MapFrom(x => x.NewValue.Model.Quantity))
                .ForMember(x => x.TotalLabelsCost, opt => opt.MapFrom(x => x.NewValue.Model.TotalLabelsCost))
                .ForMember(x => x.TotalPostageCost, opt => opt.MapFrom(x => x.NewValue.Model.TotalPostageCost))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.DateOfInvoice).WeekId.Value));

            CreateMap<UpdateMessage<UpdateMiscExpenditureMessage, DataAccess.EF.MiscExpenditure>, DataAccess.EF.MiscExpenditure>()
                .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.NewValue.Model.ExpensedOn.Year, x.NewValue.Model.ExpensedOn.Month, x.NewValue.Model.ExpensedOn.Day)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.NewValue.Model.Amount))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.NewValue.Model.Description))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpensedOn).WeekId.Value));

            CreateMap<UpdateMessage<UpdateContractorMessage, DataAccess.EF.ContractorExpenditure>, DataAccess.EF.ContractorExpenditure>()
                .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.NewValue.Model.ExpensedOn.Year, x.NewValue.Model.ExpensedOn.Month, x.NewValue.Model.ExpensedOn.Day)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.TotalPOContractAmount, opt => opt.MapFrom(x => x.NewValue.Model.TotalPOContractAmount))
                .ForMember(x => x.OrionPONumber, opt => opt.MapFrom(x => x.NewValue.Model.OrionPONumber))
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.NewValue.Model.CompanyName))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpensedOn).WeekId.Value));

            CreateMap<UpdateMessage<UpdateTimeAndExpenseExpenditureMessage, DataAccess.EF.TimeAndExpenceExpenditure>, DataAccess.EF.TimeAndExpenceExpenditure>()
                .ForMember(x => x.ExpenseOnDate, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.NewValue.Model.ExpenseOnDate.Year, x.NewValue.Model.ExpenseOnDate.Month, x.NewValue.Model.ExpenseOnDate.Day)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId))
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Existing.ExternalId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Existing.Id))
                .ForMember(x => x.Job, opt => opt.Ignore())
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.NewValue.Model.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.EasternStandardTimeOffset))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.NewValue.Model.Amount))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => WeekDTO.CreateWithWeekContaining(x.NewValue.Model.ExpenseOnDate).WeekId.Value));

            CreateMap<UpdateMessage<UpdateCompanyVehicleExpenditureMessage, DataAccess.EF.CompanyVehicleExpenditure>, DataAccess.EF.CompanyVehicleExpenditure>()
               .ForMember(x => x.DateVehicleFirstUsed, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.NewValue.Model.DateVehicleFirstUsed.Year, x.NewValue.Model.DateVehicleFirstUsed.Month, x.NewValue.Model.DateVehicleFirstUsed.Day)))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.NewValue.Model.EmployeeId))
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

            CreateMap<DataAccess.EF.ArcFlashLabelExpenditure, Orion.Web.Api.Expenditures.Models.ArcFlashLabelExpenditure>()
                .ForMember(x => x.DateOfInvoice, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.DateOfInvoice)))
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
                .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
                .ForMember(x => x.LastModified, opt => opt.MapFrom(x => x.LastModified))
                .ForMember(x => x.Quantity, opt => opt.MapFrom(x => x.Quantity))
                .ForMember(x => x.TotalLabelsCost, opt => opt.MapFrom(x => x.TotalLabelsCost))
                .ForMember(x => x.TotalPostageCost, opt => opt.MapFrom(x => x.TotalPostageCost))
                .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

            CreateMap<DataAccess.EF.CompanyVehicleExpenditure, Orion.Web.Api.Expenditures.Models.CompanyVehicleExpenditure>()
               .ForMember(x => x.DateVehicleFirstUsed, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.DateVehicleFirstUsed)))
               .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
               .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.ExternalId))
               .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
               .ForMember(x => x.LastModified, opt => opt.MapFrom(x => x.LastModified))
               .ForMember(x => x.TotalMiles, opt => opt.MapFrom(x => x.TotalMiles))
               .ForMember(x => x.TotalNumberOfDaysUsed, opt => opt.MapFrom(x => x.TotalNumberOfDaysUsed))
               .ForMember(x => x.Vehicle, opt => opt.MapFrom(x => (Orion.Web.Api.Expenditures.Models.CompanyVehicleDescriptor)x.CompanyVehicleId))
               .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

            CreateMap<DataAccess.EF.ContractorExpenditure, Orion.Web.Api.Expenditures.Models.ContractorExpenditure>()
              .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.CompanyName))
              .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
              .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.ExternalId))
              .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
              .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.LastModified)))
              .ForMember(x => x.OrionPONumber, opt => opt.MapFrom(x => x.OrionPONumber))
              .ForMember(x => x.TotalPOContractAmount, opt => opt.MapFrom(x => x.TotalPOContractAmount))
              .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

            CreateMap<DataAccess.EF.MiscExpenditure, Orion.Web.Api.Expenditures.Models.MiscExpenditure>()
             .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
             .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
             .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amount))
             .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
             .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.LastModified)))
             .ForMember(x => x.Description, opt => opt.MapFrom(x => x.Description))
             .ForMember(x => x.ExpensedOn, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.ExpensedOn)))
             .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));

            CreateMap<DataAccess.EF.TimeAndExpenceExpenditure, Orion.Web.Api.Expenditures.Models.TimeAndExpenceExpenditure>()
             .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ExternalId))
             .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
             .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amount))
             .ForMember(x => x.JobId, opt => opt.MapFrom(x => x.JobId))
             .ForMember(x => x.LastModified, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.LastModified)))
             .ForMember(x => x.ExpenseOnDate, opt => opt.MapFrom(x => DateTimeWithZone.ConvertToEST(x.ExpenseOnDate)))
             .ForMember(x => x.WeekId, opt => opt.MapFrom(x => x.WeekId));
        }
    }
}
