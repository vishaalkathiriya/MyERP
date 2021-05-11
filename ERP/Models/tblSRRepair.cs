using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRRepair
    {
        public int RepairId { get; set; }
        public int MachineId { get; set; }
        public int PartId { get; set; }
        public string Problem { get; set; }
        public string RepairedBy { get; set; }
        public string Others { get; set; }
        public System.DateTime IssueDate { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRMachine tblSRMachine { get; set; }
        public virtual tblSRPart tblSRPart { get; set; }
    }
}
