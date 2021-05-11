using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblABLanguage
    {
        public tblABLanguage()
        {
            this.tblABContacts = new List<tblABContact>();
        }

        public int LangId { get; set; }
        public string Language { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblABContact> tblABContacts { get; set; }
    }
}
