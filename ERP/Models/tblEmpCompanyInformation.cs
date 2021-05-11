using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpCompanyInformation
    {
        public short CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public short TeamId { get; set; }
        public short ReportingTo { get; set; }
        public short DesignationId { get; set; }
        public short RolesId { get; set; }
        public string IncrementCycle { get; set; }
        public bool IsTL { get; set; }
        public bool IsBillable { get; set; }
        public int ModuleUser { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblDesignation tblDesignation { get; set; }
        public virtual tblEmpPersonalInformation tblEmpPersonalInformation { get; set; }
        public virtual tblRole tblRole { get; set; }
    }
}
