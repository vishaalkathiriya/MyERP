using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDAccidentRecordMap : EntityTypeConfiguration<tblHRDAccidentRecord>
    {
        public tblHRDAccidentRecordMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.TypeOfAccident)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ManagerName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameOfInjuredPerson)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.RootCauseOfAccident)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.CorrectiveActionTaken)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.NameOfHospital)
                .HasMaxLength(100);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDAccidentRecords");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.TypeOfAccident).HasColumnName("TypeOfAccident");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.ManagerName).HasColumnName("ManagerName");
            this.Property(t => t.NameOfInjuredPerson).HasColumnName("NameOfInjuredPerson");
            this.Property(t => t.RootCauseOfAccident).HasColumnName("RootCauseOfAccident");
            this.Property(t => t.NoOfCasualities).HasColumnName("NoOfCasualities");
            this.Property(t => t.CorrectiveActionTaken).HasColumnName("CorrectiveActionTaken");
            this.Property(t => t.Hospitalized).HasColumnName("Hospitalized");
            this.Property(t => t.NameOfHospital).HasColumnName("NameOfHospital");
            this.Property(t => t.TreatmentExpenses).HasColumnName("TreatmentExpenses");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
