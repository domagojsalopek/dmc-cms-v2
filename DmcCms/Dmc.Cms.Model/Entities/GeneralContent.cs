using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class GeneralContent : ContentBase
    {
        #region Constructors

        public GeneralContent()
        {
        }

        #endregion

        #region Foreign Key Properties

        public int ContentGroupId
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

        public ICollection<Image> Gallery
        {
            get;
            private set;
        }

        public GeneralContentGroup ContentGroup
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

        public override bool CanBeDisplayed => Status == Core.ContentStatus.Published;

        #endregion
    }
}
