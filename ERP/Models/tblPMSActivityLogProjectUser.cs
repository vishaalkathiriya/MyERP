using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSActivityLogProjectUser
    {
        public int ProjectUserLogId { get; set; }
        public int ProjectUserId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsTL { get; set; }
        public string DBAction { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
    }
}
