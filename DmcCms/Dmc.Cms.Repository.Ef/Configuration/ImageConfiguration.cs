using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class ImageConfiguration : DatabaseEntityConfiguration<Image>
    {
        #region Constructors

        internal ImageConfiguration()
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

            Property(o => o.AltText)
                .IsOptional()
                .HasMaxLength(4000);

            Property(o => o.Caption)
                .IsOptional()
                .HasMaxLength(4000);
        }

        private void ConfigureRelationships()
        {
        }

        #endregion
    }
}
