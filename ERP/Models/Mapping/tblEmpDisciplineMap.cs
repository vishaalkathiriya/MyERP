using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpDisciplineMap : EntityTypeConfiguration<tblEmpDiscipline>
    {
        public tblEmpDisciplineMap()
        {
            // Primary Key
            this.HasKey(t => t.DisciplineId);

            // Properties
            this.Property(t => t.DisciplineName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpDiscipline");
            this.Property(t => t.DisciplineId).HasColumnName("DisciplineId");
            this.Property(t => t.DegreeId).HasColumnName("DegreeId");
            this.Property(t => t.DisciplineName).HasColumnName("DisciplineName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblEmpDegree)
                .WithMany(t => t.tblEmpDisciplines)
                .HasForeignKey(d => d.DegreeId);

        }
    }
}
