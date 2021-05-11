using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDTrainingsAndMeetingMap : EntityTypeConfiguration<tblHRDTrainingsAndMeeting>
    {
        public tblHRDTrainingsAndMeetingMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Manager)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Intercom)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Attachment)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDTrainingsAndMeetings");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.NoOfParticipant).HasColumnName("NoOfParticipant");
            this.Property(t => t.Intercom).HasColumnName("Intercom");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
