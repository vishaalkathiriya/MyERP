using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblVendorMap : EntityTypeConfiguration<tblVendor>
    {
        public tblVendorMap()
        {
            // Primary Key
            this.HasKey(t => t.VendorId);

            // Properties
            this.Property(t => t.VendorName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Website)
                .HasMaxLength(50);

            this.Property(t => t.Mobile)
                .HasMaxLength(20);

            this.Property(t => t.PhoneNo)
                .HasMaxLength(20);

            this.Property(t => t.Services)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.HouseNo)
                .HasMaxLength(50);

            this.Property(t => t.Location)
                .HasMaxLength(50);

            this.Property(t => t.Area)
                .HasMaxLength(50);

            this.Property(t => t.Country)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.State)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PostalCode)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblVendors");
            this.Property(t => t.VendorId).HasColumnName("VendorId");
            this.Property(t => t.VendorName).HasColumnName("VendorName");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.PhoneNo).HasColumnName("PhoneNo");
            this.Property(t => t.Services).HasColumnName("Services");
            this.Property(t => t.Rating).HasColumnName("Rating");
            this.Property(t => t.HouseNo).HasColumnName("HouseNo");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.Area).HasColumnName("Area");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
