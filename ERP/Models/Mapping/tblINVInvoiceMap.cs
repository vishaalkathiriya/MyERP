using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVInvoiceMap : EntityTypeConfiguration<tblINVInvoice>
    {
        public tblINVInvoiceMap()
        {
            // Primary Key
            this.HasKey(t => t.PKInvoiceId);

            // Properties
            this.Property(t => t.MilestoneIds)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.InvoiceCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.InvoiceType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Remarks)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("tblINVInvoice");
            this.Property(t => t.PKInvoiceId).HasColumnName("PKInvoiceId");
            this.Property(t => t.FKClientId).HasColumnName("FKClientId");
            this.Property(t => t.MilestoneIds).HasColumnName("MilestoneIds");
            this.Property(t => t.InvoiceCode).HasColumnName("InvoiceCode");
            this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
            this.Property(t => t.InvoiceType).HasColumnName("InvoiceType");
            this.Property(t => t.Currency).HasColumnName("Currency");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.RoundOff).HasColumnName("RoundOff");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVClient)
                .WithMany(t => t.tblINVInvoices)
                .HasForeignKey(d => d.FKClientId);

        }
    }
}
