using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class OptionConfiguration : EntityTypeConfiguration<Option>
    {
        #region Constructor

        internal OptionConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            HasKey(o => o.Name);
            Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.Value)
                .IsRequired()
                .HasMaxLength(4000);
        }

        private void ConfigureRelationships()
        {
        }

        #endregion
    }
}
