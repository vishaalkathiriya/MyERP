using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class ARSubModuleViewModel
    {
        public int SubModuleId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public string URL { get; set; }
        public List<SelectItemAccessRights> AllowedAccess { get; set; }
        public int SeqNo { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime ChgDate { get; set; }
    }
}