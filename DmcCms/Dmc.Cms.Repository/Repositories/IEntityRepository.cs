using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IEntityRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetPagedAsync(int page, int PerPage);
        Task<int> CountAsync();
        void Insert(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(T entity);
    }
}
