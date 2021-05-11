using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRFloorMap : EntityTypeConfiguration<tblSRFloor>
    {
        public tblSRFloorMap()
        {
            // Primary Key
            this.HasKey(t => t.FloorId);

            // Properties
            this.Property(t => t.Manager)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRFloor");
            this.Property(t => t.FloorId).HasColumnName("FloorId");
            this.Property(t => t.MachineId).HasColumnName("MachineId");
            this.Property(t => t.LocationId).HasColumnName("LocationId");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRMachine)
                .WithMany(t => t.tblSRFloors)
                .HasForeignKey(d => d.MachineId);

        }
    }
}
