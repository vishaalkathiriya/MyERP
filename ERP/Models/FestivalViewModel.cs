using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class FestivalViewModel
    {
        public int FestivalId { get; set; }
        public string FestivalName { get; set; }
        public string FestivalDate { get; set; }
        public short FestivalTypeId { get; set; }
        public string FestivalType { get; set; }
        public Guid FestivalGroupId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreDate { get; set; }
        public DateTime ChgDate { get; set; }
        public int totalDays { get; set; }
        public string DisplayColorCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}