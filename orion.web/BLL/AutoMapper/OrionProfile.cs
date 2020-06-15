﻿using AutoMapper;
using orion.web.Clients;
using orion.web.Jobs;

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
        }
    }
}