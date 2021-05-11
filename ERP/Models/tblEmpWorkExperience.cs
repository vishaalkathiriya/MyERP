using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpWorkExperience
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public string Skills { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public decimal Salary { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
