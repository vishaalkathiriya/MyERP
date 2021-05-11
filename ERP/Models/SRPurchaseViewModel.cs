using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRPurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public string ApprovedBy { get; set; }
        public string Attachment { get; set; }
        public string FullAttachmentPath { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}