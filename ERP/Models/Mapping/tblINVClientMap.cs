using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVClientMap : EntityTypeConfiguration<tblINVClient>
    {
        public tblINVClientMap()
        {
            // Primary Key
            this.HasKey(t => t.PKClientId);

            // Properties
            this.Property(t => t.ClientCode)
                .HasMaxLength(10);

            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CPrefix)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.ContactPerson)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MobileNo)
                .HasMaxLength(13);

            this.Property(t => t.CompanyAddress)
                .HasMaxLength(500);

            this.Property(t => t.City)
                .HasMaxLength(50);

            this.Property(t => t.PostalCode)
                .HasMaxLength(10);

            this.Property(t => t.TelephoneNo)
                .HasMaxLength(13);

            this.Property(t => t.FaxNo)
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Website)
                .HasMaxLength(50);

            this.Property(t => t.LicenseNo)
                .HasMaxLength(50);

            this.Property(t => t.IncomeTaxNo)
                .HasMaxLength(50);

            this.Property(t => t.VATNo)
                .HasMaxLength(50);

            this.Property(t => t.BankName)
                .HasMaxLength(50);

            this.Property(t => t.BranchAddress)
                .HasMaxLength(500);

            this.Property(t => t.BankType)
                .HasMaxLength(50);

            this.Property(t => t.AccountNo)
                .HasMaxLength(50);

            this.Property(t => t.IBANNumber)
                .HasMaxLength(50);

            this.Property(t => t.SwiftCode)
                .HasMaxLength(50);

            this.Property(t => t.URLKey)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblINVClient");
            this.Property(t => t.PKClientId).HasColumnName("PKClientId");
            this.Property(t => t.ClientCode).HasColumnName("ClientCode");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.CPrefix).HasColumnName("CPrefix");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.MobileNo).HasColumnName("MobileNo");
            this.Property(t => t.CompanyAddress).HasColumnName("CompanyAddress");
            this.Property(t => t.FKSourceId).HasColumnName("FKSourceId");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.StateId).HasColumnName("StateId");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.TelephoneNo).HasColumnName("TelephoneNo");
            this.Property(t => t.FaxNo).HasColumnName("FaxNo");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.BusinessTypeId).HasColumnName("BusinessTypeId");
            this.Property(t => t.BusinessStartDate).HasColumnName("BusinessStartDate");
            this.Property(t => t.LicenseNo).HasColumnName("LicenseNo");
            this.Property(t => t.IncomeTaxNo).HasColumnName("IncomeTaxNo");
            this.Property(t => t.VATNo).HasColumnName("VATNo");
            this.Property(t => t.BankName).HasColumnName("BankName");
            this.Property(t => t.BranchAddress).HasColumnName("BranchAddress");
            this.Property(t => t.BankType).HasColumnName("BankType");
            this.Property(t => t.AccountNo).HasColumnName("AccountNo");
            this.Property(t => t.IBANNumber).HasColumnName("IBANNumber");
            this.Property(t => t.SwiftCode).HasColumnName("SwiftCode");
            this.Property(t => t.IsConfirmed).HasColumnName("IsConfirmed");
            this.Property(t => t.IsKYCApproved).HasColumnName("IsKYCApproved");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.URLKey).HasColumnName("URLKey");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
