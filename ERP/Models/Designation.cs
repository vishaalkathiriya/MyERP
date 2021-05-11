using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class Designation
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public int DesignationGroupId { get; set; }
        public string DesignationGroup { get; set; }
        public int DesignationParentId { get; set; }
        public string DesignationParent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
    }
}