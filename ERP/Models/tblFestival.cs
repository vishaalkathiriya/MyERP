using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblFestival
    {
        public short FestivalId { get; set; }
        public string FestivalName { get; set; }
        public System.DateTime FestivalDate { get; set; }
        public short FestivalTypeId { get; set; }
        public System.Guid FestivalGroupId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblFestivalType tblFestivalType { get; set; }
    }
}
