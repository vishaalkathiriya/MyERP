using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class DashboardInfoListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime OnDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; }
        public string ColorCode { get; set; }
        public string ProfilePhoto { get; set; }
        public String OnDateWeekName { get; set; }
        public String EndDateWeekName { get; set; }
        public string LeaveTime { get; set; }
        public System.Guid? FestivalGroupId { get; set; }
    }
}