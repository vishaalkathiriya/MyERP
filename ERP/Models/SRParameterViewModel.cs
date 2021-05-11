using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRParameterViewModel
    {
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public int SubTypeId { get; set; }
        public string SubTypeName { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public string Remarks { get; set; }
    }
}