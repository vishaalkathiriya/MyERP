using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRSubTypeViewModel
    {
        public int SubTypeId { get; set; }
        public string SubTypeName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Selection { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}