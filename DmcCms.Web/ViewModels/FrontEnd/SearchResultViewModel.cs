namespace Dmc.Cms.Web.ViewModels
{
    public class SearchResultViewModel
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

        public string Excerpt
        {
            get;
            set;
        }

        public string FullTextExcerpt
        {
            get;
            set;
        }

        public ImageViewModel PreviewImage
        {
            get;
            set;
        }
    }
}