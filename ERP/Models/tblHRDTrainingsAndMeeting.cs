using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDTrainingsAndMeeting
    {
        public int SrNo { get; set; }
        public string Department { get; set; }
        public string Manager { get; set; }
        public string Subject { get; set; }
        public int NoOfParticipant { get; set; }
        public string Intercom { get; set; }
        public System.DateTime Date { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
