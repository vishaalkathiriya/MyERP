using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class ApplyLeaveViewModel
    {
        public int EmployeeId { get; set; }
        public string Mode { get; set; }
        public string LeaveTitle { get; set; }
        public short LeaveType { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string PartFullTime { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool IsFestival { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }


    public class calenderSpLeaveList
    {
        public int Id { get; set; }
        public int EmpOrFestivalTypeId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public System.DateTime StratDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public string PartFullTime { get; set; }
        public string Comment { get; set; }
        public string StatusOrColor { get; set; }
        public bool? IsWorkingDay { get; set; }
        public string ApproveReason { get; set; }
        public string CalLeaveType { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
    }


}