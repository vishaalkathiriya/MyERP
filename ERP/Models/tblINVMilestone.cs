using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVMilestone
    {
        public int PKMilestoneId { get; set; }
        public int FKProjectId { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDesc { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVProject tblINVProject { get; set; }
    }
}
