using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSModule
    {
        public int ModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public Nullable<int> ModuleType { get; set; }
        public int Priority { get; set; }
        public bool IsArchived { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
