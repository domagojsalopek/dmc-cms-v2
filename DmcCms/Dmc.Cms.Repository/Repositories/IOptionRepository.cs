using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IOptionRepository 
    {
        IEnumerable<Option> GetOptions();
        Task<Option> GetOptionByNameAsync(string name);
        Task<IEnumerable<Option>> GetOptionsAsync();
        void Insert(Option entity);
        void Update(Option entity);
    }
}
