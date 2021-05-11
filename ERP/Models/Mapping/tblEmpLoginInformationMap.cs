using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpLoginInformationMap : EntityTypeConfiguration<tblEmpLoginInformation>
    {
        public tblEmpLoginInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.LoginInfoId);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEmpLoginInformation");
            this.Property(t => t.LoginInfoId).HasColumnName("LoginInfoId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.PasswordExpiresDays).HasColumnName("PasswordExpiresDays");
            this.Property(t => t.IsRemoteLogin).HasColumnName("IsRemoteLogin");
            this.Property(t => t.IsPermit).HasColumnName("IsPermit");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsLogin).HasColumnName("IsLogin");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpLoginInformations)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
