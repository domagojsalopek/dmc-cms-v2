using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class UserClaimConfiguration : DatabaseEntityConfiguration<IdentityClaim>
    {
        #region Constructors

        internal UserClaimConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.ClaimValue)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.ClaimType)
                .IsRequired()
                .HasMaxLength(255);
        }

        private void ConfigureRelationships()
        {
            // relationship with user configured in userconfiguration. 
        }

        #endregion
    }
}
