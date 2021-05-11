using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpDegreeMap : EntityTypeConfiguration<tblEmpDegree>
    {
        public tblEmpDegreeMap()
        {
            // Primary Key
            this.HasKey(t => t.DegreeId);

            // Properties
            this.Property(t => t.DegreeName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpDegree");
            this.Property(t => t.DegreeId).HasColumnName("DegreeId");
            this.Property(t => t.AcedamicStatusId).HasColumnName("AcedamicStatusId");
            this.Property(t => t.DegreeName).HasColumnName("DegreeName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblEmpAcedamicStatu)
                .WithMany(t => t.tblEmpDegrees)
                .HasForeignKey(d => d.AcedamicStatusId);

        }
    }
}
