namespace WebApi.IntegrationTests.Infrastructure.Entities
{
    public class OrderItem : IDbEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public byte Score { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
