using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDPPEIssueRegister
    {
        public int SrNo { get; set; }
        public string NameOfIssuer { get; set; }
        public string NameOfRecievr { get; set; }
        public string TypeOfPPE { get; set; }
        public int Quanity { get; set; }
        public string Department { get; set; }
        public string ManagerName { get; set; }
        public decimal Price { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
