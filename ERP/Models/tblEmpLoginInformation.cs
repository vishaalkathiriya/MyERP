using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpLoginInformation
    {
        public short LoginInfoId { get; set; }
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int PasswordExpiresDays { get; set; }
        public bool IsRemoteLogin { get; set; }
        public bool IsPermit { get; set; }
        public bool IsActive { get; set; }
        public bool IsLogin { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
