using Application.Common.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{

    public interface IGenericRepositoryAsync<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> GetByIdAsync(long id);

        Task<TEntity> GetByIdAsync(Guid id);

        Task<TEntity> GetByIdAsync(string id);

        Task<TRes> GetAsync<TRes>(Expression<Func<TEntity, bool>> predicate) where TRes : class;

        Task<IReadOnlyList<TRes>> GetAllAsync<TRes>(Expression<Func<TEntity, bool>> filter = null,
                                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                   string defaultOrderBy = "Id") where TRes : class;
        Task<Tuple<IReadOnlyList<TRes>, int>> GetAllWithPagination<TRes>(PaginationParameter pagination,
                                                                        Expression<Func<TEntity, bool>> filter = null,
                                                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                        string defaultOrderBy = "Id") where TRes : class;

        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entity);
        void Delete(TEntity entity);
        void DeleteList(IReadOnlyList<TEntity> entity);
    }
}
