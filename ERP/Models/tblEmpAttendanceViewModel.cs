using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class tblEmpAttendanceViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime PDate { get; set; }
        public string PDateInString { get; set; }
        public decimal Presence { get; set; }
        public decimal Absence { get; set; }
        public decimal Leave { get; set; }
        public decimal OT { get; set; }
        public string WorkingHours { get; set; }
        public string PersonalWorkHours { get; set; }
        public string CompanyWorkHours { get; set; }
        public string LunchBreakHours { get; set; }
        public bool IsHoliday { get; set; }
        public string Remark { get; set; }
        public DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}