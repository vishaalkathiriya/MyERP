using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpDiscipline
    {
        public short DisciplineId { get; set; }
        public short DegreeId { get; set; }
        public string DisciplineName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblEmpDegree tblEmpDegree { get; set; }
    }
}
