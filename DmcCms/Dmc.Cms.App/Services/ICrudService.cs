using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ICrudService<T> : IService<T> where T : class
    {
        Task<ServiceResult> InsertAsync(T entity);

        Task<ServiceResult> UpdateAsync(T entity);

        Task<ServiceResult> DeleteAsync(T entity);
    }
}
