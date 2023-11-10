using AutoMapper;
using WebApi.IntegrationTests.Infrastructure.Entities;
using WebApi.IntegrationTests.Models;
using WebApi.IntegrationTests.Repositories;

namespace WebApi.IntegrationTests.Services
{
    public class OrderService
    {
        private readonly BaseRepository<Order> _repository;
        private readonly IMapper _mapper;

        public OrderService(
            BaseRepository<Order> repository,
            IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetOrders() 
        {
            var dbOrders = await _repository.GetAll();
            return _mapper.Map<IEnumerable<OrderDto>>(dbOrders);
        }

        public async Task<OrderDto> AddOrder(OrderDto newOrder)
        { 
            var dbOrder = _mapper.Map<Order>(newOrder);

            dbOrder = await _repository.Add(dbOrder);
            await _repository.SaveChanges();

            return _mapper.Map<OrderDto>(dbOrder);
        }

        public async Task<OrderDto> UpdateOrder(OrderDto updatedOrder)
        {
            var dbEntity = await _repository.GetById(updatedOrder.Id);

            _mapper.Map(updatedOrder, dbEntity);
            await _repository.SaveChanges();

            return _mapper.Map<OrderDto>(dbEntity);
        }

        public async Task DeleteOrder(Guid id)
        { 
            await _repository.Delete(id);
            await _repository.SaveChanges();
        }
    }
}
