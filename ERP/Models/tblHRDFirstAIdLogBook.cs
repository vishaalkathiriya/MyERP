using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDFirstAIdLogBook
    {
        public int SrNo { get; set; }
        public string NameOfIssuer { get; set; }
        public string NameOfReceiver { get; set; }
        public string NameOfFirstAIdItems { get; set; }
        public System.DateTime DateOfIssue { get; set; }
        public int Quanity { get; set; }
        public Nullable<decimal> Size { get; set; }
        public string ManagerName { get; set; }
        public string LocationOfFirstAIdBox { get; set; }
        public Nullable<decimal> Price { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
