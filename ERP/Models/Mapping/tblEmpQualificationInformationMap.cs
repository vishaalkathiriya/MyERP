using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpQualificationInformationMap : EntityTypeConfiguration<tblEmpQualificationInformation>
    {
        public tblEmpQualificationInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.Acedamic)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Degree)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Discipline)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.University)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Institute)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEmpQualificationInformation");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.Acedamic).HasColumnName("Acedamic");
            this.Property(t => t.Degree).HasColumnName("Degree");
            this.Property(t => t.Discipline).HasColumnName("Discipline");
            this.Property(t => t.University).HasColumnName("University");
            this.Property(t => t.Institute).HasColumnName("Institute");
            this.Property(t => t.PassingYear).HasColumnName("PassingYear");
            this.Property(t => t.Percentages).HasColumnName("Percentages");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpQualificationInformations)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
