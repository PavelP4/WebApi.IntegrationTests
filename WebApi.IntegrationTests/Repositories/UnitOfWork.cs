using WebApi.IntegrationTests.Infrastructure;
using WebApi.IntegrationTests.Infrastructure.Entities;

namespace WebApi.IntegrationTests.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly IServiceScope _serviceScope;

        public UnitOfWork(IServiceProvider serviceProvider) 
        {
            _serviceScope = serviceProvider.CreateScope();
        }

        private IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

        private AppDbContext _context;
        private AppDbContext Context => _context ??= ServiceProvider.GetRequiredService<AppDbContext>();

        private BaseRepository<Order> _orderRepository;
        public BaseRepository<Order> OrderRepository => _orderRepository ??= ServiceProvider.GetRequiredService<BaseRepository<Order>>();

        public async Task SaveChanges()
        { 
            await Context.SaveChangesAsync();
        }

        private bool _disposed;

        public void Dispose(bool disposing = true) 
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                    _serviceScope?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork() 
        {
            Dispose(false);
        }
    }
}
