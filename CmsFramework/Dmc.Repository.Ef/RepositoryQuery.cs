using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Dmc.Repository.Ef
{
    public class RepositoryQuery<T> : IQuery<T> where T : class
    {
        #region Fields

        private readonly Repository<T> _Repository;
        private readonly List<Expression<Func<T, object>>> _IncludeProperties;
        private List<Expression<Func<T, bool>>> _Filter;
        Func<IQueryable<T>, IOrderedQueryable<T>> _OrderBy;

        #endregion

        #region Constructors

        public RepositoryQuery(Repository<T> repository)
        {
            _Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _IncludeProperties = new List<Expression<Func<T, object>>>();
            _Filter = new List<Expression<Func<T, bool>>>();
        }

        #endregion

        #region IQuery Implementation

        public IQuery<T> Filter(Expression<Func<T, bool>> filter)
        {
            _Filter.Add(filter ?? throw new ArgumentNullException(nameof(filter)));
            return this;
        }

        public IQuery<T> Include(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            _IncludeProperties.Add(expression);
            return this;
        }

        public IQuery<T> OrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            _OrderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            return this;
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync()
        {
            return await Queryable().ToListAsync();
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return await Queryable().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetPagedEntitiesAsync(int page, int perPage)
        {
            var query = Queryable();

            query = query
                .Skip((page - 1) * perPage)
                .Take(perPage);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await Queryable().CountAsync();
        }

        #endregion

        #region Private Methods

        private IQueryable<T> Queryable()
        {
            return _Repository.Get(_Filter, _OrderBy, _IncludeProperties);
        }

        public IEnumerable<T> GetEntities()
        {
            return Queryable().ToList();
        }

        public T FirstOrDefault()
        {
            return Queryable().FirstOrDefault();
        }

        public int Count()
        {
            return Queryable().Count();
        }

        #endregion
    }
}
