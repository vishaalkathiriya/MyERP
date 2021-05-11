using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSubCategory
    {
        public tblSubCategory()
        {
            this.tblInvetories = new List<tblInvetory>();
            this.tblInvetoryDetails = new List<tblInvetoryDetail>();
        }

        public short SubCategoryId { get; set; }
        public short CategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblCategory tblCategory { get; set; }
        public virtual ICollection<tblInvetory> tblInvetories { get; set; }
        public virtual ICollection<tblInvetoryDetail> tblInvetoryDetails { get; set; }
    }
}
