using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblLOGPageActivityMap : EntityTypeConfiguration<tblLOGPageActivity>
    {
        public tblLOGPageActivityMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.IPAddress)
                .HasMaxLength(20);

            this.Property(t => t.Url)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblLOGPageActivity");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.IPAddress).HasColumnName("IPAddress");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.VisitDate).HasColumnName("VisitDate");
        }
    }
}
