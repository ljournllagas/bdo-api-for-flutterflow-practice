using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepositoryAsync Customer { get; }

        IAddressRepositoryAsync Address { get; }

        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
