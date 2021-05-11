using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRPurchase
    {
        public int PurchaseId { get; set; }
        public int PartId { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public string Attachment { get; set; }
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRPart tblSRPart { get; set; }
    }
}
