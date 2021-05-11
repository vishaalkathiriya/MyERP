using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblARPermissionAssigned
    {
        public int PAssignedId { get; set; }
        public short RoleId { get; set; }
        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }
        public string Permission { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblARModule tblARModule { get; set; }
        public virtual tblARSubModule tblARSubModule { get; set; }
        public virtual tblRole tblRole { get; set; }
    }
}
