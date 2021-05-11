using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRPart
    {
        public tblSRPart()
        {
            this.tblSRPartIssues = new List<tblSRPartIssue>();
            this.tblSRPurchases = new List<tblSRPurchase>();
            this.tblSRRepairs = new List<tblSRRepair>();
        }

        public int PartId { get; set; }
        public string PartName { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblSRPartIssue> tblSRPartIssues { get; set; }
        public virtual ICollection<tblSRPurchase> tblSRPurchases { get; set; }
        public virtual ICollection<tblSRRepair> tblSRRepairs { get; set; }
    }
}
