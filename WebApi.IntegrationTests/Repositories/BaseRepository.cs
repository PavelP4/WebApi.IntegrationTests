using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Infrastructure;
using WebApi.IntegrationTests.Infrastructure.Entities;

namespace WebApi.IntegrationTests.Repositories
{
    public class BaseRepository<TEntity> where TEntity : class, IDbEntity
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public DbSet<TEntity> DbSet => _context.Set<TEntity>();

        public virtual async Task<IEnumerable<TEntity>> GetAll() 
        { 
            return await DbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            return (await _context.AddAsync(entity)).Entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual async Task Delete(Guid id) 
        {
            var entity = await DbSet.FindAsync(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public bool IsAttached(TEntity entity)
        {
            return DbSet.Local.Any(e => e == entity);
        }
    }
}
