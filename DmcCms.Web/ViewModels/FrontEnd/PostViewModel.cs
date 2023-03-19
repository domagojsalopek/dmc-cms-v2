using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class PostViewModel : ContentViewModelBase
    {
        public PostViewModel()
        {
            Categories = new List<CategoryListViewModel>();
            Tags = new List<TagListViewModel>();
        }

        public PostInfoViewModel PreviousPost
        {
            get;
            set;
        }

        public PostInfoViewModel NextPost
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public bool IsCommentingEnabled
        {
            get;
            set;
        }

        public ImageViewModel PreviewImage
        {
            get;
            set;
        }

        public ImageViewModel DetailImage
        {
            get;
            set;
        }

        public ICollection<CategoryListViewModel> Categories
        {
            get;
            set;
        }

        public ICollection<TagListViewModel> Tags
        {
            get;
            set;
        }

        public bool IsFavourite
        {
            get;
            set;
        }

        public bool HasRating
        {
            get;
            set;
        }

        public bool Liked
        {
            get;
            set;
        }

        public int NumberOfComments
        {
            get;
            set;
        }
    }
}