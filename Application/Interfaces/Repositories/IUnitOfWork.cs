using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepositoryAsync Auth { get;  }

        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
