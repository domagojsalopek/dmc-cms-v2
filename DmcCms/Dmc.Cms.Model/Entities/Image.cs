using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Image : EntityBase
    {
        public string Name
        {
            get;
            set;
        }

        public string AltText
        {
            get;
            set;
        }

        public string Caption
        {
            get;
            set;
        }

        public string SmallImage
        {
            get;
            set;
        }

        public string LargeImage
        {
            get;
            set;
        }        
    }
}
