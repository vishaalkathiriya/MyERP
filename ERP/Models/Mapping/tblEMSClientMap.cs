using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSClientMap : EntityTypeConfiguration<tblEMSClient>
    {
        public tblEMSClientMap()
        {
            // Primary Key
            this.HasKey(t => t.ClientID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEMSClient");
            this.Property(t => t.ClientID).HasColumnName("ClientID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
        }
    }
}
