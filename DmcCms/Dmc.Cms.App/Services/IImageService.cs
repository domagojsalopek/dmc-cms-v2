using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IImageService : ICrudService<Image>
    {
        Task<IEnumerable<Image>> GetAllImagesAsync();
    }
}
