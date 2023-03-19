using Dmc.Cms.Core;
using Dmc.Core;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal abstract class EntityRepositoryBase<T> : IEntityRepository<T> where T : class, IEntity, IModifiedInfo
    {
        #region Constructors

        protected EntityRepositoryBase(IRepository<T> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region Properties

        protected IRepository<T> Repository { get; }

        #endregion

        #region Public Methods

        public virtual Task<int> CountAsync()
        {
            return Repository.Query().CountAsync();
        }

        public virtual void Delete(int id)
        {
            Repository.Delete(id);
        }

        public virtual void Delete(T entity)
        {
            Repository.Delete(entity);
        }

        public virtual Task<T> GetByIdAsync(int id)
        {
            return Repository.FindAsync(id);
        }

        public virtual Task<IEnumerable<T>> GetPagedAsync(int page, int perPage)
        {
            return Repository
                .Query()
                .OrderBy(o => o.OrderByDescending(d => d.Created))
                .GetPagedEntitiesAsync(page, perPage);
        }

        public virtual void Insert(T entity)
        {
            entity.Created = DateTimeOffset.Now; // we shouldn't do this here, but in service or somewhere. this way we fuck up if in any way somebody wants to set something different.
            // does not belong this low
            entity.Modified = DateTimeOffset.Now;

            Repository.Insert(entity);
        }

        public virtual void Update(T entity)
        {
            entity.Modified = DateTimeOffset.Now; // we shouldn't do this here

            Repository.Update(entity);
        }

        #endregion
    }
}
