using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblARPermissionMap : EntityTypeConfiguration<tblARPermission>
    {
        public tblARPermissionMap()
        {
            // Primary Key
            this.HasKey(t => t.PermissionId);

            // Properties
            this.Property(t => t.PermissionName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblARPermission");
            this.Property(t => t.PermissionId).HasColumnName("PermissionId");
            this.Property(t => t.PermissionName).HasColumnName("PermissionName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
