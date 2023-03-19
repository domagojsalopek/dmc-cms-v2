using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dmc.Cms.Core;

namespace Dmc.Cms.Web.ViewModels
{
    public abstract class ContentViewModelBase : CrudViewModelBase, IContentViewModel
    {
        public virtual ContentStatus Status
        {
            get;
            set;
        }

        public virtual string Slug
        {
            get;
            set;
        }

        public virtual DateTimeOffset? Published
        {
            get;
            set;
        }

        public virtual bool CanBeDisplayed => Status == ContentStatus.Published;
    }
}