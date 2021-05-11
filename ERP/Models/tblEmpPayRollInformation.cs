using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpPayRollInformation
    {
        public short PayRollId { get; set; }
        public int EmployeeId { get; set; }
        public decimal CTC { get; set; }
        public decimal BasicSalary { get; set; }
        public Nullable<decimal> EmploymentTax { get; set; }
        public Nullable<decimal> ESIC { get; set; }
        public int LeavesAllowedPerYear { get; set; }
        public string PFAccountNumber { get; set; }
        public Nullable<decimal> PF { get; set; }
        public string CompanyBankName { get; set; }
        public string CompanyBankAccount { get; set; }
        public string PersonalBankName { get; set; }
        public string PersonalBankAccount { get; set; }
        public string AllocatedPassNo { get; set; }
        public System.DateTime JoiningDate { get; set; }
        public Nullable<System.DateTime> ReLeavingDate { get; set; }
        public string EmploymentStatus { get; set; }
        public Nullable<System.DateTime> PermanentFromDate { get; set; }
        public string GName { get; set; }
        public string SalaryBasedOn { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
