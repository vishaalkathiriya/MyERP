using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class SRExtraViewModel
    {
        public int ExtraId { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string MachineNo { get; set; }
        public System.DateTime ExtraDate { get; set; }
        public string Remark { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
    }
}