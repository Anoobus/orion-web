using AutoMapper;
using orion.web.BLL.Jobs;
using orion.web.Clients;
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
                .ForMember(x => x.Site, opt => opt.Ignore());
            CreateMap<UpdateJobDto, DataAccess.EF.Job>()
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.ProjectManagerEmployeeId))
                .ForMember(x => x.JobStatusId, opt => opt.MapFrom(x => (int)x.JobStatusId))
                .ForMember(x => x.Client, opt => opt.Ignore())
                .ForMember(x => x.JobStatus, opt => opt.Ignore())
                .ForMember(x => x.ProjectManager, opt => opt.Ignore())
                .ForMember(x => x.Site, opt => opt.Ignore());
            CreateMap<DataAccess.EF.Job, JobDTO>()
                .ForMember(x => x.ProjectManagerEmployeeId, opt => opt.MapFrom(z => z.EmployeeId))
                .ForMember(x => x.FullJobCodeWithName, opt => opt.Ignore());

            CreateMap<EmployeeDTO, ProjectManagerModel>()
                .ForMember(x => x.EmployeeId, opt => opt.MapFrom(x => x.EmployeeId))
                .ForMember(x => x.EmployeeName, opt => opt.MapFrom(x => $"{x.Last}, {x.First}"));
        }
    }
}
