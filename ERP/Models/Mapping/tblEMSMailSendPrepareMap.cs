using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSMailSendPrepareMap : EntityTypeConfiguration<tblEMSMailSendPrepare>
    {
        public tblEMSMailSendPrepareMap()
        {
            // Primary Key
            this.HasKey(t => t.RecordID);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblEMSMailSendPrepare");
            this.Property(t => t.RecordID).HasColumnName("RecordID");
            this.Property(t => t.NewsletterID).HasColumnName("NewsletterID");
            this.Property(t => t.ClientGroupID).HasColumnName("ClientGroupID");
            this.Property(t => t.ClientID).HasColumnName("ClientID");
            this.Property(t => t.DatePickup).HasColumnName("DatePickup");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
