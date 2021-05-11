using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblDesignationParent
    {
        public tblDesignationParent()
        {
            this.tblDesignations = new List<tblDesignation>();
        }

        public short Id { get; set; }
        public string DesignationParent { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblDesignation> tblDesignations { get; set; }
    }
}
