using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRPartIssueMap : EntityTypeConfiguration<tblSRPartIssue>
    {
        public tblSRPartIssueMap()
        {
            // Primary Key
            this.HasKey(t => t.PartIssueId);

            // Properties
            this.Property(t => t.IssuedFrom)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ChallanNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Problem)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRPartIssue");
            this.Property(t => t.PartIssueId).HasColumnName("PartIssueId");
            this.Property(t => t.MachineId).HasColumnName("MachineId");
            this.Property(t => t.PartId).HasColumnName("PartId");
            this.Property(t => t.IssuedFrom).HasColumnName("IssuedFrom");
            this.Property(t => t.ChallanNo).HasColumnName("ChallanNo");
            this.Property(t => t.IssuedDate).HasColumnName("IssuedDate");
            this.Property(t => t.Problem).HasColumnName("Problem");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRMachine)
                .WithMany(t => t.tblSRPartIssues)
                .HasForeignKey(d => d.MachineId);
            this.HasRequired(t => t.tblSRPart)
                .WithMany(t => t.tblSRPartIssues)
                .HasForeignKey(d => d.PartId);

        }
    }
}
