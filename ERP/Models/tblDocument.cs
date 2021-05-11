using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblDocument
    {
        public tblDocument()
        {
            this.tblEmpDocuments = new List<tblEmpDocument>();
            this.tblHRDIssuedDocuments = new List<tblHRDIssuedDocument>();
        }

        public short Id { get; set; }
        public string Documents { get; set; }
        public bool IsActive { get; set; }
        public int DocumentTypeId { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblEmpDocument> tblEmpDocuments { get; set; }
        public virtual ICollection<tblHRDIssuedDocument> tblHRDIssuedDocuments { get; set; }
    }
}
