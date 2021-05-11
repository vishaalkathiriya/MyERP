using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpPersonalInformation
    {
        public tblEmpPersonalInformation()
        {
            this.tblApplyLeaves = new List<tblApplyLeave>();
            this.tblEmpCompanyInformations = new List<tblEmpCompanyInformation>();
            this.tblEmpCredentialInformations = new List<tblEmpCredentialInformation>();
            this.tblEmpDailyInOuts = new List<tblEmpDailyInOut>();
            this.tblEmpDailyInOuts1 = new List<tblEmpDailyInOut>();
            this.tblEmpDocuments = new List<tblEmpDocument>();
            this.tblEmpLoginInformations = new List<tblEmpLoginInformation>();
            this.tblEmpQualificationInformations = new List<tblEmpQualificationInformation>();
            this.tblEmpRelativeInformations = new List<tblEmpRelativeInformation>();
            this.tblEmpWorkExperiences = new List<tblEmpWorkExperience>();
            this.tblPMSProjectUsers = new List<tblPMSProjectUser>();
        }

        public int EmployeeId { get; set; }
        public string EmployeeRegisterCode { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateMiddleName { get; set; }
        public string CandidateLastName { get; set; }
        public string GuardianFirstName { get; set; }
        public string GuardianMiddleName { get; set; }
        public string GuardianLastName { get; set; }
        public string ProfilePhoto { get; set; }
        public string Present_HouseNo { get; set; }
        public string Present_Location { get; set; }
        public string Present_Area { get; set; }
        public short Present_Country { get; set; }
        public short Present_State { get; set; }
        public string Present_City { get; set; }
        public string Present_PostalCode { get; set; }
        public string Permanent_HouseNo { get; set; }
        public string Permanent_Location { get; set; }
        public string Permanent_Area { get; set; }
        public short Permanent_Country { get; set; }
        public short Permanent_State { get; set; }
        public string Permanent_City { get; set; }
        public string Permanent_PostalCode { get; set; }
        public string MaritalStatus { get; set; }
        public Nullable<System.DateTime> MarriageAnniversaryDate { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string DrivingLicenceNumber { get; set; }
        public string PassportNumber { get; set; }
        public Nullable<System.DateTime> PassportExpiryDate { get; set; }
        public string AdharNumber { get; set; }
        public string PANCardNumber { get; set; }
        public string PersonalEmailId { get; set; }
        public string PersonalMobile { get; set; }
        public string NomineeMobile { get; set; }
        public string CompanyEmailId { get; set; }
        public string CompanyMobile { get; set; }
        public string BloodGroup { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual ICollection<tblApplyLeave> tblApplyLeaves { get; set; }
        public virtual ICollection<tblEmpCompanyInformation> tblEmpCompanyInformations { get; set; }
        public virtual ICollection<tblEmpCredentialInformation> tblEmpCredentialInformations { get; set; }
        public virtual ICollection<tblEmpDailyInOut> tblEmpDailyInOuts { get; set; }
        public virtual ICollection<tblEmpDailyInOut> tblEmpDailyInOuts1 { get; set; }
        public virtual ICollection<tblEmpDocument> tblEmpDocuments { get; set; }
        public virtual ICollection<tblEmpLoginInformation> tblEmpLoginInformations { get; set; }
        public virtual ICollection<tblEmpQualificationInformation> tblEmpQualificationInformations { get; set; }
        public virtual ICollection<tblEmpRelativeInformation> tblEmpRelativeInformations { get; set; }
        public virtual ICollection<tblEmpWorkExperience> tblEmpWorkExperiences { get; set; }
        public virtual ICollection<tblPMSProjectUser> tblPMSProjectUsers { get; set; }
    }
}
