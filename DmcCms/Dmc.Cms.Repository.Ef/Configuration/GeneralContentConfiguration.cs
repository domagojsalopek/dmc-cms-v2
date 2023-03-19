using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class GeneralContentConfiguration : DatabaseEntityConfiguration<GeneralContent>
    {
        #region Constructor

        internal GeneralContentConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion
        
        #region Private Methods

        private void ConfigureProperties()
        {
            Ignore(o => o.CanBeDisplayed);

            Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(255);

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
            HasRequired(a => a.ContentGroup)
                   .WithMany(c => c.Contents)
                   .HasForeignKey(a => a.ContentGroupId)
                   .WillCascadeOnDelete(true);

            HasOptional(o => o.PreviewImage)
                .WithMany()
                .HasForeignKey(o => o.PreviewImageId)
                .WillCascadeOnDelete(false);

            HasOptional(o => o.DetailImage)
                .WithMany()
                .HasForeignKey(o => o.DetailImageId)
                .WillCascadeOnDelete(false);

            HasMany(a => a.Gallery).WithMany().Map(
                at => at.MapLeftKey("ContentId")
                        .MapRightKey("ImageId")
                        .ToTable("ContentImages"));
        }

        #endregion
    }
}
