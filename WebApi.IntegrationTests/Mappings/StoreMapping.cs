using AutoMapper;
using WebApi.IntegrationTests.Infrastructure.Entities;
using WebApi.IntegrationTests.Models;

namespace WebApi.IntegrationTests.Mappings
{
    public class StoreMapping : Profile
    {
        public StoreMapping() 
        {
            CreateMap<Store, StoreDto>();
            CreateMap<StoreDto, Store>();
        }
    }
}
