using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IRoleStore<T, TKey>
        where T : IRole<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<T> FindByIdAsync(TKey id);

        Task<T> FindByNameAsync(string name);

        Task<MembershipResult> CreateAsync(T role);

        Task<MembershipResult> UpdateAsync(T role);

        Task<MembershipResult> DeleteAsync(T role);
    }
}
