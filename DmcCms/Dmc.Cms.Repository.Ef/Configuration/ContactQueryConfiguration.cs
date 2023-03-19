using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class ContactQueryConfiguration : DatabaseEntityConfiguration<ContactQuery>
    {
        #region Constructors

        internal ContactQueryConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.IP)
                .IsRequired()
                .HasMaxLength(64);

            Property(o => o.Message)
                .IsRequired()
                .HasMaxLength(4000);

            Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Subject)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.AttachmentPath)
                .IsOptional()
                .HasMaxLength(4000);
        }

        private void ConfigureRelationships()
        {
            HasOptional(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);
        }

        #endregion
    }
}
