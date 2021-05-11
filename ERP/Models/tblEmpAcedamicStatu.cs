using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpAcedamicStatu
    {
        public tblEmpAcedamicStatu()
        {
            this.tblEmpDegrees = new List<tblEmpDegree>();
        }

        public short AcedamicStatusId { get; set; }
        public string AcedamicStatus { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblEmpDegree> tblEmpDegrees { get; set; }
    }
}
