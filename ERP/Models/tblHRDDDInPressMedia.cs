using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDDDInPressMedia
    {
        public int SrNo { get; set; }
        public System.DateTime Date { get; set; }
        public string NameOfNewspaper { get; set; }
        public string EventName { get; set; }
        public string Website { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
