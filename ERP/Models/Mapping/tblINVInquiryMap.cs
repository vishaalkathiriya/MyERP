using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVInquiryMap : EntityTypeConfiguration<tblINVInquiry>
    {
        public tblINVInquiryMap()
        {
            // Primary Key
            this.HasKey(t => t.PKInquiryId);

            // Properties
            this.Property(t => t.InquiryCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.InquiryTitle)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FKTechnologyIds)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblINVInquiry");
            this.Property(t => t.PKInquiryId).HasColumnName("PKInquiryId");
            this.Property(t => t.FKClientId).HasColumnName("FKClientId");
            this.Property(t => t.InquiryCode).HasColumnName("InquiryCode");
            this.Property(t => t.InquiryTitle).HasColumnName("InquiryTitle");
            this.Property(t => t.InquiryStatus).HasColumnName("InquiryStatus");
            this.Property(t => t.InquiryDate).HasColumnName("InquiryDate");
            this.Property(t => t.FKTechnologyIds).HasColumnName("FKTechnologyIds");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVClient)
                .WithMany(t => t.tblINVInquiries)
                .HasForeignKey(d => d.FKClientId);

        }
    }
}
