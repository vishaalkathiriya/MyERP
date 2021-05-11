using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRSubTypeMap : EntityTypeConfiguration<tblSRSubType>
    {
        public tblSRSubTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.SubTypeId);

            // Properties
            this.Property(t => t.SubTypeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Selection)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRSubType");
            this.Property(t => t.SubTypeId).HasColumnName("SubTypeId");
            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.SubTypeName).HasColumnName("SubTypeName");
            this.Property(t => t.Selection).HasColumnName("Selection");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRType)
                .WithMany(t => t.tblSRSubTypes)
                .HasForeignKey(d => d.TypeId);

        }
    }
}
