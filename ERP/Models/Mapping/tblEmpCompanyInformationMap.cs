using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpCompanyInformationMap : EntityTypeConfiguration<tblEmpCompanyInformation>
    {
        public tblEmpCompanyInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.CompanyId);

            // Properties
            this.Property(t => t.IncrementCycle)
                .IsRequired()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("tblEmpCompanyInformation");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.TeamId).HasColumnName("TeamId");
            this.Property(t => t.ReportingTo).HasColumnName("ReportingTo");
            this.Property(t => t.DesignationId).HasColumnName("DesignationId");
            this.Property(t => t.RolesId).HasColumnName("RolesId");
            this.Property(t => t.IncrementCycle).HasColumnName("IncrementCycle");
            this.Property(t => t.IsTL).HasColumnName("IsTL");
            this.Property(t => t.IsBillable).HasColumnName("IsBillable");
            this.Property(t => t.ModuleUser).HasColumnName("ModuleUser");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblDesignation)
                .WithMany(t => t.tblEmpCompanyInformations)
                .HasForeignKey(d => d.DesignationId);
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpCompanyInformations)
                .HasForeignKey(d => d.EmployeeId);
            this.HasRequired(t => t.tblRole)
                .WithMany(t => t.tblEmpCompanyInformations)
                .HasForeignKey(d => d.RolesId);

        }
    }
}
