using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmpAttendanceReportViewModel
    {
        public int EmployeeId { get; set; }
        public string EmpName { get; set; }

        public string JAN { get; set; }
        public string FEB { get; set; }
        public string MAR { get; set; }
        public string APR { get; set; }
        public string MAY { get; set; }
        public string JUN { get; set; }
        public string JUL { get; set; }
        public string AUG { get; set; }
        public string SEP { get; set; }
        public string OCT { get; set; }
        public string NOV { get; set; }
        public string DEC { get; set; }

    }

    public class EmpAttendanceReportDetailViewModel
    {
        public int EmployeeId { get; set; }
        public string EmpName { get; set; }

        public int JOIN_MONTH { get; set; }
        public int JOIN_YEAR { get; set; }
        public int REL_MONTH { get; set; }
        public int REL_YEAR { get; set; }

        public string JAN_P { get; set; }
        public string JAN_A { get; set; }
        public string JAN_L { get; set; }
        public string JAN_O { get; set; }
        public string JAN_H { get; set; }
        public string JAN_WORKHR { get; set; }

        public string FEB_P { get; set; }
        public string FEB_A { get; set; }
        public string FEB_L { get; set; }
        public string FEB_O { get; set; }
        public string FEB_H { get; set; }
        public string FEB_WORKHR { get; set; }

        public string MAR_P { get; set; }
        public string MAR_A { get; set; }
        public string MAR_L { get; set; }
        public string MAR_O { get; set; }
        public string MAR_H { get; set; }
        public string MAR_WORKHR { get; set; }


        public string APR_P { get; set; }
        public string APR_A { get; set; }
        public string APR_L { get; set; }
        public string APR_O { get; set; }
        public string APR_H { get; set; }
        public string APR_WORKHR { get; set; }

        public string MAY_P { get; set; }
        public string MAY_A { get; set; }
        public string MAY_L { get; set; }
        public string MAY_O { get; set; }
        public string MAY_H { get; set; }
        public string MAY_WORKHR { get; set; }

        public string JUN_P { get; set; }
        public string JUN_A { get; set; }
        public string JUN_L { get; set; }
        public string JUN_O { get; set; }
        public string JUN_H { get; set; }
        public string JUN_WORKHR { get; set; }

        public string JUL_P { get; set; }
        public string JUL_A { get; set; }
        public string JUL_L { get; set; }
        public string JUL_O { get; set; }
        public string JUL_H { get; set; }
        public string JUL_WORKHR { get; set; }

        public string AUG_P { get; set; }
        public string AUG_A { get; set; }
        public string AUG_L { get; set; }
        public string AUG_O { get; set; }
        public string AUG_H { get; set; }
        public string AUG_WORKHR { get; set; }

        public string SEP_P { get; set; }
        public string SEP_A { get; set; }
        public string SEP_L { get; set; }
        public string SEP_O { get; set; }
        public string SEP_H { get; set; }
        public string SEP_WORKHR { get; set; }

        public string OCT_P { get; set; }
        public string OCT_A { get; set; }
        public string OCT_L { get; set; }
        public string OCT_O { get; set; }
        public string OCT_H { get; set; }
        public string OCT_WORKHR { get; set; }

        public string NOV_P { get; set; }
        public string NOV_A { get; set; }
        public string NOV_L { get; set; }
        public string NOV_O { get; set; }
        public string NOV_H { get; set; }
        public string NOV_WORKHR { get; set; }

        public string DEC_P { get; set; }
        public string DEC_A { get; set; }
        public string DEC_L { get; set; }
        public string DEC_O { get; set; }
        public string DEC_H { get; set; }
        public string DEC_WORKHR { get; set; }

        public double Total_P { get; set; }
        public double Total_A { get; set; }
        public double Total_L { get; set; }
        public double Total_O { get; set; }
        public double Total_H { get; set; }
        public string Total_WORKHR { get; set; }

        public string PendingLeave { get; set; }
    }

    public class EmpAttendanceMonthFormat
    {
        public string EmpName { get; set; }

        public List<EmpAttendanceEmployeeMonthDetailViewModel> objDetail { get; set; }
        public double Total_P { get; set; }
        public double Total_A { get; set; }
        public double Total_L { get; set; }
        public double Total_O { get; set; }
        public double Total_H { get; set; }
        public double Total_S { get; set; }
        public string Total_WORKHR { get; set; }
    }

    public class EmpAttendanceEmployeeMonthDetailViewModel
    {
        public string EmpName { get; set; }
        public string Remark { get; set; }
        public DateTime Edate { get; set; }
        public bool isHoliday { get; set; }
        public bool isJoined { get; set; }
        public string P { get; set; }
        public string A { get; set; }
        public string L { get; set; }
        public string O { get; set; }
        public string H { get; set; }
        public string WORKHR { get; set; }
        public DateTime? INTime { get; set; }
        public DateTime? OUTTime { get; set; }
    }


    public class EmpAttendanceRemainderViewModel
    {
        public string mth { get; set; }
        public string presence { get; set; }
        public string absence { get; set; }
        public string leave { get; set; }
        public string overTime { get; set; }
        public string holiday { get; set; }
        public string pendingLeave { get; set; }
        public int monthNumber { get; set; }

    }
}