using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpUniversity
    {
        public tblEmpUniversity()
        {
            this.tblEmpInstitutes = new List<tblEmpInstitute>();
        }

        public short UniversityId { get; set; }
        public string UniversityName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblEmpInstitute> tblEmpInstitutes { get; set; }
    }
}
