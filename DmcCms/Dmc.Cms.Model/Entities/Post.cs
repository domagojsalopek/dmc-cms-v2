using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Post : ContentBase
    {
        #region Constructors

        public Post()
        {
            Comments = new List<Comment>();
            Tags = new List<Tag>();
            Categories = new List<Category>();
            Ratings = new List<Rating>();
            Gallery = new List<Image>();
        }

        #endregion

        #region Foreign Keys

        public int? AuthorId
        {
            get;
            set;
        }

        public int? PreviewImageId
        {
            get;
            set;
        }

        public int? DetailImageId
        {
            get;
            set;
        }

        #endregion

        #region Properties

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

        // raters ...
        // rating ...

        public bool IsCommentingEnabled
        {
            get;
            set;
        }

        public User Author
        {
            get;
            set;
        }

        public Image PreviewImage
        {
            get;
            set;
        }

        public Image DetailImage
        {
            get;
            set;
        }

        public ICollection<Category> Categories
        {
            get;
            private set;
        }

        public ICollection<Comment> Comments
        {
            get;
            private set;
        }

        public ICollection<Tag> Tags
        {
            get;
            private set;
        }

        public ICollection<Rating> Ratings
        {
            get;
            private set;
        }

        public ICollection<Image> Gallery
        {
            get;
            private set;
        }

        #endregion

        #region Abstract Implementation

        public override bool CanBeDisplayed => Status == ContentStatus.Published;

        #endregion
    }
}
