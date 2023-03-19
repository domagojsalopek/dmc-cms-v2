using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;

namespace Dmc.Cms.Repository.Ef
{
    internal class ImageRepository : EntityRepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(IRepository<Image> repository) 
            : base(repository)
        {
        }

        public Task<IEnumerable<Image>> GetAllAsync()
        {
            return Repository.Query().GetEntitiesAsync();
        }
    }
}
