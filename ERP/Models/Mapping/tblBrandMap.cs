using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblBrandMap : EntityTypeConfiguration<tblBrand>
    {
        public tblBrandMap()
        {
            // Primary Key
            this.HasKey(t => t.BrandId);

            // Properties
            this.Property(t => t.BrandName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblBrands");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.BrandName).HasColumnName("BrandName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
