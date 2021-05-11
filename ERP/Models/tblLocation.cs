using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblLocation
    {
        public tblLocation()
        {
            this.tblInvetories = new List<tblInvetory>();
        }

        public short LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblInvetory> tblInvetories { get; set; }
    }
}
