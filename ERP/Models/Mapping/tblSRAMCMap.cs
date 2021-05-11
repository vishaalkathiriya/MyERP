using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRAMCMap : EntityTypeConfiguration<tblSRAMC>
    {
        public tblSRAMCMap()
        {
            // Primary Key
            this.HasKey(t => t.AMCId);

            // Properties
            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRAMC");
            this.Property(t => t.AMCId).HasColumnName("AMCId");
            this.Property(t => t.MachineId).HasColumnName("MachineId");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRMachine)
                .WithMany(t => t.tblSRAMCs)
                .HasForeignKey(d => d.MachineId);

        }
    }
}
