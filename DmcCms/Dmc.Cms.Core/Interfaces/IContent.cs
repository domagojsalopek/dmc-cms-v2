using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Core
{
    public interface IContent : IEntity
    {
        Guid UniqueId
        {
            get;
        }

        string Slug
        {
            get;
            set;
        }

        ContentStatus Status
        {
            get;
            set;
        }

        DateTimeOffset? Published
        {
            get;
            set;
        }
    }
}
