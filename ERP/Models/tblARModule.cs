using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblARModule
    {
        public tblARModule()
        {
            this.tblARPermissionAssigneds = new List<tblARPermissionAssigned>();
            this.tblARSubModules = new List<tblARSubModule>();
        }

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int SeqNo { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual ICollection<tblARPermissionAssigned> tblARPermissionAssigneds { get; set; }
        public virtual ICollection<tblARSubModule> tblARSubModules { get; set; }
    }
}
