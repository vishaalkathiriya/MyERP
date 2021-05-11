using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblInvetoryDetailMap : EntityTypeConfiguration<tblInvetoryDetail>
    {
        public tblInvetoryDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.SerialNumber)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("tblInvetoryDetails");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.InventoryId).HasColumnName("InventoryId");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.SubCategoryId).HasColumnName("SubCategoryId");
            this.Property(t => t.SerialNumber).HasColumnName("SerialNumber");
            this.Property(t => t.IsAvailable).HasColumnName("IsAvailable");
            this.Property(t => t.IsScrap).HasColumnName("IsScrap");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblBrand)
                .WithMany(t => t.tblInvetoryDetails)
                .HasForeignKey(d => d.BrandId);
            this.HasRequired(t => t.tblCategory)
                .WithMany(t => t.tblInvetoryDetails)
                .HasForeignKey(d => d.CategoryId);
            this.HasRequired(t => t.tblInvetory)
                .WithMany(t => t.tblInvetoryDetails)
                .HasForeignKey(d => d.InventoryId);
            this.HasRequired(t => t.tblSubCategory)
                .WithMany(t => t.tblInvetoryDetails)
                .HasForeignKey(d => d.SubCategoryId);

        }
    }
}
