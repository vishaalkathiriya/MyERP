using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeRegisterCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string ExpInCompany { get; set; }
        public string ExpTotal { get; set; }
        public bool IsActive { get; set; }
        public DateTime ChgDate { get; set; }
    }
}