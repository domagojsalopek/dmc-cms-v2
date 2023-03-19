using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Repository.Ef
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly DbSet<T> _DbSet;
        private readonly DbContext _Context;

        #endregion

        #region Constructors

        public Repository(DbContext context)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
            _DbSet = context.Set<T>();
        }

        #endregion

        #region Properties

        protected DbSet<T> DbSet => _DbSet;

        protected DbContext Context => _Context;

        #endregion

        #region IRepository

        public T Find(params object[] keyValues)
        {
            return _DbSet.Find(keyValues);
        }

        public async Task<T> FindAsync(params object[] keyValues)
        {
            return await _DbSet.FindAsync(keyValues);
        }

        public void Insert(T entity)
        {
            _DbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _DbSet.Attach(entity);
            _Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var entity = Find(id);
            Delete(entity);
        }

        public void Delete(T entity)
        {
            if (_Context.Entry(entity).State == EntityState.Detached)
            {
                _DbSet.Attach(entity);
            }

            _DbSet.Remove(entity);
        }

        public IQuery<T> Query()
        {
            return new RepositoryQuery<T>(this);
        }

        #endregion

        #region Internal Methods

        // helper method which query calls
        internal IQueryable<T> Get(List<Expression<Func<T, bool>>> filter = null,
                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                   List<Expression<Func<T, object>>> includeProperties = null)
        {
            // start
            IQueryable<T> query = _DbSet;

            // include
            if (includeProperties != null)
            {
                includeProperties.ForEach(i => query = query.Include(i));
            }

            // filter
            if (filter != null)
            {
                filter.ForEach(i => query = query.Where(i));
                //query = query.Where(filter);
            }

            // query
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }

        #endregion
    }
}
