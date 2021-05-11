using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblDesignationGroup
    {
        public tblDesignationGroup()
        {
            this.tblDesignations = new List<tblDesignation>();
        }

        public short Id { get; set; }
        public string DesignationGroup { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblDesignation> tblDesignations { get; set; }
    }
}
