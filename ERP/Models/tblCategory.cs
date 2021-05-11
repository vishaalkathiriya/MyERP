using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblCategory
    {
        public tblCategory()
        {
            this.tblInvetories = new List<tblInvetory>();
            this.tblInvetoryDetails = new List<tblInvetoryDetail>();
            this.tblSubCategories = new List<tblSubCategory>();
        }

        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblInvetory> tblInvetories { get; set; }
        public virtual ICollection<tblInvetoryDetail> tblInvetoryDetails { get; set; }
        public virtual ICollection<tblSubCategory> tblSubCategories { get; set; }
    }
}
