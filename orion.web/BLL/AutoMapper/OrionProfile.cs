using AutoMapper;
using orion.web.Clients;
using orion.web.Jobs;
using orion.web.UI.Models;

namespace orion.web.BLL.AutoMapper
{
    public class OrionProfile : Profile
    {
        public OrionProfile()
        {
            CreateMap<ClientDTO, DataAccess.EF.Client>().ReverseMap();
            CreateMap<JobDTO, JobModel>().ReverseMap();
            CreateMap<ClientModel, ClientDTO>().ReverseMap();
            CreateMap<SiteModel, SiteDTO>().ReverseMap();
            CreateMap<JobStatusModel, JobStatusDTO>().ReverseMap();
            CreateMap<ProjectManagerModel, ProjectManagerDTO>().ReverseMap();
            CreateMap<DataAccess.EF.Site, SiteDTO>().ReverseMap();
        }
    }
}
