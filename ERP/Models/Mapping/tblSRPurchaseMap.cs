using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRPurchaseMap : EntityTypeConfiguration<tblSRPurchase>
    {
        public tblSRPurchaseMap()
        {
            // Primary Key
            this.HasKey(t => t.PurchaseId);

            // Properties
            this.Property(t => t.Attachment)
                .HasMaxLength(50);

            this.Property(t => t.ApprovedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRPurchase");
            this.Property(t => t.PurchaseId).HasColumnName("PurchaseId");
            this.Property(t => t.PartId).HasColumnName("PartId");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRPart)
                .WithMany(t => t.tblSRPurchases)
                .HasForeignKey(d => d.PartId);

        }
    }
}
