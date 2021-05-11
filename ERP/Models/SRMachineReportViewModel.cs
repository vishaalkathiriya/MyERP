using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRMachineReportViewModel
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string SerialNo { get; set; }
        public DateTime InstallationDate { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int SubTypeId { get; set; }
        public string SubTypeName { get; set; }
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public int FloorId { get; set; }
        public int MachineLocationId { get; set; }
        public string LocationName{ get; set; }
        public string ManagerName { get; set; }
        public DateTime IssueDate { get; set; }


        
    }
}