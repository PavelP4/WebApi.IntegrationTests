using Microsoft.EntityFrameworkCore;

namespace WebApi.IntegrationTests.Extentions
{
    public static class DbContextExts
    {
        public static void AddRangeToEmptyDb(this DbContext context, params object[] entities)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(entities);
        }

        public static void AddRangeToEmptyDb(this DbContext context, IEnumerable<object> entities)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(entities);
        }

        public static bool IsAttached<TContext, TEntity>(this TContext context, TEntity entity)
            where TContext : DbContext
            where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}
