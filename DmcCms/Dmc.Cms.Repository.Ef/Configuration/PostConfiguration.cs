using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class PostConfiguration : DatabaseEntityConfiguration<Post>
    {
        #region Constructor

        internal PostConfiguration() 
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

            Property(o => o.Slug)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Content)
                .HasColumnType("ntext")
                .IsRequired();

            Property(o => o.Description)
                .IsOptional()
                .HasMaxLength(4000);

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
            HasOptional(o => o.PreviewImage)
                .WithMany()
                .HasForeignKey(o => o.PreviewImageId)
                .WillCascadeOnDelete(false);

            HasOptional(o => o.DetailImage)
                .WithMany()
                .HasForeignKey(o => o.DetailImageId)
                .WillCascadeOnDelete(false);

            // has an author - we must change this to required when membership is done
            HasOptional(o => o.Author)
                .WithMany()
                .HasForeignKey(o => o.AuthorId)
                .WillCascadeOnDelete(false);

            // Configure comments
            HasMany(o => o.Comments)
                .WithRequired(o => o.Post)
                .HasForeignKey(o => o.PostId)
                .WillCascadeOnDelete(true);

            // Many to many tags
            HasMany(a => a.Tags).WithMany(t => t.Posts).Map(
                at => at.MapLeftKey("PostId")
                        .MapRightKey("TagId")
                        .ToTable("PostTags"));

            HasMany(a => a.Gallery).WithMany().Map(
                at => at.MapLeftKey("PostId")
                        .MapRightKey("ImageId")
                        .ToTable("PostImages"));

            // Relationship with category configured in category configuration
        }

        #endregion
    }
}
