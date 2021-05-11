using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpRelativeRelation
    {
        public short RelationId { get; set; }
        public string RelativeRelationName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
    }
}
