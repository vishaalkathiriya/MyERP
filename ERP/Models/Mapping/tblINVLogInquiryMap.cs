using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVLogInquiryMap : EntityTypeConfiguration<tblINVLogInquiry>
    {
        public tblINVLogInquiryMap()
        {
            // Primary Key
            this.HasKey(t => t.PKLogId);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblINVLogInquiry");
            this.Property(t => t.PKLogId).HasColumnName("PKLogId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
        }
    }
}
