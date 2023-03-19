using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Annotations;

namespace Dmc.Cms.Repository.Ef.Configuration
{ 
    internal abstract class DatabaseEntityConfiguration<T> : EntityTypeConfiguration<T> where T : class, IEntity
    {
        #region Constructors

        protected DatabaseEntityConfiguration()
        {
            HasKey(o => o.Id);
            Property(o => o.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
        }

        #endregion
    }
}
