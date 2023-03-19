using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class EventListViewModel
    {
        #region Properties

        public DateTime DateRequested
        {
            get;
            set;
        }

        public List<EventViewModel> Events
        {
            get;
            set;
        }

        #endregion
    }
}