using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDQuarterlyManagementMeetingMap : EntityTypeConfiguration<tblHRDQuarterlyManagementMeeting>
    {
        public tblHRDQuarterlyManagementMeetingMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.ListOfParticipants)
                .IsRequired();

            this.Property(t => t.AgendaOfTraining)
                .IsRequired();

            this.Property(t => t.DecisionTakenDuringMeeting)
                .IsRequired();

            this.Property(t => t.Attachment)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDQuarterlyManagementMeeting");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.DateOfMeeting).HasColumnName("DateOfMeeting");
            this.Property(t => t.ListOfParticipants).HasColumnName("ListOfParticipants");
            this.Property(t => t.AgendaOfTraining).HasColumnName("AgendaOfTraining");
            this.Property(t => t.DecisionTakenDuringMeeting).HasColumnName("DecisionTakenDuringMeeting");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
