using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class TagConfiguration : DatabaseEntityConfiguration<Tag>
    {
        #region Constructors

        internal TagConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.Slug)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(255);

            // Slug is index, is unique
            HasIndex(o => o.Slug)
                .IsUnique(true);

            Property(o => o.UniqueId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            HasIndex(o => o.UniqueId)
                .IsUnique(true);
        }

        private void ConfigureRelationships()
        {
            // relationship with post in post configuration
        }

        #endregion
    }
}
