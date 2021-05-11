using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSMailSendTempMap : EntityTypeConfiguration<tblEMSMailSendTemp>
    {
        public tblEMSMailSendTempMap()
        {
            // Primary Key
            this.HasKey(t => t.MailID);

            // Properties
            this.Property(t => t.FromName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.FromEmail)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ToName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.ToEmail)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.Body)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblEMSMailSendTemp");
            this.Property(t => t.MailID).HasColumnName("MailID");
            this.Property(t => t.NewsletterID).HasColumnName("NewsletterID");
            this.Property(t => t.ClientID).HasColumnName("ClientID");
            this.Property(t => t.FromName).HasColumnName("FromName");
            this.Property(t => t.FromEmail).HasColumnName("FromEmail");
            this.Property(t => t.ToName).HasColumnName("ToName");
            this.Property(t => t.ToEmail).HasColumnName("ToEmail");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Body).HasColumnName("Body");
            this.Property(t => t.DatePickup).HasColumnName("DatePickup");
            this.Property(t => t.DateAdded).HasColumnName("DateAdded");
        }
    }
}
