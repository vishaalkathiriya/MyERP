using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblStateMap : EntityTypeConfiguration<tblState>
    {
        public tblStateMap()
        {
            // Primary Key
            this.HasKey(t => t.StateId);

            // Properties
            this.Property(t => t.StateName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblStates");
            this.Property(t => t.StateId).HasColumnName("StateId");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.StateName).HasColumnName("StateName");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblCountry)
                .WithMany(t => t.tblStates)
                .HasForeignKey(d => d.CountryId);

        }
    }
}
