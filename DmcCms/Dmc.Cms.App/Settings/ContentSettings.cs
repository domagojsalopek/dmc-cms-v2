using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class ContentSettings : IContentSettings
    {
        public bool IsAdvertisingEnabled
        {
            get;
            set;
        }

        public bool IsShowingEventsEnabled
        {
            get;
            set;
        }
    }
}
