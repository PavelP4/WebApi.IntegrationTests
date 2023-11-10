using AutoMapper;
using WebApi.IntegrationTests.Enums;
using WebApi.IntegrationTests.Extentions;
using WebApi.IntegrationTests.Infrastructure;
using WebApi.IntegrationTests.Models;
using Xunit;

namespace WebApi.IntegrationTests.IntegrationTests.Controllers
{
    [Collection(Sequential)]
    public class OrderControllerTests : BaseControllerTest<Startup, AppDbContext>, IClassFixture<WebApplicationFixture>
    {
        protected override string ControllerName => "Order";

        public OrderControllerTests(WebApplicationFixture factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllOrders()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var orders = Enumerable.Range(1,3).Select(x => ObjectsHelper.CreateOrder($"ORD-{x}", store.Id)).ToArray();
            var firstOrder = orders.First();
            firstOrder.OrderItems = Enumerable.Range(1, 4).Select(_ => ObjectsHelper.CreateOrderItem()).ToArray();

            await RunTest<IEnumerable<OrderDto>>(
                context => context.AddRangeToEmptyDb(orders),
                null,
                AppRoles.User.ToString(),
                result =>
                {
                    Assert.NotNull(result);                    
                    Assert.Equal(orders.Length, result.Count());

                    var firstOrderDb = result.FirstOrDefault(x => x.Number == firstOrder.Number);
                    Assert.NotNull(firstOrderDb);
                    Assert.NotNull(firstOrderDb.OrderItems);
                    Assert.Equal(firstOrder.OrderItems.Count(), firstOrderDb.OrderItems.Count());
                });
        }

        [Fact]
        public async Task CreateOrder()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var order = ObjectsHelper.CreateOrder($"ORD-Create-001", store.Id);
            var order2 = ObjectsHelper.CreateOrder($"ORD-Create-002", store.Id);
            order.OrderItems = Enumerable.Range(1, 2).Select(_ => ObjectsHelper.CreateOrderItem()).ToArray();

            var mapper = Services.GetRequiredService<IMapper>();
            var orderDto = mapper.Map<OrderDto>(order);
            await RunTestPost(
                context => context.AddRange(store, order2),
                null,
                $"{AppRoles.Admin},{AppRoles.AdvancedUser}",
                orderDto,
                context =>
                {
                    var dbOrder = context.Orders.FirstOrDefault(x => x.Number == order.Number);
                    Assert.NotNull(dbOrder);

                    Assert.Equal(order.Customer, dbOrder.Customer);
                    Assert.Equal(order.Total, dbOrder.Total);
                    Assert.Equal(store.Id, dbOrder.StoreId);
                    Assert.True(order.OrderItems.OrderBy(x => x.Name).Select(x => x.Name)
                        .SequenceEqual(dbOrder.OrderItems.OrderBy(x => x.Name).Select(x => x.Name)));
                });
        }

        [Fact]
        public async Task UpdateOrder()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var order = ObjectsHelper.CreateOrder($"ORD-{DateTime.Now}", store.Id, Guid.NewGuid());
            var orderItem = ObjectsHelper.CreateOrderItem(Guid.NewGuid());
            order.OrderItems = new[] { orderItem };

            var mapper = Services.GetRequiredService<IMapper>();
            var orderDto = mapper.Map<OrderDto>(order);
            orderDto.Customer += "!";
            orderDto.Number += "!";
            orderDto.Total++;

            var orderItemDto = orderDto.OrderItems.Single();
            orderItemDto.Name += "!";
            orderItemDto.Price++;
            orderItemDto.Score++;

            await RunTestPut(
                context => context.AddRange(order),
                null,
                $"{AppRoles.Admin},{AppRoles.AdvancedUser}",
                orderDto,
                context =>
                {
                    var dbOrder = context.Orders.FirstOrDefault(x => x.Id == order.Id);
                    Assert.NotNull(dbOrder);

                    Assert.Equal(orderDto.Customer, dbOrder.Customer);
                    Assert.Equal(orderDto.Number, dbOrder.Number);
                    Assert.Equal(orderDto.Total, dbOrder.Total);
                    Assert.Equal(orderDto.StoreId, dbOrder.StoreId);

                    Assert.NotNull(dbOrder.OrderItems);
                    Assert.Single(dbOrder.OrderItems);
                    var dbOrderItem = dbOrder.OrderItems.Single();
                    Assert.Equal(orderItemDto.Name, dbOrderItem.Name);
                    Assert.Equal(orderItemDto.Price, dbOrderItem.Price);
                    Assert.Equal(orderItemDto.Score, dbOrderItem.Score);
                });
        }

        [Fact]
        public async Task UpdateAndDeleteOrderItem()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var order = ObjectsHelper.CreateOrder($"ORD-{DateTime.Now}", store.Id, Guid.NewGuid());
            var orderItem1 = ObjectsHelper.CreateOrderItem(Guid.NewGuid());
            var orderItem2 = ObjectsHelper.CreateOrderItem(Guid.NewGuid());
            order.OrderItems = new[] { orderItem1, orderItem2 };

            var mapper = Services.GetRequiredService<IMapper>();
            var orderDto = mapper.Map<OrderDto>(order);
            orderDto.Customer += "!";
            orderDto.Number += "!";
            orderDto.Total++;

            orderDto.OrderItems = orderDto.OrderItems.Take(1).ToArray();
            var orderItemDto = orderDto.OrderItems.First();
            orderItemDto.Name += "!";
            orderItemDto.Price++;
            orderItemDto.Score++;

            await RunTestPut(
                context => context.AddRange(order),
                null,
                $"{AppRoles.Admin},{AppRoles.AdvancedUser}",
                orderDto,
                context =>
                {
                    var dbOrder = context.Orders.FirstOrDefault(x => x.Id == order.Id);
                    Assert.NotNull(dbOrder);

                    Assert.Equal(orderDto.Customer, dbOrder.Customer);
                    Assert.Equal(orderDto.Number, dbOrder.Number);
                    Assert.Equal(orderDto.Total, dbOrder.Total);
                    Assert.Equal(orderDto.StoreId, dbOrder.StoreId);

                    Assert.NotEmpty(dbOrder.OrderItems);
                    Assert.Equal(orderDto.OrderItems.Count(), dbOrder.OrderItems.Count());
                    var dbOrderItem = dbOrder.OrderItems.First();
                    Assert.Equal(orderItemDto.Name, dbOrderItem.Name);
                    Assert.Equal(orderItemDto.Price, dbOrderItem.Price);
                    Assert.Equal(orderItemDto.Score, dbOrderItem.Score);
                });
        }

        [Fact]
        public async Task UpdateAndAddOrderItem()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var order = ObjectsHelper.CreateOrder($"ORD-{DateTime.Now}", store.Id, Guid.NewGuid());
            var orderItem1 = ObjectsHelper.CreateOrderItem(Guid.NewGuid());
            order.OrderItems = new[] { orderItem1 };

            var mapper = Services.GetRequiredService<IMapper>();
            var orderDto = mapper.Map<OrderDto>(order);

            var orderItemNew = ObjectsHelper.CreateOrderItem();
            var orderItemDto = mapper.Map<OrderItemDto>(orderItemNew);
            orderDto.OrderItems.Add(orderItemDto);

            await RunTestPut(
                context => context.AddRange(order),
                null,
                $"{AppRoles.Admin},{AppRoles.AdvancedUser}",
                orderDto,
                context =>
                {
                    var dbOrder = context.Orders.FirstOrDefault(x => x.Id == order.Id);
                    Assert.NotNull(dbOrder);

                    Assert.NotEmpty(dbOrder.OrderItems);
                    Assert.Equal(orderDto.OrderItems.Count(), dbOrder.OrderItems.Count());

                    var dbOrderItemNew = dbOrder.OrderItems.SingleOrDefault(x => x.Name == orderItemNew.Name);
                    Assert.Equal(orderItemDto.Name, dbOrderItemNew.Name);
                    Assert.Equal(orderItemDto.Price, dbOrderItemNew.Price);
                    Assert.Equal(orderItemDto.Score, dbOrderItemNew.Score);
                });
        }

        [Fact]
        public async Task DeleteOrder()
        {
            var store = ObjectsHelper.CreateStore(Guid.NewGuid());
            var orders = Enumerable.Range(1, 3).Select(x => ObjectsHelper.CreateOrder($"ORD-{x}", store.Id, Guid.NewGuid())).ToArray();
            var firstOrder = orders.First();
            firstOrder.OrderItems = Enumerable.Range(1, 4).Select(_ => ObjectsHelper.CreateOrderItem()).ToArray();
            var firstOrderItem = firstOrder.OrderItems.First();

            await RunTestDelete(
                context => context.AddRange(orders),
                null,
                firstOrder.Id.ToString(),
                AppRoles.Admin.ToString(),
                context =>
                {
                    var dbOrder = context.Orders.FirstOrDefault(x => x.Id == firstOrder.Id);
                    Assert.Null(dbOrder);

                    var dbOrderItem = context.Orders.FirstOrDefault(x => x.Id == firstOrderItem.Id);
                    Assert.Null(dbOrderItem);

                    var otherOrderIds = orders.Where(x => x.Id != firstOrder.Id).Select(x => x.Id).ToList();
                    var dbOtherOrders = context.Orders.Where(x => otherOrderIds.Contains(x.Id)).ToArray();
                    Assert.Equal(otherOrderIds.Count(), dbOtherOrders.Count());
                });
        }
    }
}
