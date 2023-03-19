using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Advertisement : EntityBase
    {
        // TODO: Should be possible to have a image and a link.
        // or should have a banner which has image and a link
        // or banners separate from advertisements?
        // TODO: think about this

        public Advertisement()
        {
            AdvertisementType = AdvertisementType.Aside;
        }

        public Guid UniqueId
        {
            get;
            private set;
        }

        public AdvertisementType AdvertisementType
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Html
        {
            get;
            set;
        }

        public bool IsVisible
        {
            get;
            set;
        }

        public DateTimeOffset? VisibleFrom
        {
            get;
            set;
        }

        public DateTimeOffset? VisibleTo
        {
            get;
            set;
        }
    }
}
