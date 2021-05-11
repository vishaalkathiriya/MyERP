using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVDocument
    {
        public int PKDocId { get; set; }
        public int tblRefId { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public int DocTypeId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
    }
}
