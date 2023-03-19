using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Web.ViewModels
{
    public interface IContentViewModel : ICrudViewModel
    {
        ContentStatus Status
        {
            get;
            set;
        }

        bool CanBeDisplayed
        {
            get;
        }

        string Slug
        {
            get;
            set;
        }

        DateTimeOffset? Published
        {
            get;
            set;
        }
    }
}
