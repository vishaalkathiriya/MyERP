using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVProposalMap : EntityTypeConfiguration<tblINVProposal>
    {
        public tblINVProposalMap()
        {
            // Primary Key
            this.HasKey(t => t.PKProposalId);

            // Properties
            this.Property(t => t.ProposalTitle)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblINVProposal");
            this.Property(t => t.PKProposalId).HasColumnName("PKProposalId");
            this.Property(t => t.FKInquiryId).HasColumnName("FKInquiryId");
            this.Property(t => t.ProposalTitle).HasColumnName("ProposalTitle");
            this.Property(t => t.ProposalDate).HasColumnName("ProposalDate");
            this.Property(t => t.IsFinalized).HasColumnName("IsFinalized");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVInquiry)
                .WithMany(t => t.tblINVProposals)
                .HasForeignKey(d => d.FKInquiryId);

        }
    }
}
