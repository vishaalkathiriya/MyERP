using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDChemicalStorageInspectionLogMap : EntityTypeConfiguration<tblHRDChemicalStorageInspectionLog>
    {
        public tblHRDChemicalStorageInspectionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.CheckedyBy)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Findings)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.RootCause)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.CorrectiveAction)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDChemicalStorageInspectionLog");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.DateOfInspection).HasColumnName("DateOfInspection");
            this.Property(t => t.CheckedyBy).HasColumnName("CheckedyBy");
            this.Property(t => t.Findings).HasColumnName("Findings");
            this.Property(t => t.RootCause).HasColumnName("RootCause");
            this.Property(t => t.CorrectiveAction).HasColumnName("CorrectiveAction");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
