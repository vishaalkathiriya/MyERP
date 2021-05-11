using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpPayRollInformationMap : EntityTypeConfiguration<tblEmpPayRollInformation>
    {
        public tblEmpPayRollInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.PayRollId);

            // Properties
            this.Property(t => t.PFAccountNumber)
                .HasMaxLength(50);

            this.Property(t => t.CompanyBankName)
                .HasMaxLength(50);

            this.Property(t => t.CompanyBankAccount)
                .HasMaxLength(50);

            this.Property(t => t.PersonalBankName)
                .HasMaxLength(50);

            this.Property(t => t.PersonalBankAccount)
                .HasMaxLength(50);

            this.Property(t => t.AllocatedPassNo)
                .HasMaxLength(50);

            this.Property(t => t.EmploymentStatus)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.GName)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SalaryBasedOn)
                .IsRequired()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("tblEmpPayRollInformation");
            this.Property(t => t.PayRollId).HasColumnName("PayRollId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.CTC).HasColumnName("CTC");
            this.Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            this.Property(t => t.EmploymentTax).HasColumnName("EmploymentTax");
            this.Property(t => t.ESIC).HasColumnName("ESIC");
            this.Property(t => t.LeavesAllowedPerYear).HasColumnName("LeavesAllowedPerYear");
            this.Property(t => t.PFAccountNumber).HasColumnName("PFAccountNumber");
            this.Property(t => t.PF).HasColumnName("PF");
            this.Property(t => t.CompanyBankName).HasColumnName("CompanyBankName");
            this.Property(t => t.CompanyBankAccount).HasColumnName("CompanyBankAccount");
            this.Property(t => t.PersonalBankName).HasColumnName("PersonalBankName");
            this.Property(t => t.PersonalBankAccount).HasColumnName("PersonalBankAccount");
            this.Property(t => t.AllocatedPassNo).HasColumnName("AllocatedPassNo");
            this.Property(t => t.JoiningDate).HasColumnName("JoiningDate");
            this.Property(t => t.ReLeavingDate).HasColumnName("ReLeavingDate");
            this.Property(t => t.EmploymentStatus).HasColumnName("EmploymentStatus");
            this.Property(t => t.PermanentFromDate).HasColumnName("PermanentFromDate");
            this.Property(t => t.GName).HasColumnName("GName");
            this.Property(t => t.SalaryBasedOn).HasColumnName("SalaryBasedOn");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
