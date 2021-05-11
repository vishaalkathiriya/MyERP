using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSProjectUser
    {
        public int ProjectUserId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsTL { get; set; }
        public int UserUnder { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
        public virtual tblPMSProject tblPMSProject { get; set; }
    }
}
