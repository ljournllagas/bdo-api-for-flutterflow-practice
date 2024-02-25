using Application.Interfaces.Repositories;
using Infra.Persistence.Contexts;
using Infra.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;

        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _dbContext;


        public UnitOfWork(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        #region "UnitOfWork Implementation"
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        public async Task RollbackAsync()
        {
            await _dbContext.DisposeAsync();
        }


        #endregion


    }
}
