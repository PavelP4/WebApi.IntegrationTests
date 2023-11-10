using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using WebApi.IntegrationTests.Attributes;
using WebApi.IntegrationTests.Enums;
using WebApi.IntegrationTests.Models;
using WebApi.IntegrationTests.Services;

namespace WebApi.IntegrationTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService) 
        { 
            _orderService = orderService;
        }

        [HttpGet]
        [AppAuthorize]
        public async Task<IEnumerable<OrderDto>> GetOrders() 
        {
            return await _orderService.GetOrders();
        }

        [HttpPost]
        [AppAuthorize(AppRoles.Admin, AppRoles.AdvancedUser)]
        public async Task<OrderDto> AddOrder(OrderDto newOrder)
        {
            return await _orderService.AddOrder(newOrder);
        }

        [HttpPut]
        [AppAuthorize(AppRoles.Admin, AppRoles.AdvancedUser)]
        public async Task<OrderDto> UpdateOrder(OrderDto updatedOrder)
        {
            return await _orderService.UpdateOrder(updatedOrder);
        }

        [HttpDelete]
        [Route("{id}")]
        [AppAuthorize(AppRoles.Admin)]
        public async Task DeleteOrder(Guid id)
        {
            await _orderService.DeleteOrder(id);
        }
    }
}
