using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class EventConfiguration : DatabaseEntityConfiguration<Event>
    {
        #region Constructor

        internal EventConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Ignore(c => c.CanBeDisplayed);

            Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.EventDate)
                .IsRequired();

            Property(o => o.Slug)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Content)
                .HasColumnType("ntext")
                .IsRequired();

            Property(o => o.Description)
                .IsOptional()
                .HasMaxLength(4000);

            Property(o => o.Order)
                .IsRequired();

            // date is index
            HasIndex(o => o.EventDate)
                .IsUnique(false);

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
            HasOptional(o => o.Image)
                .WithMany()
                .HasForeignKey(o => o.ImageId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
