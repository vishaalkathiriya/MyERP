using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDChemicalStorageInspectionLog
    {
        public int SrNo { get; set; }
        public System.DateTime DateOfInspection { get; set; }
        public string CheckedyBy { get; set; }
        public string Findings { get; set; }
        public string RootCause { get; set; }
        public string CorrectiveAction { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
