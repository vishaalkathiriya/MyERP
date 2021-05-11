using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblRole
    {
        public tblRole()
        {
            this.tblARPermissionAssigneds = new List<tblARPermissionAssigned>();
            this.tblEmpCompanyInformations = new List<tblEmpCompanyInformation>();
        }

        public short RolesId { get; set; }
        public string Roles { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblARPermissionAssigned> tblARPermissionAssigneds { get; set; }
        public virtual ICollection<tblEmpCompanyInformation> tblEmpCompanyInformations { get; set; }
    }
}
