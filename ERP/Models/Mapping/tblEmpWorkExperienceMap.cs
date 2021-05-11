using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpWorkExperienceMap : EntityTypeConfiguration<tblEmpWorkExperience>
    {
        public tblEmpWorkExperienceMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Designation)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Skills)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Comments)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("tblEmpWorkExperience");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.Skills).HasColumnName("Skills");
            this.Property(t => t.FromDate).HasColumnName("FromDate");
            this.Property(t => t.ToDate).HasColumnName("ToDate");
            this.Property(t => t.Salary).HasColumnName("Salary");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpWorkExperiences)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
