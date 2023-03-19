using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class SiteResources : ISiteResources
    {
        public string Logo
        {
            get;
            set;
        }

        public string RetinaLogo
        {
            get;
            set;
        }

        public string FooterLogo
        {
            get;
            set;
        }

        public string DefaultSiteHeader
        {
            get;
            set;
        }

        public string EmailHeader
        {
            get;
            set;
        }

        public string DefaultOpenGraphImage
        {
            get;
            set;
        }
    }
}
