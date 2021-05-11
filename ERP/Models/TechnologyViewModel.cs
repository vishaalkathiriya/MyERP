using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class TechnologyViewModel
    {
        public int Id { get; set; }
        public string Technologies { get; set; }
        public short TechnologiesGroupId { get; set; }
        public string TechnologiesGroup { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public bool IsActive { get; set; }
    }
}