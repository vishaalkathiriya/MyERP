using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRAMC
    {
        public int AMCId { get; set; }
        public int MachineId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRMachine tblSRMachine { get; set; }
    }
}
