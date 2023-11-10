namespace WebApi.IntegrationTests.Models
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public byte Score { get; set; }
    }
}
