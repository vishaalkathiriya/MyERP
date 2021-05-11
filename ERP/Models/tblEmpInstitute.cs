using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpInstitute
    {
        public short InstituteId { get; set; }
        public short UniversityId { get; set; }
        public string InstituteName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblEmpUniversity tblEmpUniversity { get; set; }
    }
}
