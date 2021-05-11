using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRPartMap : EntityTypeConfiguration<tblSRPart>
    {
        public tblSRPartMap()
        {
            // Primary Key
            this.HasKey(t => t.PartId);

            // Properties
            this.Property(t => t.PartName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRPart");
            this.Property(t => t.PartId).HasColumnName("PartId");
            this.Property(t => t.PartName).HasColumnName("PartName");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
