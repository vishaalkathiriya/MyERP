using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSubCategoryMap : EntityTypeConfiguration<tblSubCategory>
    {
        public tblSubCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.SubCategoryId);

            // Properties
            this.Property(t => t.SubCategoryName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblSubCategories");
            this.Property(t => t.SubCategoryId).HasColumnName("SubCategoryId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.SubCategoryName).HasColumnName("SubCategoryName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblCategory)
                .WithMany(t => t.tblSubCategories)
                .HasForeignKey(d => d.CategoryId);

        }
    }
}
