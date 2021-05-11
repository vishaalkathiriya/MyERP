using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblBloodGroup
    {
        public short BloodGroupId { get; set; }
        public string BloodGroupName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
    }
}
