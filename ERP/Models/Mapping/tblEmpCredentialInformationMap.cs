using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpCredentialInformationMap : EntityTypeConfiguration<tblEmpCredentialInformation>
    {
        public tblEmpCredentialInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EmailId)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SecurityQuestion1)
                .HasMaxLength(255);

            this.Property(t => t.SecurityAnswer1)
                .HasMaxLength(50);

            this.Property(t => t.SecurityQuestion2)
                .HasMaxLength(255);

            this.Property(t => t.SecurityAnswer2)
                .HasMaxLength(50);

            this.Property(t => t.Comments)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("tblEmpCredentialInformation");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.SourceId).HasColumnName("SourceId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.SecurityQuestion1).HasColumnName("SecurityQuestion1");
            this.Property(t => t.SecurityAnswer1).HasColumnName("SecurityAnswer1");
            this.Property(t => t.SecurityQuestion2).HasColumnName("SecurityQuestion2");
            this.Property(t => t.SecurityAnswer2).HasColumnName("SecurityAnswer2");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpCredentialInformations)
                .HasForeignKey(d => d.EmployeeId);
            this.HasRequired(t => t.tblEmpSource)
                .WithMany(t => t.tblEmpCredentialInformations)
                .HasForeignKey(d => d.SourceId);

        }
    }
}
