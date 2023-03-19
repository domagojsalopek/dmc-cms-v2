using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IService
    {
    }

    public interface IService<T> : IDisposable, IService where T : class
    {
        Task<IEnumerable<T>> GetPagedAsync(int page, int perPage);

        Task<int> CountAsync();

        Task<T> GetByIdAsync(int id);
    }
}