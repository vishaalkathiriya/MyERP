using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDSocialWelfareExpenseMap : EntityTypeConfiguration<tblHRDSocialWelfareExpense>
    {
        public tblHRDSocialWelfareExpenseMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.ProgrammeName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Venue)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Time)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.GuestName)
                .IsRequired();

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDSocialWelfareExpense");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.ProgrammeName).HasColumnName("ProgrammeName");
            this.Property(t => t.Venue).HasColumnName("Venue");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.ExpenseAmount).HasColumnName("ExpenseAmount");
            this.Property(t => t.GuestName).HasColumnName("GuestName");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
