using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Repository;

namespace Dmc.Cms.Repository.Ef
{
    internal class OptionRepository : IOptionRepository
    {
        public OptionRepository(IRepository<Option> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        private IRepository<Option> Repository { get; }

        public Task<Option> GetOptionByNameAsync(string name)
        {
            return Repository.Query()
                .Filter(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public IEnumerable<Option> GetOptions()
        {
             return Repository.Query().GetEntities();
        }

        public Task<IEnumerable<Option>> GetOptionsAsync()
        {
            return Repository.Query().GetEntitiesAsync();
        }

        public virtual void Insert(Option entity)
        {
            // does not belong here
            entity.Modified = DateTimeOffset.Now;
            Repository.Insert(entity);
        }

        public virtual void Update(Option entity)
        {
            entity.Modified = DateTimeOffset.Now;
            Repository.Update(entity);
        }
    }
}
