using WebApi.IntegrationTests.Infrastructure.Entities;

namespace WebApi.IntegrationTests.IntegrationTests
{
    public static class ObjectsHelper
    {
        public static Order CreateOrder(string orderNumber)
        {
            return new Order()
            {
                Number = orderNumber,
                Customer = Guid.NewGuid().ToString(),
                Total = new Random().Next(0, 100)
            };
        }

        public static Order CreateOrder(string orderNumber, Guid storeId, Guid? id = null)
        {
            var order = CreateOrder(orderNumber);
            order.Id = id ?? default;
            order.StoreId = storeId;
            return order;
        }

        public static OrderItem CreateOrderItem(Guid? id = null)
        {
            return new OrderItem() 
            {
                Id = id ?? default,
                Name = Guid.NewGuid().ToString(),
                Price = new Random().Next(0, 100),
                Score = (byte)new Random().Next(0, 10)
            };
        }

        public static Store CreateStore(Guid? id = null)
        {
            return new Store()
            {
                Id = id ?? default,
                Name = Guid.NewGuid().ToString()
            };
        }
    }
}
