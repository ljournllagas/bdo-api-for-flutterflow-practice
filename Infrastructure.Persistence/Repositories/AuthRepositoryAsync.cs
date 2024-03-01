using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AuthRepositoryAsync : GenericRepositoryAsync<Auth>, IAuthRepositoryAsync
    {
        private readonly DbSet<Auth> _db;

        public AuthRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext.Set<Auth>();
        }
    }
}
