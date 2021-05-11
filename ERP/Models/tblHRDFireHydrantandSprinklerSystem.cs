using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDFireHydrantandSprinklerSystem
    {
        public int SrNo { get; set; }
        public string BuildingName { get; set; }
        public System.DateTime DateOfInspection { get; set; }
        public string CheckedBy { get; set; }
        public string Findings { get; set; }
        public string RootCause { get; set; }
        public string CorrectiveActionTaken { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
