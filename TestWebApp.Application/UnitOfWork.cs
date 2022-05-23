using Serilog;
using System;
using System.Threading.Tasks;
using TestWebApp.Infrastructure;

namespace TestWebApp.Application
{
    public class UnitOfWork
    {
        private IServiceProvider _serviceProvider;
        private TestWebAppDbContext _db;

        public UnitOfWork(IServiceProvider serviceProvider, TestWebAppDbContext db)
        {
            _db = db;
            _serviceProvider = serviceProvider;
        }

        public virtual async Task SaveChangesAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected exception happened: " + ex.Message);
                throw;
            }
        }

        public GenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class
        {
            return (GenericRepository<TEntity>)_serviceProvider.GetService(typeof(GenericRepository<TEntity>));
        }
    }
}
