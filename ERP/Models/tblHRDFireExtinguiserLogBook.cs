using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDFireExtinguiserLogBook
    {
        public int SrNo { get; set; }
        public string TypeOfFireExtinguiser { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public System.DateTime DateOfInspection { get; set; }
        public string UsedOfFireExtinguiser { get; set; }
        public System.DateTime DateOfRefilling { get; set; }
        public System.DateTime DueDateForNextRefilling { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
