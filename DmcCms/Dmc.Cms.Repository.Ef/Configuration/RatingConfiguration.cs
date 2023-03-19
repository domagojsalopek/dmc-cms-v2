using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class RatingConfiguration : DatabaseEntityConfiguration<Rating>
    {
        #region Constructor

        internal RatingConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            const string IndexName = "IX_UserIdPostId";

            Property(o => o.UserId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(IndexName, 1) { IsUnique = true }));

            Property(o => o.PostId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(IndexName, 2) { IsUnique = true }));
        }

        private void ConfigureRelationships()
        {
            HasRequired(o => o.User)
                .WithMany(o => o.Ratings)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            HasRequired(o => o.Post)
                .WithMany(o => o.Ratings)
                .HasForeignKey(o => o.PostId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
