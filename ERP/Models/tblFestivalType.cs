using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblFestivalType
    {
        public tblFestivalType()
        {
            this.tblFestivals = new List<tblFestival>();
        }

        public short FestivalTypeId { get; set; }
        public string FestivalType { get; set; }
        public bool IsWorkingDay { get; set; }
        public string PartFullTime { get; set; }
        public string DisplayColorCode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblFestival> tblFestivals { get; set; }
    }
}
