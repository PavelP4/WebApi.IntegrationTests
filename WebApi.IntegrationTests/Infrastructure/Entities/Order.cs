namespace WebApi.IntegrationTests.Infrastructure.Entities
{
    public class Order : IDbEntity
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Customer {  get; set; }
        public decimal Total { get; set; }

        public Guid StoreId { get; set; }
        public virtual Store Store { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
