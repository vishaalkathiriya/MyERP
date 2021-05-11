using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblRoleMap : EntityTypeConfiguration<tblRole>
    {
        public tblRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.RolesId);

            // Properties
            this.Property(t => t.Roles)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblRoles");
            this.Property(t => t.RolesId).HasColumnName("RolesId");
            this.Property(t => t.Roles).HasColumnName("Roles");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
