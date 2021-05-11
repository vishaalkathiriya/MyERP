using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpInstituteMap : EntityTypeConfiguration<tblEmpInstitute>
    {
        public tblEmpInstituteMap()
        {
            // Primary Key
            this.HasKey(t => t.InstituteId);

            // Properties
            this.Property(t => t.InstituteName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEmpInstitute");
            this.Property(t => t.InstituteId).HasColumnName("InstituteId");
            this.Property(t => t.UniversityId).HasColumnName("UniversityId");
            this.Property(t => t.InstituteName).HasColumnName("InstituteName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblEmpUniversity)
                .WithMany(t => t.tblEmpInstitutes)
                .HasForeignKey(d => d.UniversityId);

        }
    }
}
