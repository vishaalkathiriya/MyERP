using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRMachine
    {
        public tblSRMachine()
        {
            this.tblSRAMCs = new List<tblSRAMC>();
            this.tblSRFloors = new List<tblSRFloor>();
            this.tblSRPartIssues = new List<tblSRPartIssue>();
            this.tblSRRepairs = new List<tblSRRepair>();
        }

        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string SerialNo { get; set; }
        public System.DateTime InstallationDate { get; set; }
        public int TypeId { get; set; }
        public int SubTypeId { get; set; }
        public int ParameterId { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblSRAMC> tblSRAMCs { get; set; }
        public virtual ICollection<tblSRFloor> tblSRFloors { get; set; }
        public virtual ICollection<tblSRPartIssue> tblSRPartIssues { get; set; }
        public virtual ICollection<tblSRRepair> tblSRRepairs { get; set; }
    }
}
