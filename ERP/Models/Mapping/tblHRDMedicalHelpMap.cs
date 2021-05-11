using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDMedicalHelpMap : EntityTypeConfiguration<tblHRDMedicalHelp>
    {
        public tblHRDMedicalHelpMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.ECode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.EmployeeName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.PatientName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Relation)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.HospitalName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.ChequeNumber)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.ReceiverName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.MobileNumber)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ApprovedBy)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDMedicalHelp");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.ECode).HasColumnName("ECode");
            this.Property(t => t.EmployeeName).HasColumnName("EmployeeName");
            this.Property(t => t.PatientName).HasColumnName("PatientName");
            this.Property(t => t.Relation).HasColumnName("Relation");
            this.Property(t => t.HospitalName).HasColumnName("HospitalName");
            this.Property(t => t.ChequeIssueDate).HasColumnName("ChequeIssueDate");
            this.Property(t => t.ChequeNumber).HasColumnName("ChequeNumber");
            this.Property(t => t.ReceiverName).HasColumnName("ReceiverName");
            this.Property(t => t.MobileNumber).HasColumnName("MobileNumber");
            this.Property(t => t.QuotationAmount).HasColumnName("QuotationAmount");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.IsPatelSocialGroup).HasColumnName("IsPatelSocialGroup");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
