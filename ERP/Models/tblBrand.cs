using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblBrand
    {
        public tblBrand()
        {
            this.tblInvetories = new List<tblInvetory>();
            this.tblInvetoryDetails = new List<tblInvetoryDetail>();
        }

        public short BrandId { get; set; }
        public string BrandName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblInvetory> tblInvetories { get; set; }
        public virtual ICollection<tblInvetoryDetail> tblInvetoryDetails { get; set; }
    }
}
