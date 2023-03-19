using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class CommentConfiguration : DatabaseEntityConfiguration<Comment>
    {
        #region Constructors

        internal CommentConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Ignore(o => o.HasChildren);
            Ignore(o => o.IsRoot);

            // 25.08. Making things optional for GDPR.
            Property(o => o.Author)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.Email)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.Text)
                .IsRequired()
                .HasMaxLength(4000);

            Property(o => o.IP)
                .IsOptional()
                .HasMaxLength(64);
        }

        private void ConfigureRelationships()
        {
            // Relation with post configured in post configuration

            // Self reference
            HasOptional(x => x.Parent)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ParentId)
                .WillCascadeOnDelete(false);

            // Has user
            HasOptional(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
