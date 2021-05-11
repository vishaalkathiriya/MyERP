using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSNewsletterMap : EntityTypeConfiguration<tblEMSNewsletter>
    {
        public tblEMSNewsletterMap()
        {
            // Primary Key
            this.HasKey(t => t.NewsletterID);

            // Properties
            this.Property(t => t.FromName)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.FromEmail)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.Too)
                .IsRequired();

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.HTML)
                .IsRequired();

            this.Property(t => t.HeaderAndFooter)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblEMSNewsletter");
            this.Property(t => t.NewsletterID).HasColumnName("NewsletterID");
            this.Property(t => t.FromName).HasColumnName("FromName");
            this.Property(t => t.FromEmail).HasColumnName("FromEmail");
            this.Property(t => t.Too).HasColumnName("Too");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.HTML).HasColumnName("HTML");
            this.Property(t => t.HeaderAndFooter).HasColumnName("HeaderAndFooter");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.NrOpened).HasColumnName("NrOpened");
        }
    }
}
