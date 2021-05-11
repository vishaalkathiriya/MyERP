using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class DailyInOutReportViewModel
    {
        public int EmployeeId { get; set; }
        public string Ecode { get; set; }
        public string EmpName { get; set; }
        public DateTime Edate { get; set; }
        public string TotalHours { get; set; }
        public string TotalPendingLeave { get; set; }
        public string AppliedLeaveStatus { get; set; }
        public CompanyInOutDetail InOutDetail { get; set; }
        public tblEmpAttendanceViewModel Attendance { get; set; }
        public bool isHolidayDay { get; set; }
        public string SalaryBasedOn { get; set; }

    }

    public class CompanyInOutDetail
    {
        public DateTime? InCompanyTime { get; set; }
        public DateTime? OutCompanyTime { get; set; }
        public DateTime? CompanyWorkStart { get; set; }
        public DateTime? CompanyWorkEnd { get; set; }
        public DateTime? PersonalWorkStart { get; set; }
        public DateTime? PersonalWorkEnd { get; set; }
        public DateTime? LunchBreakStart { get; set; }
        public DateTime? LunchBreakEnd { get; set; }

        public string companyHrs { get; set; }
        public string CompanyWorkHrs { get; set; }
        public string personalWorkHrs { get; set; }
        public string lunchBreakHrs { get; set; }
    }

}