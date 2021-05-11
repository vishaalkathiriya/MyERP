using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblCountryMap : EntityTypeConfiguration<tblCountry>
    {
        public tblCountryMap()
        {
            // Primary Key
            this.HasKey(t => t.CountryId);

            // Properties
            this.Property(t => t.CountryName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CountryCode)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.DialCode)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("tblCountries");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.CountryName).HasColumnName("CountryName");
            this.Property(t => t.CountryCode).HasColumnName("CountryCode");
            this.Property(t => t.DialCode).HasColumnName("DialCode");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
