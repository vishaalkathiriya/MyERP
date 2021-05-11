using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVInvoiceTaxMap : EntityTypeConfiguration<tblINVInvoiceTax>
    {
        public tblINVInvoiceTaxMap()
        {
            // Primary Key
            this.HasKey(t => t.PKInvoiceTaxId);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblINVInvoiceTax");
            this.Property(t => t.PKInvoiceTaxId).HasColumnName("PKInvoiceTaxId");
            this.Property(t => t.FKInvoiceId).HasColumnName("FKInvoiceId");
            this.Property(t => t.FKTaxId).HasColumnName("FKTaxId");
            this.Property(t => t.TaxPercentage).HasColumnName("TaxPercentage");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVInvoice)
                .WithMany(t => t.tblINVInvoiceTaxes)
                .HasForeignKey(d => d.FKInvoiceId);
            this.HasRequired(t => t.tblINVTaxMaster)
                .WithMany(t => t.tblINVInvoiceTaxes)
                .HasForeignKey(d => d.FKTaxId);

        }
    }
}
