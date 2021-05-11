using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Export utility for Inventory module.
    /// </summary>
    public class Inventories : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            #region VARIABLES
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            int VendorId = Convert.ToInt16(context.Request.QueryString["vendor"]);
            int LocationId = Convert.ToInt16(context.Request.QueryString["location"]);
            int BrandId = Convert.ToInt16(context.Request.QueryString["brand"]);
            int CategoryId = Convert.ToInt16(context.Request.QueryString["category"]);
            int SubCategoryId = Convert.ToInt16(context.Request.QueryString["subcategory"]);
            ERPContext db = new ERPContext();
            #endregion

            #region RETRIEVING LIST OF RECORDS
            var inventoryDetailList = from inventoryDetail in db.tblInvetoryDetails
                                      join brand in db.tblBrands on inventoryDetail.BrandId equals brand.BrandId
                                      join category in db.tblCategories on inventoryDetail.CategoryId equals category.CategoryId
                                      join subcategory in db.tblSubCategories on inventoryDetail.SubCategoryId equals subcategory.SubCategoryId
                                      select new
                                      {
                                          inventoryDetail.SrNo,
                                          inventoryDetail.InventoryId,
                                          brand.BrandName,
                                          category.CategoryName,
                                          subcategory.SubCategoryName,
                                          inventoryDetail.SerialNumber,
                                          inventoryDetail.IsAvailable,
                                          inventoryDetail.IsScrap,
                                          inventoryDetail.Status
                                      };

            var inventoryList = from inventory in db.tblInvetories
                                join vendor in db.tblVendors on inventory.VendorId equals vendor.VendorId
                                join location in db.tblLocations on inventory.LocationId equals location.LocationId
                                join brand in db.tblBrands on inventory.BrandId equals brand.BrandId
                                join category in db.tblCategories on inventory.CategoryId equals category.CategoryId
                                join subcategory in db.tblSubCategories on inventory.SubCategoryId equals subcategory.SubCategoryId
                                join d in inventoryDetailList on inventory.InventoryId equals d.InventoryId into temp
                                from detail in temp.DefaultIfEmpty()
                                select new
                                {
                                    inventory.InventoryId,
                                    inventory.InventoryName,
                                    inventory.IssueTo,
                                    vendor.VendorId,
                                    vendor.VendorName,
                                    location.LocationId,
                                    location.LocationName,
                                    brand.BrandId,
                                    brand.BrandName,
                                    category.CategoryId,
                                    category.CategoryName,
                                    subcategory.SubCategoryId,
                                    subcategory.SubCategoryName,
                                    inventory.PurchaseDate,
                                    inventory.Amount,
                                    inventory.SerialNumber,
                                    IsAvailable = inventory.IsAvailable == null ? "" : (detail.IsAvailable ? "True" : "False"),
                                    IsScrap = inventory.IsScrap == null ? "" : (detail.IsScrap ? "True" : "False"),
                                    inventory.Remarks,
                                    detail.SrNo,
                                    _BrandName = detail.BrandName,
                                    _CategoryName = detail.CategoryName,
                                    _SubCategoryName = detail.SubCategoryName,
                                    _SerialNumber = detail.SerialNumber,
                                    _IsAvailable = detail.IsAvailable == null ? "" : (detail.IsAvailable ? "True" : "False"),
                                    _IsScrap = detail.IsScrap == null ? "" : (detail.IsScrap ? "True" : "False"),
                                    _Status = detail.Status == "I" ? "Issue" : "Receive"
                                };

            if (VendorId != 0)
            {
                inventoryList = inventoryList.Where(z => z.VendorId == VendorId);
            }
            if (LocationId != 0)
            {
                inventoryList = inventoryList.Where(z => z.LocationId == LocationId);
            }
            if (BrandId != 0)
            {
                inventoryList = inventoryList.Where(z => z.BrandId == BrandId);
            }
            if (CategoryId != 0)
            {
                inventoryList = inventoryList.Where(z => z.CategoryId == CategoryId);
            }
            if (SubCategoryId != 0)
            {
                inventoryList = inventoryList.Where(z => z.SubCategoryId == SubCategoryId);
            }

            var list = inventoryList.Select(z => new
            {
                z.InventoryId,
                z.InventoryName,
                z.IssueTo,
                z.VendorName,
                z.LocationName,
                z.BrandName,
                z.CategoryName,
                z.SubCategoryName,
                z.PurchaseDate,
                z.Amount,
                z.SerialNumber,
                z.IsAvailable,
                z.IsScrap,
                z.Remarks,
                z._BrandName,
                z._CategoryName,
                z._SubCategoryName,
                z._SerialNumber,
                z._IsAvailable,
                z._IsScrap,
                z._Status
            }).Where(z => z._Status == "Issue").OrderBy(z => z.InventoryId).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(list.ToList());

            int id = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (id == Convert.ToInt32(dt.Rows[i]["InventoryId"]))
                {
                    dt.Rows[i][1] = "";
                    dt.Rows[i][2] = "";
                    dt.Rows[i][3] = "";
                    dt.Rows[i][4] = "";
                    dt.Rows[i][5] = "";
                    dt.Rows[i][6] = "";
                    dt.Rows[i][7] = "";
                    dt.Rows[i][8] = DBNull.Value;
                    dt.Rows[i][9] = DBNull.Value;
                    dt.Rows[i][10] = "";
                    dt.Rows[i][11] = DBNull.Value;
                    dt.Rows[i][12] = DBNull.Value;
                    dt.Rows[i][13] = "";
                }
                else
                {
                    id = Convert.ToInt32(dt.Rows[i]["InventoryId"]);
                }
            }
            dt.AcceptChanges();
            dt.Columns.RemoveAt(0);

            ERPUtilities.ExportExcel(context, timezone, dt, "Inventory List", "Inventory List", "InventoryList");
            #endregion
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    #region CLASSES
    //public class Inventories
    //{
    //    public int InventoryId { get; set; }
    //    public string InventoryName { get; set; }
    //    public string IssueTo { get; set; }
    //    public string VendorName { get; set; }
    //    public string LocationName { get; set; }
    //    public string BrandName { get; set; }
    //    public string CategoryName { get; set; }
    //    public string SubCategoryName { get; set; }
    //    public DateTime PurchaseDate { get; set; }
    //    public decimal Amount { get; set; }
    //    public string SerialNumber { get; set; }
    //    public bool? IsAvailable { get; set; }
    //    public bool? IsScrap { get; set; }
    //    public string Remarks { get; set; }
    //    public string _BrandName { get; set; }
    //    public string _CategoryName { get; set; }
    //    public string _SubCategoryName { get; set; }
    //    public string _SerialNumber { get; set; }
    //    public bool? _IsAvailable { get; set; }
    //    public bool? _IsScrap { get; set; }
    //    public string _Status { get; set; }
    //}
    #endregion
}