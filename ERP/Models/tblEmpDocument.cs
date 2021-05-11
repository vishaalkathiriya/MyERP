using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpDocument
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public short DocumentId { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblDocument tblDocument { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
