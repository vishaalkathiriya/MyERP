using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVPaymentMap : EntityTypeConfiguration<tblINVPayment>
    {
        public tblINVPaymentMap()
        {
            // Primary Key
            this.HasKey(t => t.PKPaymentId);

            // Properties
            this.Property(t => t.Remarks)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("tblINVPayment");
            this.Property(t => t.PKPaymentId).HasColumnName("PKPaymentId");
            this.Property(t => t.FKInvoiceId).HasColumnName("FKInvoiceId");
            this.Property(t => t.PaymentReceivedDate).HasColumnName("PaymentReceivedDate");
            this.Property(t => t.OnHandReceivedAmount).HasColumnName("OnHandReceivedAmount");
            this.Property(t => t.OtherCharges).HasColumnName("OtherCharges");
            this.Property(t => t.ExchangeRateINR).HasColumnName("ExchangeRateINR");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVInvoice)
                .WithMany(t => t.tblINVPayments)
                .HasForeignKey(d => d.FKInvoiceId);

        }
    }
}
