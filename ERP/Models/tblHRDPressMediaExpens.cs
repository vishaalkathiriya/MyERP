using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDPressMediaExpens
    {
        public int SrNo { get; set; }
        public string NameOfPressMedia { get; set; }
        public string RepresentativeName { get; set; }
        public System.DateTime Date { get; set; }
        public string MobileNumber { get; set; }
        public decimal Amount { get; set; }
        public string ApprovedBy { get; set; }
        public string Occasion { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
