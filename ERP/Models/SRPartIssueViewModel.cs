using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRPartIssueViewModel
    {
        public int PartIssueId { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string ChallanNo{ get; set; }
        public string IssuedFrom { get; set; }
        public DateTime IssuedDate{ get; set; }
        public string Problem{ get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}