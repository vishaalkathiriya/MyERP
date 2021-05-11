using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblTechnologiesGroup
    {
        public tblTechnologiesGroup()
        {
            this.tblTechnologies = new List<tblTechnology>();
        }

        public short Id { get; set; }
        public string TechnologiesGroup { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblTechnology> tblTechnologies { get; set; }
    }
}
