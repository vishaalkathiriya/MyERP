using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpRelativeInformationMap : EntityTypeConfiguration<tblEmpRelativeInformation>
    {
        public tblEmpRelativeInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.RelativeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RelativeRelation)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Acedamic)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Degree)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TypeOfWork)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpRelativeInformation");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.RelativeName).HasColumnName("RelativeName");
            this.Property(t => t.RelativeRelation).HasColumnName("RelativeRelation");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Acedamic).HasColumnName("Acedamic");
            this.Property(t => t.Degree).HasColumnName("Degree");
            this.Property(t => t.TypeOfWork).HasColumnName("TypeOfWork");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpRelativeInformations)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
