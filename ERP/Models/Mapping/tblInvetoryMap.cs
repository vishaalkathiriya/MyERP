using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblInvetoryMap : EntityTypeConfiguration<tblInvetory>
    {
        public tblInvetoryMap()
        {
            // Primary Key
            this.HasKey(t => t.InventoryId);

            // Properties
            this.Property(t => t.InventoryName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IssueTo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SerialNumber)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("tblInvetories");
            this.Property(t => t.InventoryId).HasColumnName("InventoryId");
            this.Property(t => t.InventoryName).HasColumnName("InventoryName");
            this.Property(t => t.VendorId).HasColumnName("VendorId");
            this.Property(t => t.IssueTo).HasColumnName("IssueTo");
            this.Property(t => t.LocationId).HasColumnName("LocationId");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.SubCategoryId).HasColumnName("SubCategoryId");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.SerialNumber).HasColumnName("SerialNumber");
            this.Property(t => t.IsAvailable).HasColumnName("IsAvailable");
            this.Property(t => t.IsScrap).HasColumnName("IsScrap");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblBrand)
                .WithMany(t => t.tblInvetories)
                .HasForeignKey(d => d.BrandId);
            this.HasRequired(t => t.tblCategory)
                .WithMany(t => t.tblInvetories)
                .HasForeignKey(d => d.CategoryId);
            this.HasRequired(t => t.tblLocation)
                .WithMany(t => t.tblInvetories)
                .HasForeignKey(d => d.LocationId);
            this.HasRequired(t => t.tblSubCategory)
                .WithMany(t => t.tblInvetories)
                .HasForeignKey(d => d.SubCategoryId);
            this.HasRequired(t => t.tblVendor)
                .WithMany(t => t.tblInvetories)
                .HasForeignKey(d => d.VendorId);

        }
    }
}
