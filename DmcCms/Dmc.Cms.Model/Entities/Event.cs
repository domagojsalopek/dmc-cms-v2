using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Event : ContentBase
    {
        #region Constructors

        public Event()
        {
            EventType = EventType.Occurence;
            EventDate = DateTime.Today;
        }

        #endregion

        #region Foreign Keys

        public int? ImageId
        {
            get;
            set;
        }

        #endregion

        #region Properties

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

        public Image Image
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public override bool CanBeDisplayed => Status == Core.ContentStatus.Published;

        #endregion
    }
}
