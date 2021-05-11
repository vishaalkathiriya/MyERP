using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblInvetoryDetail
    {
        public int SrNo { get; set; }
        public int InventoryId { get; set; }
        public short BrandId { get; set; }
        public short CategoryId { get; set; }
        public short SubCategoryId { get; set; }
        public string SerialNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsScrap { get; set; }
        public string Status { get; set; }
        public System.DateTime CreDate { get; set; }
        public short CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public short ChgBy { get; set; }
        public virtual tblBrand tblBrand { get; set; }
        public virtual tblCategory tblCategory { get; set; }
        public virtual tblInvetory tblInvetory { get; set; }
        public virtual tblSubCategory tblSubCategory { get; set; }
    }
}
