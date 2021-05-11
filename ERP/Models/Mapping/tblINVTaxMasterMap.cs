using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVTaxMasterMap : EntityTypeConfiguration<tblINVTaxMaster>
    {
        public tblINVTaxMasterMap()
        {
            // Primary Key
            this.HasKey(t => t.PKTaxId);

            // Properties
            this.Property(t => t.TaxTypeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Mode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("tblINVTaxMaster");
            this.Property(t => t.PKTaxId).HasColumnName("PKTaxId");
            this.Property(t => t.TaxTypeName).HasColumnName("TaxTypeName");
            this.Property(t => t.Mode).HasColumnName("Mode");
            this.Property(t => t.Percentage).HasColumnName("Percentage");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
