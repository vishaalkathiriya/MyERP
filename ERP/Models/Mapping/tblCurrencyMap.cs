using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblCurrencyMap : EntityTypeConfiguration<tblCurrency>
    {
        public tblCurrencyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CurrencyName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CurrencyCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Remark)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblCurrency");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CurrencyName).HasColumnName("CurrencyName");
            this.Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblCountry)
                .WithMany(t => t.tblCurrencies)
                .HasForeignKey(d => d.CountryId);

        }
    }
}
