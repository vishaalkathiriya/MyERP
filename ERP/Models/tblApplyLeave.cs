using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblApplyLeave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string LeaveTitle { get; set; }
        public short LeaveType { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string PartFullTime { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string ApproveReason { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
    }
}
