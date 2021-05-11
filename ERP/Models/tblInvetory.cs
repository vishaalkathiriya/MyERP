using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblInvetory
    {
        public tblInvetory()
        {
            this.tblInvetoryDetails = new List<tblInvetoryDetail>();
        }

        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public short VendorId { get; set; }
        public string IssueTo { get; set; }
        public short LocationId { get; set; }
        public short BrandId { get; set; }
        public short CategoryId { get; set; }
        public short SubCategoryId { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public decimal Amount { get; set; }
        public string SerialNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsScrap { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreDate { get; set; }
        public short CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public short ChgBy { get; set; }
        public virtual tblBrand tblBrand { get; set; }
        public virtual tblCategory tblCategory { get; set; }
        public virtual tblLocation tblLocation { get; set; }
        public virtual tblSubCategory tblSubCategory { get; set; }
        public virtual tblVendor tblVendor { get; set; }
        public virtual ICollection<tblInvetoryDetail> tblInvetoryDetails { get; set; }
    }
}
