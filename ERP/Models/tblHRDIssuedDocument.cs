using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDIssuedDocument
    {
        public short HRDIssuedDocId { get; set; }
        public string ECode { get; set; }
        public string FullName { get; set; }
        public short DocumentTypeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string AttachmentName { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> IntercomNo { get; set; }
        public string IssuedBy { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual tblDocument tblDocument { get; set; }
    }
}
