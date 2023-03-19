using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Page : ContentBase
    {
        #region Constructors

        public Page()
        {
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

        public int Order
        {
            get;
            set;
        }

        public User Author
        {
            get;
            set;
        }

        public ICollection<Image> Gallery
        {
            get;
            private set;
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

        #endregion

        #region Abstract Implementation

        public override bool CanBeDisplayed => Status == ContentStatus.Published;

        #endregion
    }
}
