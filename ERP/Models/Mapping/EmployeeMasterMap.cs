using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class EmployeeMasterMap : EntityTypeConfiguration<EmployeeMaster>
    {
        public EmployeeMasterMap()
        {
            // Primary Key
            this.HasKey(t => t.ECode);

            // Properties
            this.Property(t => t.ECode)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FormNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MiddleName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GrandFather)
                .HasMaxLength(50);

            this.Property(t => t.Manager)
                .HasMaxLength(10);

            this.Property(t => t.Designation)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MaritalStatus)
                .HasMaxLength(50);

            this.Property(t => t.PANCardNo)
                .HasMaxLength(50);

            this.Property(t => t.Cast)
                .HasMaxLength(50);

            this.Property(t => t.BirthPlace)
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .HasMaxLength(255);

            this.Property(t => t.PhoneNo)
                .HasMaxLength(50);

            this.Property(t => t.MobileNo)
                .HasMaxLength(50);

            this.Property(t => t.Village)
                .HasMaxLength(50);

            this.Property(t => t.County)
                .HasMaxLength(50);

            this.Property(t => t.District)
                .HasMaxLength(50);

            this.Property(t => t.Hobbies)
                .HasMaxLength(50);

            this.Property(t => t.PFNo)
                .HasMaxLength(25);

            this.Property(t => t.ESICNo)
                .HasMaxLength(25);

            this.Property(t => t.PFNomeeneeName)
                .HasMaxLength(100);

            this.Property(t => t.RefName)
                .HasMaxLength(50);

            this.Property(t => t.RefAdd)
                .HasMaxLength(255);

            this.Property(t => t.RefRelation)
                .HasMaxLength(50);

            this.Property(t => t.RefMobileNo)
                .HasMaxLength(50);

            this.Property(t => t.LeaveReason)
                .HasMaxLength(255);

            this.Property(t => t.Gender)
                .HasMaxLength(50);

            this.Property(t => t.BloodGroup)
                .HasMaxLength(50);

            this.Property(t => t.ShoesBoxNo)
                .HasMaxLength(50);

            this.Property(t => t.PassNo)
                .HasMaxLength(50);

            this.Property(t => t.Intercom)
                .HasMaxLength(50);

            this.Property(t => t.LockerNo)
                .HasMaxLength(50);

            this.Property(t => t.Wing)
                .HasMaxLength(50);

            this.Property(t => t.Housing)
                .HasMaxLength(50);

            this.Property(t => t.GName)
                .HasMaxLength(50);

            this.Property(t => t.ManCode)
                .HasMaxLength(50);

            this.Property(t => t.AMCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.WagesType)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Remark)
                .HasMaxLength(100);

            this.Property(t => t.IDProof)
                .HasMaxLength(25);

            this.Property(t => t.ProofType)
                .HasMaxLength(25);

            this.Property(t => t.TCode)
                .HasMaxLength(50);

            this.Property(t => t.ESICNomeeneeName)
                .HasMaxLength(25);

            this.Property(t => t.PersonalEmail)
                .HasMaxLength(50);

            this.Property(t => t.OfficialEmail)
                .HasMaxLength(50);

            this.Property(t => t.ACNO)
                .HasMaxLength(50);

            this.Property(t => t.Shift)
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("EmployeeMaster");
            this.Property(t => t.ECode).HasColumnName("ECode");
            this.Property(t => t.FormNo).HasColumnName("FormNo");
            this.Property(t => t.FormDate).HasColumnName("FormDate");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleName).HasColumnName("MiddleName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.GrandFather).HasColumnName("GrandFather");
            this.Property(t => t.Photo).HasColumnName("Photo");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.JoinDate).HasColumnName("JoinDate");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            this.Property(t => t.PresentPincode).HasColumnName("PresentPincode");
            this.Property(t => t.VillagePincode).HasColumnName("VillagePincode");
            this.Property(t => t.PANCardNo).HasColumnName("PANCardNo");
            this.Property(t => t.Cast).HasColumnName("Cast");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.BirthPlace).HasColumnName("BirthPlace");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.PhoneNo).HasColumnName("PhoneNo");
            this.Property(t => t.MobileNo).HasColumnName("MobileNo");
            this.Property(t => t.Village).HasColumnName("Village");
            this.Property(t => t.County).HasColumnName("County");
            this.Property(t => t.District).HasColumnName("District");
            this.Property(t => t.Hobbies).HasColumnName("Hobbies");
            this.Property(t => t.PFNo).HasColumnName("PFNo");
            this.Property(t => t.ESICNo).HasColumnName("ESICNo");
            this.Property(t => t.PFNomeeneeName).HasColumnName("PFNomeeneeName");
            this.Property(t => t.RefName).HasColumnName("RefName");
            this.Property(t => t.RefAdd).HasColumnName("RefAdd");
            this.Property(t => t.RefRelation).HasColumnName("RefRelation");
            this.Property(t => t.RefMobileNo).HasColumnName("RefMobileNo");
            this.Property(t => t.RefKnownYr).HasColumnName("RefKnownYr");
            this.Property(t => t.LeaveDate).HasColumnName("LeaveDate");
            this.Property(t => t.LeaveReason).HasColumnName("LeaveReason");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            this.Property(t => t.ShoesBoxNo).HasColumnName("ShoesBoxNo");
            this.Property(t => t.PassNo).HasColumnName("PassNo");
            this.Property(t => t.Intercom).HasColumnName("Intercom");
            this.Property(t => t.DressDate).HasColumnName("DressDate");
            this.Property(t => t.LockerNo).HasColumnName("LockerNo");
            this.Property(t => t.Wing).HasColumnName("Wing");
            this.Property(t => t.Housing).HasColumnName("Housing");
            this.Property(t => t.GName).HasColumnName("GName");
            this.Property(t => t.ManCode).HasColumnName("ManCode");
            this.Property(t => t.AMCode).HasColumnName("AMCode");
            this.Property(t => t.WagesType).HasColumnName("WagesType");
            this.Property(t => t.JHrDate).HasColumnName("JHrDate");
            this.Property(t => t.IsFixed).HasColumnName("IsFixed");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Removable).HasColumnName("Removable");
            this.Property(t => t.AnniversaryDate).HasColumnName("AnniversaryDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IDProof).HasColumnName("IDProof");
            this.Property(t => t.ProofType).HasColumnName("ProofType");
            this.Property(t => t.RefIDProofImg).HasColumnName("RefIDProofImg");
            this.Property(t => t.IDProofImg).HasColumnName("IDProofImg");
            this.Property(t => t.TCode).HasColumnName("TCode");
            this.Property(t => t.Wages).HasColumnName("Wages");
            this.Property(t => t.Template).HasColumnName("Template");
            this.Property(t => t.ESICNomeeneeName).HasColumnName("ESICNomeeneeName");
            this.Property(t => t.PersonalEmail).HasColumnName("PersonalEmail");
            this.Property(t => t.OfficialEmail).HasColumnName("OfficialEmail");
            this.Property(t => t.IsPerfect).HasColumnName("IsPerfect");
            this.Property(t => t.ACNO).HasColumnName("ACNO");
            this.Property(t => t.RefECode).HasColumnName("RefECode");
            this.Property(t => t.Shift).HasColumnName("Shift");
            this.Property(t => t.Disapproved).HasColumnName("Disapproved");
            this.Property(t => t.ESign).HasColumnName("ESign");
        }
    }
}
