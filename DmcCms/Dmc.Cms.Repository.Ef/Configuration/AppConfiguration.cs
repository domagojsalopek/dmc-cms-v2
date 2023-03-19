using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class AppConfiguration : DatabaseEntityConfiguration<App>
    {
        #region Constructors

        internal AppConfiguration()
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

            Property(o => o.Description)
                .IsOptional()
                .HasMaxLength(4000);

            Property(o => o.ClientId)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.ClientSecret)
                .IsRequired()
                .HasMaxLength(1000);

            // Slug is index, is unique
            HasIndex(o => o.ClientId)
                .IsUnique(true);
        }

        private void ConfigureRelationships()
        {
        }

        #endregion
    }
}
