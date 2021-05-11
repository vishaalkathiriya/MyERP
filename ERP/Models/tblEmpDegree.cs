using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpDegree
    {
        public tblEmpDegree()
        {
            this.tblEmpDisciplines = new List<tblEmpDiscipline>();
        }

        public short DegreeId { get; set; }
        public short AcedamicStatusId { get; set; }
        public string DegreeName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblEmpAcedamicStatu tblEmpAcedamicStatu { get; set; }
        public virtual ICollection<tblEmpDiscipline> tblEmpDisciplines { get; set; }
    }
}
