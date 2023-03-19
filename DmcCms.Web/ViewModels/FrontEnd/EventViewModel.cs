using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class EventViewModel : ContentViewModelBase
    {
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

        public ImageViewModel Image
        {
            get;
            set;
        }

        public EventType EventType
        {
            get;
            set;
        }

        public DateTime EventDate
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }
    }
}