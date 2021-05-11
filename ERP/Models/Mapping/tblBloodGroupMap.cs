using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblBloodGroupMap : EntityTypeConfiguration<tblBloodGroup>
    {
        public tblBloodGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.BloodGroupId);

            // Properties
            this.Property(t => t.BloodGroupName)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblBloodGroup");
            this.Property(t => t.BloodGroupId).HasColumnName("BloodGroupId");
            this.Property(t => t.BloodGroupName).HasColumnName("BloodGroupName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
