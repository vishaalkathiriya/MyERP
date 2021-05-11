using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDQuarterlyManagementMeeting
    {
        public int SrNo { get; set; }
        public string Title { get; set; }
        public System.DateTime DateOfMeeting { get; set; }
        public string ListOfParticipants { get; set; }
        public string AgendaOfTraining { get; set; }
        public string DecisionTakenDuringMeeting { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
