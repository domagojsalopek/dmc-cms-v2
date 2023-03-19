using Dmc.Cms.Core;
using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class ContactQuery : IEntity, IModifiedInfo
    {
        #region Constructors

        public ContactQuery()
        {

        }

        #endregion

        #region Key

        public int Id
        {
            get;
            set;
        }

        public int? UserId
        {
            get;
            set;
        }

        #endregion

        #region Properties

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public string AttachmentPath
        {
            get;
            set;
        }

        public DateTimeOffset Created
        {
            get;
            set;
        }

        public DateTimeOffset Modified
        {
            get;
            set;
        }

        public User User
        {
            get;
            set;
        }

        #endregion
    }
}
