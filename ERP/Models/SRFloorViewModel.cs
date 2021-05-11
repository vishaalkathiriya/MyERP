using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRFloorViewModel
    {
        public int FloorId { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string SerialNo { get; set; }
        public int LocationId{ get; set; }
        public string LocationName{ get; set; }
        public string Manager{ get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}