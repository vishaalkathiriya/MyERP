using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRExtraMap : EntityTypeConfiguration<tblSRExtra>
    {
        public tblSRExtraMap()
        {
            // Primary Key
            this.HasKey(t => t.ExtraId);

            // Properties
            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MachineNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRExtra");
            this.Property(t => t.ExtraId).HasColumnName("ExtraId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.MachineNo).HasColumnName("MachineNo");
            this.Property(t => t.ExtraDate).HasColumnName("ExtraDate");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
