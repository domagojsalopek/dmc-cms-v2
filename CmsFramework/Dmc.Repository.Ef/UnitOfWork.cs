using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dmc.Repository.Ef
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Private Fields

        private Hashtable _Repositories;
        private readonly DbContext _Context;

        #endregion

        #region Constructors

        public UnitOfWork(DbContext context)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Properties

        protected DbContext Context => _Context;

        #endregion

        #region IRepository Implementation

        public IRepository<T> Repository<T>() where T : class
        {
            if (_Repositories == null)
            {
                _Repositories = new Hashtable();
            }

            Type type = typeof(T);

            if (_Repositories.ContainsKey(type))
            {
                return _Repositories[type] as IRepository<T>;
            }

            lock (_Repositories.SyncRoot)
            {
                _Repositories.Add(type, new Repository<T>(Context));
            }

            return _Repositories[type] as IRepository<T>;
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex) //TODO: other exceptions
            {
                throw;
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception ex) //TODO: Other exceptions
            {
                throw;
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Context != null)
                    {
                        Context.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
