using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRAmcViewModel
    {
        public int AMCId { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate{ get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}