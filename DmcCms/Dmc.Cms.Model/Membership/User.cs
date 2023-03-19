using Dmc.Core;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    // for now we'll have them in the same table.
    public class User : IdentityUser, IEntity
    {
        #region Constructors

        public User()
        {
            Comments = new List<Comment>();
            FavouritePosts = new List<Post>();
            Ratings = new List<Rating>();
        }

        #endregion

        #region Properties

        public override int Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string NickName // need this not to reveal usernames
        {
            get;
            set;
        }

        public string About
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        #endregion

        #region NonDatabase

        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
                {
                    return string.Empty;
                }

                return string.Format("{0} {1}", FirstName, LastName).Trim();
            }
        }

        #endregion

        #region Relationships

        public ICollection<Comment> Comments
        {
            get;
            private set;
        }

        public ICollection<Post> FavouritePosts
        {
            get;
            private set;
        }

        public ICollection<Rating> Ratings
        {
            get;
            private set;
        }

        #endregion

        #region Non Database Properties

        public bool HasPassword => PasswordHash != null;

        #endregion
    }
}
