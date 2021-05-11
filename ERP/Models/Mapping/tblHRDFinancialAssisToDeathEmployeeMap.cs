using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDFinancialAssisToDeathEmployeeMap : EntityTypeConfiguration<tblHRDFinancialAssisToDeathEmployee>
    {
        public tblHRDFinancialAssisToDeathEmployeeMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.Ecode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.EmployeeName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.ChequeNumber)
                .HasMaxLength(30);

            this.Property(t => t.ReceiveBy)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Relation)
                .HasMaxLength(250);

            this.Property(t => t.FamilyBackgroundDetail)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblHRDFinancialAssisToDeathEmployees");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Ecode).HasColumnName("Ecode");
            this.Property(t => t.EmployeeName).HasColumnName("EmployeeName");
            this.Property(t => t.DateOfDeath).HasColumnName("DateOfDeath");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ChequeNumber).HasColumnName("ChequeNumber");
            this.Property(t => t.ChequeIssueDate).HasColumnName("ChequeIssueDate");
            this.Property(t => t.ReceiveBy).HasColumnName("ReceiveBy");
            this.Property(t => t.Relation).HasColumnName("Relation");
            this.Property(t => t.FamilyBackgroundDetail).HasColumnName("FamilyBackgroundDetail");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
