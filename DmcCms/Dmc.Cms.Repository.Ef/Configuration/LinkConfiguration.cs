using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class LinkConfiguration : DatabaseEntityConfiguration<Link>
    {
        #region Constructor

        internal LinkConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.CssClass)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.Location)
                .IsRequired()
                .HasMaxLength(2000);

            Property(o => o.Text)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Tooltip)
                .IsOptional()
                .HasMaxLength(255);
        }

        private void ConfigureRelationships()
        {
            
        }

        #endregion
    }
}
