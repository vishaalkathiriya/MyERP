using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblTechnologyMap : EntityTypeConfiguration<tblTechnology>
    {
        public tblTechnologyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Technologies)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblTechnologies");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Technologies).HasColumnName("Technologies");
            this.Property(t => t.TechnologiesGroupId).HasColumnName("TechnologiesGroupId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblTechnologiesGroup)
                .WithMany(t => t.tblTechnologies)
                .HasForeignKey(d => d.TechnologiesGroupId);

        }
    }
}
