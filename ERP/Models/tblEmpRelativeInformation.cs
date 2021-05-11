using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpRelativeInformation
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public string RelativeName { get; set; }
        public string RelativeRelation { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string Acedamic { get; set; }
        public string Degree { get; set; }
        public string TypeOfWork { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
