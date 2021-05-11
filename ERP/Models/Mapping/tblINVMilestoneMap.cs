using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVMilestoneMap : EntityTypeConfiguration<tblINVMilestone>
    {
        public tblINVMilestoneMap()
        {
            // Primary Key
            this.HasKey(t => t.PKMilestoneId);

            // Properties
            this.Property(t => t.MilestoneName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MilestoneDesc)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblINVMilestone");
            this.Property(t => t.PKMilestoneId).HasColumnName("PKMilestoneId");
            this.Property(t => t.FKProjectId).HasColumnName("FKProjectId");
            this.Property(t => t.MilestoneName).HasColumnName("MilestoneName");
            this.Property(t => t.MilestoneDesc).HasColumnName("MilestoneDesc");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.TotalHours).HasColumnName("TotalHours");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Currency).HasColumnName("Currency");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVProject)
                .WithMany(t => t.tblINVMilestones)
                .HasForeignKey(d => d.FKProjectId);

        }
    }
}
