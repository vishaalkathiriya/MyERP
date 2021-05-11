using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRRepairViewModel
    {
        public int RepairId { get; set; }
        public int MachineId { get; set; }
        public string SerialNo { get; set; }
        public string MachineName { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string Problem { get; set; }

        public string RepairedBy { get; set; }
        public string RepairMansName { get; set; }
        public string Others { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}