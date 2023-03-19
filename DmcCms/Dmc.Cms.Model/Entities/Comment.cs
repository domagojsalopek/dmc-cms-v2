using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Comment : EntityBase
    {
        #region Constructors

        public Comment()
        {
            Comments = new List<Comment>();
        }

        #endregion

        #region Relation Key Properties

        public int PostId
        {
            get;
            set;
        }

        public int? ParentId
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

        public CommentStatus Status
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public bool Approved
        {
            get;
            set;
        }

        public User User
        {
            get;
            set;
        }

        public Post Post //do we need this?
        {
            get;
            set;
        }

        public Comment Parent
        {
            get;
            set;
        }

        public ICollection<Comment> Comments
        {
            get;
            private set;
        }

        #endregion

        #region Non-Storage Properties

        public bool IsRoot => !ParentId.HasValue;

        public bool HasChildren => Comments.Count > 0;

        #endregion
    }
}
