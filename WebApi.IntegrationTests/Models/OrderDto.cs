namespace WebApi.IntegrationTests.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Customer { get; set; }
        public decimal Total { get; set; }

        public Guid StoreId { get; set; }

        public IList<OrderItemDto> OrderItems { get; set; }
    }
}
