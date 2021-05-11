using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpAttendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime PDate { get; set; }
        public decimal Presence { get; set; }
        public decimal Absence { get; set; }
        public decimal Leave { get; set; }
        public decimal OT { get; set; }
        public decimal WorkingHours { get; set; }
        public decimal PersonalWorkHours { get; set; }
        public decimal CompanyWorkHours { get; set; }
        public decimal LunchBreakHours { get; set; }
        public bool IsHoliday { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
