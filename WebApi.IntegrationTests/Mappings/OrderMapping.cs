using AutoMapper;
using WebApi.IntegrationTests.Extentions;
using WebApi.IntegrationTests.Infrastructure.Entities;
using WebApi.IntegrationTests.Models;

namespace WebApi.IntegrationTests.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping() 
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom((s, d, dm, context) =>
                {
                    return context.MapEntities(s.OrderItems, d.OrderItems, sItem => sItem.Id, dItem => dItem.Id);
                }))
                ;

            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItemDto, OrderItem>();
                //.ForMember(dest => dest.Order, opt => opt.Ignore())
                //.ForMember(dest => dest.OrderId, opt => opt.Ignore());
        }
    }
}
