using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVClientSourceMap : EntityTypeConfiguration<tblINVClientSource>
    {
        public tblINVClientSourceMap()
        {
            // Primary Key
            this.HasKey(t => t.PKSourceId);

            // Properties
            this.Property(t => t.SourceName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblINVClientSource");
            this.Property(t => t.PKSourceId).HasColumnName("PKSourceId");
            this.Property(t => t.SourceName).HasColumnName("SourceName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
