using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class GeneralContentGroup : ContentBase
    {
        #region Constructors

        public GeneralContentGroup()
        {
            Published = DateTimeOffset.Now;
        }

        #endregion

        #region Foreign Key Properties

        public int? ParentId
        {
            get;
            set;
        }

        public int? IntroImageId
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

        public GeneralContentGroup Parent
        {
            get;
            set;
        }

        public ICollection<GeneralContentGroup> Children
        {
            get;
            private set;
        }

        public ICollection<GeneralContent> Contents
        {
            get;
            private set;
        }

        public Image IntroImage
        {
            get;
            set;
        }

        public override bool CanBeDisplayed => Status == Core.ContentStatus.Published;

        #endregion

        #region Non Database Properties

        public bool HasChildren => Children != null && Children.Count > 0;

        public bool IsRoot => !ParentId.HasValue;

        #endregion
    }
}
