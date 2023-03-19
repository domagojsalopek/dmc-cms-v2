using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class NewsletterConfiguration : DatabaseEntityConfiguration<Newsletter>
    {
        #region Constructors

        internal NewsletterConfiguration()
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

            Property(o => o.Content)
                .IsOptional()
                .HasColumnType("ntext");
        }

        private void ConfigureRelationships()
        {
            HasOptional(o => o.CreatedBy)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
