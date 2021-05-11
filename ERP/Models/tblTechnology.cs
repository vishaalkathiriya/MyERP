using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblTechnology
    {
        public short Id { get; set; }
        public string Technologies { get; set; }
        public short TechnologiesGroupId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblTechnologiesGroup tblTechnologiesGroup { get; set; }
    }
}
