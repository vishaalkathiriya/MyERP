using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDSafetyTrainingRecordMap : EntityTypeConfiguration<tblHRDSafetyTrainingRecord>
    {
        public tblHRDSafetyTrainingRecordMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.SubjectOfTraining)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ManagerName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TrainersName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDSafetyTrainingRecords");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.SubjectOfTraining).HasColumnName("SubjectOfTraining");
            this.Property(t => t.DateOfTraining).HasColumnName("DateOfTraining");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.ManagerName).HasColumnName("ManagerName");
            this.Property(t => t.NoOfParticipants).HasColumnName("NoOfParticipants");
            this.Property(t => t.TrainersName).HasColumnName("TrainersName");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
