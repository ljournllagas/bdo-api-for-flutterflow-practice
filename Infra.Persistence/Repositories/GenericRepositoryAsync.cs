using Application.Common.Parameters;
using Application.Interfaces.Repositories;
using Infra.Persistence.Contexts;
using Infra.Persistence.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories
{
    public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(long id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TRes> GetAsync<TRes>(Expression<Func<TEntity, bool>> predicate) where TRes : class
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var response = await query
                                .ProjectToType<TRes>()
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<IReadOnlyList<TRes>> GetAllAsync<TRes>(Expression<Func<TEntity, bool>> filter = null,
                                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                 string defaultOrderBy = "RowId")
            where TRes: class
        {
           
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            //implement the custom order by, if there's any.
            if (orderBy != null)
            {
                var response = await orderBy(query).ProjectToType<TRes>().AsNoTracking().ToListAsync();

                return response;
            }
            else
            {
                var toReturn = await query
                                .ProjectToType<TRes>()
                                .AsNoTracking()
                                .ToListAsync();

                //check if the default order by property is existing, in the domain entity.
                //if yes, then sort it by using the defaultOrderBy parameter
                //else, return the default retrieved data
                bool propertyExists = typeof(TRes).GetProperties()
                                .Any(x => x.Name == defaultOrderBy);

                return !propertyExists ? toReturn : toReturn.OrderBy(defaultOrderBy).ToList();
            }
        }

        public async Task<Tuple<IReadOnlyList<TRes>, int>> GetAllWithPagination<TRes>(PaginationParameter pagination,
                                                                                      Expression<Func<TEntity, bool>> filter = null,
                                                                                      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                                      string defaultOrderBy = "RowId") where TRes : class
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            int totalPages = await query.AsQueryable().CountAsync();

            //implement the custom order by, if there's any.
            if (orderBy != null)
            {

                return new Tuple<IReadOnlyList<TRes>, int>
                        (await orderBy(query)
                        .ProjectToType<TRes>()
                        .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                        .Take(pagination.PageSize)
                        .AsNoTracking()
                        .ToListAsync(), totalPages);
            }
            else
            {

                var toReturn = await query
                    .ProjectToType<TRes>()
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                //check if the default order by property is existing, in the domain entity.
                //if yes, then sort it by using the defaultOrderBy parameter
                //else, return the default retrieved data
                bool propertyExists = typeof(TRes).GetProperties()
                                .Any(x => x.Name == defaultOrderBy);

                return propertyExists ?
                    new Tuple<IReadOnlyList<TRes>, int>(toReturn.OrderBy(defaultOrderBy).ToList(), totalPages) : //with default order by
                    new Tuple<IReadOnlyList<TRes>, int>(toReturn, totalPages) //default return
                    ;
            }

        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entity)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteList(IReadOnlyList<TEntity> entity)
        {
            foreach (var item in entity)
            {
                _dbContext.Entry(item).State = EntityState.Deleted;
            }

            _dbContext.Set<TEntity>().RemoveRange(entity);
        }
    }
}
