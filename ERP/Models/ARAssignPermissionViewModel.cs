using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class ARAssignPermissionViewModel
    {
        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public string SubModuleURL { get; set; }
        public int ModuleSeqNo { get; set; }
        public int SubModuleSeqNo { get; set; }
        public List<SelectItemAccessRights> AllowedAccess { get; set; }
    }


    public class SelectItemAccessRights 
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ARMenu
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int ModuleSeqNo { get; set; }
        public List<tblARSubModule> SubModules { get; set; }
    }
}