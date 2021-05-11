using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblARPermissionAssignedMap : EntityTypeConfiguration<tblARPermissionAssigned>
    {
        public tblARPermissionAssignedMap()
        {
            // Primary Key
            this.HasKey(t => t.PAssignedId);

            // Properties
            this.Property(t => t.Permission)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblARPermissionAssigned");
            this.Property(t => t.PAssignedId).HasColumnName("PAssignedId");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.SubModuleId).HasColumnName("SubModuleId");
            this.Property(t => t.Permission).HasColumnName("Permission");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblARModule)
                .WithMany(t => t.tblARPermissionAssigneds)
                .HasForeignKey(d => d.ModuleId);
            this.HasRequired(t => t.tblARSubModule)
                .WithMany(t => t.tblARPermissionAssigneds)
                .HasForeignKey(d => d.SubModuleId);
            this.HasRequired(t => t.tblRole)
                .WithMany(t => t.tblARPermissionAssigneds)
                .HasForeignKey(d => d.RoleId);

        }
    }
}
