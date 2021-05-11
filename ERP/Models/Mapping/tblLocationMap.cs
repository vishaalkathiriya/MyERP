using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblLocationMap : EntityTypeConfiguration<tblLocation>
    {
        public tblLocationMap()
        {
            // Primary Key
            this.HasKey(t => t.LocationId);

            // Properties
            this.Property(t => t.LocationName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblLocation");
            this.Property(t => t.LocationId).HasColumnName("LocationId");
            this.Property(t => t.LocationName).HasColumnName("LocationName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
