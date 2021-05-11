using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpQualificationInformation
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public string Acedamic { get; set; }
        public string Degree { get; set; }
        public string Discipline { get; set; }
        public string University { get; set; }
        public string Institute { get; set; }
        public System.DateTime PassingYear { get; set; }
        public decimal Percentages { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
