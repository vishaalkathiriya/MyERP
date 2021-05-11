using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblARSubModule
    {
        public tblARSubModule()
        {
            this.tblARPermissionAssigneds = new List<tblARPermissionAssigned>();
        }

        public int SubModuleId { get; set; }
        public int ModuleId { get; set; }
        public string SubModuleName { get; set; }
        public string URL { get; set; }
        public string AllowedAccess { get; set; }
        public int SeqNo { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblARModule tblARModule { get; set; }
        public virtual ICollection<tblARPermissionAssigned> tblARPermissionAssigneds { get; set; }
    }
}
