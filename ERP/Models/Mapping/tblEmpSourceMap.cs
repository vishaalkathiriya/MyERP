using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpSourceMap : EntityTypeConfiguration<tblEmpSource>
    {
        public tblEmpSourceMap()
        {
            // Primary Key
            this.HasKey(t => t.SourceId);

            // Properties
            this.Property(t => t.SourceName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpSource");
            this.Property(t => t.SourceId).HasColumnName("SourceId");
            this.Property(t => t.SourceName).HasColumnName("SourceName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
