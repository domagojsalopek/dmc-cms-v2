using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class CrudViewModelBase : ICrudViewModel
    {
        public int? Id
        {
            get;
            set;
        }

        public bool IsNewObject => !Id.HasValue;
    }
}