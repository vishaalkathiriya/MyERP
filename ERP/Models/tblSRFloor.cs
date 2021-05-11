using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRFloor
    {
        public int FloorId { get; set; }
        public int MachineId { get; set; }
        public int LocationId { get; set; }
        public string Manager { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRMachine tblSRMachine { get; set; }
    }
}
