using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class GeneralContentGroupConfiguration : DatabaseEntityConfiguration<GeneralContentGroup>
    {
        #region Constructor

        internal GeneralContentGroupConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.Description)
                .IsOptional()
                .HasMaxLength(4000);

            Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Slug)
                .IsRequired()
                .HasMaxLength(255);

            // Slug is index, is unique
            HasIndex(o => o.Slug)
                .IsUnique(true);

            Property(o => o.UniqueId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            HasIndex(o => o.UniqueId)
                .IsUnique(true);

            Ignore(o => o.CanBeDisplayed);
            Ignore(o => o.HasChildren);
            Ignore(o => o.IsRoot);
        }

        private void ConfigureRelationships()
        {
            HasOptional(o => o.IntroImage)
                .WithMany()
                .HasForeignKey(o => o.IntroImageId)
                .WillCascadeOnDelete(false);

            // self-reference
            HasOptional(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
