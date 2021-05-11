using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEmpSource
    {
        public tblEmpSource()
        {
            this.tblEmpCredentialInformations = new List<tblEmpCredentialInformation>();
        }

        public short SourceId { get; set; }
        public string SourceName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblEmpCredentialInformation> tblEmpCredentialInformations { get; set; }
    }
}
