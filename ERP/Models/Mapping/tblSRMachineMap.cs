using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRMachineMap : EntityTypeConfiguration<tblSRMachine>
    {
        public tblSRMachineMap()
        {
            // Primary Key
            this.HasKey(t => t.MachineId);

            // Properties
            this.Property(t => t.MachineName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SerialNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRMachine");
            this.Property(t => t.MachineId).HasColumnName("MachineId");
            this.Property(t => t.MachineName).HasColumnName("MachineName");
            this.Property(t => t.SerialNo).HasColumnName("SerialNo");
            this.Property(t => t.InstallationDate).HasColumnName("InstallationDate");
            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.SubTypeId).HasColumnName("SubTypeId");
            this.Property(t => t.ParameterId).HasColumnName("ParameterId");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
