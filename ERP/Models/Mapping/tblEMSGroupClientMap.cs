using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSGroupClientMap : EntityTypeConfiguration<tblEMSGroupClient>
    {
        public tblEMSGroupClientMap()
        {
            // Primary Key
            this.HasKey(t => t.RecordID);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblEMSGroupClient");
            this.Property(t => t.RecordID).HasColumnName("RecordID");
            this.Property(t => t.ClientGroupID).HasColumnName("ClientGroupID");
            this.Property(t => t.ClientID).HasColumnName("ClientID");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
