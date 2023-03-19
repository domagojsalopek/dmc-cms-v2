using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class LoggedInUserInfo
    {
        #region Properties

        public string UserId
        {
            get;
            set;
        }

        public string SecurityStamp
        {
            get;
            set;
        }

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

        public DateTimeOffset ExpiresUtc
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void FromUser(User user)
        {
            UserId = user.UniqueId.ToString();
            Name = user.FullName;
            Email = user.Email;
            SecurityStamp = user.SecurityStamp;
        }

        public string Serialize()
        {
            return string.Join("|", new string[] { UserId, Name, SecurityStamp, Email, ExpiresUtc.UtcTicks.ToString() });
        }

        public bool FromString(string userInfoAsString)
        {
            if (string.IsNullOrWhiteSpace(userInfoAsString))
            {
                return false;
            }

            string[] parts = userInfoAsString.Split('|');

            if (parts.Length < 5)
            {
                return false;
            }

            UserId = parts[0];
            Name = parts[1];
            SecurityStamp = parts[2];
            Email = parts[3];

            if (!long.TryParse(parts[4], out long ticksAsLong))
            {
                return false;
            }

            ExpiresUtc = new DateTimeOffset(ticksAsLong, TimeSpan.Zero);
            return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(SecurityStamp);
        }

        #endregion
    }
}
