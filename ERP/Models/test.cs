using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class test
    {
        public int Id { get; set; }
        public string name { get; set; }
        public short SubCategoryId { get; set; }
        public virtual tblSubCategory tblSubCategory { get; set; }
    }
}
