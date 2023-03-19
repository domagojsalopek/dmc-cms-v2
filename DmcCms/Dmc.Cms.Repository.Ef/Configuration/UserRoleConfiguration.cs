using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Annotations;
using Dmc.Identity;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class UserRoleConfiguration : DatabaseEntityConfiguration<IdentityRole>
    {
        #region Constructors

        internal UserRoleConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.Name)
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
