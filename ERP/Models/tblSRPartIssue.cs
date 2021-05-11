using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRPartIssue
    {
        public int PartIssueId { get; set; }
        public int MachineId { get; set; }
        public int PartId { get; set; }
        public string IssuedFrom { get; set; }
        public string ChallanNo { get; set; }
        public System.DateTime IssuedDate { get; set; }
        public string Problem { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRMachine tblSRMachine { get; set; }
        public virtual tblSRPart tblSRPart { get; set; }
    }
}
