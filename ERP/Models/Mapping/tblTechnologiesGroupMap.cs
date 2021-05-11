using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblTechnologiesGroupMap : EntityTypeConfiguration<tblTechnologiesGroup>
    {
        public tblTechnologiesGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TechnologiesGroup)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblTechnologiesGroup");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TechnologiesGroup).HasColumnName("TechnologiesGroup");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
