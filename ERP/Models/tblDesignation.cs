using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblDesignation
    {
        public tblDesignation()
        {
            this.tblEmpCompanyInformations = new List<tblEmpCompanyInformation>();
        }

        public short Id { get; set; }
        public string Designation { get; set; }
        public short DesignationGroupId { get; set; }
        public short DesignationParentId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblDesignationGroup tblDesignationGroup { get; set; }
        public virtual tblDesignationParent tblDesignationParent { get; set; }
        public virtual ICollection<tblEmpCompanyInformation> tblEmpCompanyInformations { get; set; }
    }
}
