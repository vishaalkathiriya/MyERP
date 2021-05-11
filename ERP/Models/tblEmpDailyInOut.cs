using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpDailyInOut
    {
        public int SrNo { get; set; }
        public int EmployeeId { get; set; }
        public string Ecode { get; set; }
        public System.DateTime Intime { get; set; }
        public Nullable<System.DateTime> OutTime { get; set; }
        public string InComments { get; set; }
        public string OutComments { get; set; }
        public Nullable<int> InType { get; set; }
        public Nullable<int> OutType { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChangedBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation1 { get; set; }
    }
}
