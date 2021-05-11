using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpPersonalInformationMap : EntityTypeConfiguration<tblEmpPersonalInformation>
    {
        public tblEmpPersonalInformationMap()
        {
            // Primary Key
            this.HasKey(t => t.EmployeeId);

            // Properties
            this.Property(t => t.EmployeeRegisterCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.CandidateFirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CandidateMiddleName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CandidateLastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GuardianFirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GuardianMiddleName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GuardianLastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ProfilePhoto)
                .HasMaxLength(50);

            this.Property(t => t.Present_HouseNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Present_Location)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Present_Area)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Present_City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Present_PostalCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Permanent_HouseNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Permanent_Location)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Permanent_Area)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Permanent_City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Permanent_PostalCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.MaritalStatus)
                .HasMaxLength(10);

            this.Property(t => t.Gender)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.DrivingLicenceNumber)
                .HasMaxLength(50);

            this.Property(t => t.PassportNumber)
                .HasMaxLength(50);

            this.Property(t => t.AdharNumber)
                .HasMaxLength(50);

            this.Property(t => t.PANCardNumber)
                .HasMaxLength(50);

            this.Property(t => t.PersonalEmailId)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PersonalMobile)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NomineeMobile)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CompanyEmailId)
                .HasMaxLength(50);

            this.Property(t => t.CompanyMobile)
                .HasMaxLength(50);

            this.Property(t => t.BloodGroup)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblEmpPersonalInformation");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.EmployeeRegisterCode).HasColumnName("EmployeeRegisterCode");
            this.Property(t => t.CandidateFirstName).HasColumnName("CandidateFirstName");
            this.Property(t => t.CandidateMiddleName).HasColumnName("CandidateMiddleName");
            this.Property(t => t.CandidateLastName).HasColumnName("CandidateLastName");
            this.Property(t => t.GuardianFirstName).HasColumnName("GuardianFirstName");
            this.Property(t => t.GuardianMiddleName).HasColumnName("GuardianMiddleName");
            this.Property(t => t.GuardianLastName).HasColumnName("GuardianLastName");
            this.Property(t => t.ProfilePhoto).HasColumnName("ProfilePhoto");
            this.Property(t => t.Present_HouseNo).HasColumnName("Present_HouseNo");
            this.Property(t => t.Present_Location).HasColumnName("Present_Location");
            this.Property(t => t.Present_Area).HasColumnName("Present_Area");
            this.Property(t => t.Present_Country).HasColumnName("Present_Country");
            this.Property(t => t.Present_State).HasColumnName("Present_State");
            this.Property(t => t.Present_City).HasColumnName("Present_City");
            this.Property(t => t.Present_PostalCode).HasColumnName("Present_PostalCode");
            this.Property(t => t.Permanent_HouseNo).HasColumnName("Permanent_HouseNo");
            this.Property(t => t.Permanent_Location).HasColumnName("Permanent_Location");
            this.Property(t => t.Permanent_Area).HasColumnName("Permanent_Area");
            this.Property(t => t.Permanent_Country).HasColumnName("Permanent_Country");
            this.Property(t => t.Permanent_State).HasColumnName("Permanent_State");
            this.Property(t => t.Permanent_City).HasColumnName("Permanent_City");
            this.Property(t => t.Permanent_PostalCode).HasColumnName("Permanent_PostalCode");
            this.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            this.Property(t => t.MarriageAnniversaryDate).HasColumnName("MarriageAnniversaryDate");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.DrivingLicenceNumber).HasColumnName("DrivingLicenceNumber");
            this.Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            this.Property(t => t.PassportExpiryDate).HasColumnName("PassportExpiryDate");
            this.Property(t => t.AdharNumber).HasColumnName("AdharNumber");
            this.Property(t => t.PANCardNumber).HasColumnName("PANCardNumber");
            this.Property(t => t.PersonalEmailId).HasColumnName("PersonalEmailId");
            this.Property(t => t.PersonalMobile).HasColumnName("PersonalMobile");
            this.Property(t => t.NomineeMobile).HasColumnName("NomineeMobile");
            this.Property(t => t.CompanyEmailId).HasColumnName("CompanyEmailId");
            this.Property(t => t.CompanyMobile).HasColumnName("CompanyMobile");
            this.Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
