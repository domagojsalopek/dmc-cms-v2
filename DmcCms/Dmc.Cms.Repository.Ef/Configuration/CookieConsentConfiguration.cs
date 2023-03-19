using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class CookieConsentConfiguration : DatabaseEntityConfiguration<CookieConsent>
    {
        #region Constructors

        internal CookieConsentConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.EncryptedValue)
                .IsRequired()
                .HasMaxLength(4000);

            Property(o => o.IpAddress)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.UserAgent)
                .IsOptional()
                .HasMaxLength(1000);

            Property(o => o.RequestUrl)
                .IsOptional()
                .HasMaxLength(2000);

            Property(o => o.UniqueId)
                .IsRequired();

            HasIndex(o => o.UniqueId)
                .IsUnique(true);
        }

        private void ConfigureRelationships()
        {
        }

        #endregion
    }
}
