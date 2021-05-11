using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblARPermission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
