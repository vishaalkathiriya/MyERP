using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDSafetyTrainingRecord
    {
        public int SrNo { get; set; }
        public string SubjectOfTraining { get; set; }
        public System.DateTime DateOfTraining { get; set; }
        public string Department { get; set; }
        public string ManagerName { get; set; }
        public int NoOfParticipants { get; set; }
        public string TrainersName { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
