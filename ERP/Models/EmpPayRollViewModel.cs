using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmpPayRollViewModel
    {
        public short PayRollId { get; set; }
        public int EmployeeId { get; set; }
        public decimal CTC { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal EmploymentTax { get; set; }
        public decimal ESIC { get; set; }
        public int LeavesAllowedPerYear { get; set; }
        public string PFAccountNumber { get; set; }
        public decimal PF { get; set; }
        public string CompanyBankName { get; set; }
        public string CompanyBankAccount { get; set; }
        public string PersonalBankName { get; set; }
        public string PersonalBankAccount { get; set; }
        public string AllocatedPassNo { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? ReLeavingDate { get; set; }
        public string EmploymentStatus { get; set; }
        public DateTime? PermanentFromDate { get; set; }
        public string GroupName { get; set; }
        public string SalaryBasedOn { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}