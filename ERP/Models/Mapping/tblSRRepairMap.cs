using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRRepairMap : EntityTypeConfiguration<tblSRRepair>
    {
        public tblSRRepairMap()
        {
            // Primary Key
            this.HasKey(t => t.RepairId);

            // Properties
            this.Property(t => t.Problem)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.RepairedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Others)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRRepair");
            this.Property(t => t.RepairId).HasColumnName("RepairId");
            this.Property(t => t.MachineId).HasColumnName("MachineId");
            this.Property(t => t.PartId).HasColumnName("PartId");
            this.Property(t => t.Problem).HasColumnName("Problem");
            this.Property(t => t.RepairedBy).HasColumnName("RepairedBy");
            this.Property(t => t.Others).HasColumnName("Others");
            this.Property(t => t.IssueDate).HasColumnName("IssueDate");
            this.Property(t => t.ReceiveDate).HasColumnName("ReceiveDate");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRMachine)
                .WithMany(t => t.tblSRRepairs)
                .HasForeignKey(d => d.MachineId);
            this.HasRequired(t => t.tblSRPart)
                .WithMany(t => t.tblSRRepairs)
                .HasForeignKey(d => d.PartId);

        }
    }
}
