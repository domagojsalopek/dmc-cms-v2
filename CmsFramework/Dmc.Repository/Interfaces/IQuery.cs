using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Repository
{
    public interface IQuery<TEntity> where TEntity : class
    {
        //TODO: See if this is ok ...
        IQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        IQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQuery<TEntity> Include(Expression<Func<TEntity, object>> expression);

        IEnumerable<TEntity> GetEntities();
        TEntity FirstOrDefault();
        int Count();

        Task<IEnumerable<TEntity>> GetPagedEntitiesAsync(int page, int perPage);
        Task<IEnumerable<TEntity>> GetEntitiesAsync();
        Task<TEntity> FirstOrDefaultAsync();
        Task<int> CountAsync();
    }
}
