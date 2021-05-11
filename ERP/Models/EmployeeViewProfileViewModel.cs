using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmployeeViewProfileViewModel
    {
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
        public string Present_Country { get; set; }
        public string Present_State { get; set; }
        public string Present_City { get; set; }
        public string Present_PostalCode { get; set; }
        public string Permanent_HouseNo { get; set; }
        public string Permanent_Location { get; set; }
        public string Permanent_Area { get; set; }
        public string Permanent_Country { get; set; }
        public string Permanent_State { get; set; }
        public string Permanent_City { get; set; }
        public string Permanent_PostalCode { get; set; }
        public string MaritalStatus { get; set; }
        public System.DateTime? MarriageAnniversaryDate { get; set; }
        public System.DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string DrivingLicenceNumber { get; set; }
        public string PassportNumber { get; set; }
        public System.DateTime? PassportExpiryDate { get; set; }
        public string AdharNumber { get; set; }
        public string PANCardNumber { get; set; }
        public string PersonalEmailId { get; set; }
        public string PersonalMobile { get; set; }
        public string NomineeMobile { get; set; }
        public string CompanyEmailId { get; set; }
        public string CompanyMobile { get; set; }
        public string BloodGroup { get; set; }
        public string CompanyBankName { get; set; }
        public string CompanyBankAccount { get; set; }
        public System.DateTime? JoiningDate { get; set; }
    }
}