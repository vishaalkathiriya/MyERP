using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class EmployeeMaster
    {
        public decimal ECode { get; set; }
        public string FormNo { get; set; }
        public Nullable<System.DateTime> FormDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string GrandFather { get; set; }
        public byte[] Photo { get; set; }
        public string Manager { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string MaritalStatus { get; set; }
        public Nullable<decimal> PresentPincode { get; set; }
        public Nullable<decimal> VillagePincode { get; set; }
        public string PANCardNo { get; set; }
        public string Cast { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Village { get; set; }
        public string County { get; set; }
        public string District { get; set; }
        public string Hobbies { get; set; }
        public string PFNo { get; set; }
        public string ESICNo { get; set; }
        public string PFNomeeneeName { get; set; }
        public string RefName { get; set; }
        public string RefAdd { get; set; }
        public string RefRelation { get; set; }
        public string RefMobileNo { get; set; }
        public Nullable<decimal> RefKnownYr { get; set; }
        public Nullable<System.DateTime> LeaveDate { get; set; }
        public string LeaveReason { get; set; }
        public string Gender { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string BloodGroup { get; set; }
        public string ShoesBoxNo { get; set; }
        public string PassNo { get; set; }
        public string Intercom { get; set; }
        public Nullable<System.DateTime> DressDate { get; set; }
        public string LockerNo { get; set; }
        public string Wing { get; set; }
        public string Housing { get; set; }
        public string GName { get; set; }
        public string ManCode { get; set; }
        public string AMCode { get; set; }
        public string WagesType { get; set; }
        public Nullable<System.DateTime> JHrDate { get; set; }
        public bool IsFixed { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Removable { get; set; }
        public Nullable<System.DateTime> AnniversaryDate { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string IDProof { get; set; }
        public string ProofType { get; set; }
        public byte[] RefIDProofImg { get; set; }
        public byte[] IDProofImg { get; set; }
        public string TCode { get; set; }
        public Nullable<decimal> Wages { get; set; }
        public string Template { get; set; }
        public string ESICNomeeneeName { get; set; }
        public string PersonalEmail { get; set; }
        public string OfficialEmail { get; set; }
        public Nullable<bool> IsPerfect { get; set; }
        public string ACNO { get; set; }
        public Nullable<decimal> RefECode { get; set; }
        public string Shift { get; set; }
        public bool Disapproved { get; set; }
        public byte[] ESign { get; set; }
    }
}
