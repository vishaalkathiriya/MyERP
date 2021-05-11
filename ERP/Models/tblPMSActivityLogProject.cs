using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSActivityLogProject
    {
        public int ProjectLogId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public decimal TotalEstDays { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
        public string DBAction { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
    }
}
