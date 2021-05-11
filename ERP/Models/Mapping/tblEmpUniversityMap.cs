using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpUniversityMap : EntityTypeConfiguration<tblEmpUniversity>
    {
        public tblEmpUniversityMap()
        {
            // Primary Key
            this.HasKey(t => t.UniversityId);

            // Properties
            this.Property(t => t.UniversityName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEmpUniversity");
            this.Property(t => t.UniversityId).HasColumnName("UniversityId");
            this.Property(t => t.UniversityName).HasColumnName("UniversityName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
