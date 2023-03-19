using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class SearchResult
    {
        public string Slug
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string DescriptionExcerpt
        {
            get;
            set;
        }

        public string ContentExcerpt
        {
            get;
            set;
        }

        public Image PreviewImage
        {
            get;
            set;
        }
    }
}
