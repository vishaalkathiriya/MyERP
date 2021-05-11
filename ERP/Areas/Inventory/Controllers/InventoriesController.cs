using ERP.ActionFilter;
using ERP.Models;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.Inventory.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class InventoriesController : Controller
    {
        #region VARIABLE DECLARATION
        ERPContext db = null;
        #endregion

        #region CONSTRUCTOR
        public InventoriesController()
        {
            db = new ERPContext();
        }
        #endregion

        #region ACTION METHODS
        //
        // GET: /Inventories/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

        public ActionResult Create(int Id = 0)
        {
            ViewBag.Id = Id;
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }
        #endregion

        #region UPLOAD EXCEL

        /// <summary>
        /// Upload Excel file
        /// </summary>
        /// <param name="FileData"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadExcel(HttpPostedFileBase FileData)
        {
            try
            {
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string mainPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                FileData.SaveAs(mainPath);
                ReadExcel(mainPath);
                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Read content of excel and insert in relative table
        /// </summary>
        /// <param name="mainPath"></param>
        private void ReadExcel(string mainPath)
        {
            try
            {
                FileInfo fi = new FileInfo(mainPath);
                int inventoryId = 0;

                using (ExcelPackage xlPackage = new ExcelPackage(fi))
                {
                    // GET NUMBER OF WORKSHEET IN CURRENT WORKBOOK
                    int workSheetCount = xlPackage.Workbook.Worksheets.Count;
                    for (int i = 1; i <= workSheetCount; i++)
                    {
                        ExcelWorksheet workSheet = xlPackage.Workbook.Worksheets[i];
                        int rowCount = workSheet.Dimension.End.Row;
                        int colCount = workSheet.Dimension.End.Column;

                        for (int row = 3; row <= rowCount; row++)
                        {
                            var inventoryName = workSheet.Cells[row, 1].Value;

                            if (inventoryName != null)
                            {
                                int _vendorId = CheckForVendor(workSheet.Cells[row, 3].Value.ToString());
                                int _locationId = CheckForLocation(workSheet.Cells[row, 4].Value.ToString());
                                int _brandId = CheckForBrand(workSheet.Cells[row, 5].Value.ToString());
                                int _categoryId = CheckForCategory(workSheet.Cells[row, 6].Value.ToString());
                                int _subCategoryId = CheckForSubCategory(_categoryId, workSheet.Cells[row, 7].Value.ToString());
                                //string _dateString = workSheet.Cells[row, 8].Value.ToString();
                                DateTime _purchaseDate = DateTime.FromOADate(Convert.ToDouble(workSheet.Cells[row, 8].Value));
                                //DateTime _purchaseDate = DateTime.ParseExact(_dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

                                tblInvetory tblInventory = new tblInvetory()
                                {
                                    InventoryName = inventoryName.ToString(),
                                    IssueTo = workSheet.Cells[row, 2].Value.ToString(),
                                    VendorId = Convert.ToInt16(_vendorId),
                                    LocationId = Convert.ToInt16(_locationId),
                                    BrandId = Convert.ToInt16(_brandId),
                                    CategoryId = Convert.ToInt16(_categoryId),
                                    SubCategoryId = Convert.ToInt16(_subCategoryId),
                                    PurchaseDate = _purchaseDate,
                                    Amount = Convert.ToInt32(workSheet.Cells[row, 9].Value.ToString()),
                                    SerialNumber = workSheet.Cells[row, 10].Value.ToString(),
                                    IsAvailable = Convert.ToBoolean(workSheet.Cells[row, 11].Value.ToString()),
                                    IsScrap = Convert.ToBoolean(workSheet.Cells[row, 12].Value.ToString()),
                                    Remarks = workSheet.Cells[row, 13].Value.ToString(),
                                    CreDate = DateTime.Now.ToUniversalTime(),
                                    CreBy = Convert.ToInt16(Session["UserId"].ToString()),
                                    ChgDate = DateTime.Now.ToUniversalTime(),
                                    ChgBy = Convert.ToInt16(Session["UserId"].ToString())
                                };

                                db.tblInvetories.Add(tblInventory);
                                db.SaveChanges();
                                // GETTING MAX ID FOR INSERTING INVENTORY DETAIL
                                inventoryId = db.tblInvetories.Max(z => z.InventoryId);

                                int _brandIdForDetail = CheckForBrand(workSheet.Cells[row, 14].Value.ToString());
                                int _categoryIdForDetail = CheckForCategory(workSheet.Cells[row, 15].Value.ToString());
                                int _subCategoryIdForDetail = CheckForSubCategory(_categoryIdForDetail, workSheet.Cells[row, 16].Value.ToString());

                                tblInvetoryDetail tblInventroyDetail = new tblInvetoryDetail()
                                {
                                    InventoryId = inventoryId,
                                    BrandId = Convert.ToInt16(_brandIdForDetail),
                                    CategoryId = Convert.ToInt16(_categoryIdForDetail),
                                    SubCategoryId = Convert.ToInt16(_subCategoryIdForDetail),
                                    SerialNumber = "0",
                                    IsAvailable = false,
                                    IsScrap = false,
                                    Status = "I",
                                    CreDate = DateTime.Now.ToUniversalTime(),
                                    CreBy = Convert.ToInt16(Session["UserId"].ToString()),
                                    ChgDate = DateTime.Now.ToUniversalTime(),
                                    ChgBy = Convert.ToInt16(Session["UserId"].ToString()),
                                };

                                db.tblInvetoryDetails.Add(tblInventroyDetail);
                                db.SaveChanges();
                            }
                            else
                            {
                                int _brandIdForDetail = CheckForBrand(workSheet.Cells[row, 14].Value.ToString());
                                int _categoryIdForDetail = CheckForCategory(workSheet.Cells[row, 15].Value.ToString());
                                int _subCategoryIdForDetail = CheckForSubCategory(_categoryIdForDetail, workSheet.Cells[row, 16].Value.ToString());

                                tblInvetoryDetail tblInventroyDetail = new tblInvetoryDetail()
                                {
                                    InventoryId = inventoryId,
                                    BrandId = Convert.ToInt16(_brandIdForDetail),
                                    CategoryId = Convert.ToInt16(_categoryIdForDetail),
                                    SubCategoryId = Convert.ToInt16(_subCategoryIdForDetail),
                                    SerialNumber = "0",
                                    IsAvailable = false,
                                    IsScrap = false,
                                    Status = "I",
                                    CreDate = DateTime.Now.ToUniversalTime(),
                                    CreBy = Convert.ToInt16(Session["UserId"].ToString()),
                                    ChgDate = DateTime.Now.ToUniversalTime(),
                                    ChgBy = Convert.ToInt16(Session["UserId"].ToString()),
                                };

                                db.tblInvetoryDetails.Add(tblInventroyDetail);
                                db.SaveChanges();
                            }
                        }
                    }
                    // DELETING FILE AFTER PROCESSING
                    if (System.IO.File.Exists(mainPath))
                    {
                        System.IO.File.Delete(mainPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check for Vendor, if not exist create new one
        /// </summary>
        /// <param name="vendorName"></param>
        /// <returns></returns>
        public int CheckForVendor(string vendorName)
        {
            int vendorId = 0;
            try
            {
                var vendor = db.tblVendors.Where(z => z.VendorName == vendorName).FirstOrDefault();
                if (vendor != null)
                {
                    vendorId = vendor.VendorId;
                }
                else
                {
                    tblVendor tbl = new tblVendor()
                    {
                        VendorName = vendorName,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = Convert.ToInt16(Session["UserId"].ToString()),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = Convert.ToInt16(Session["UserId"].ToString())
                    };

                    db.tblVendors.Add(tbl);
                    db.SaveChanges();
                    // GETTING MAX ID
                    vendorId = db.tblVendors.Max(z => z.VendorId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vendorId;
        }

        /// <summary>
        /// Check for Location, if not exist create new one
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public int CheckForLocation(string locationName)
        {
            int locationId = 0;
            try
            {
                var location = db.tblLocations.Where(z => z.LocationName == locationName).FirstOrDefault();
                if (location != null)
                {
                    locationId = location.LocationId;
                }
                else
                {
                    tblLocation tbl = new tblLocation()
                    {
                        LocationName = locationName,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                    };

                    db.tblLocations.Add(tbl);
                    db.SaveChanges();
                    // GETTING MAX ID
                    locationId = db.tblLocations.Max(z => z.LocationId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return locationId;
        }

        /// <summary>
        /// Check for Brand, if not exist create new one
        /// </summary>
        /// <param name="brandName"></param>
        /// <returns></returns>
        public int CheckForBrand(string brandName)
        {
            int brandId = 0;
            try
            {
                var brand = db.tblBrands.Where(z => z.BrandName == brandName).FirstOrDefault();
                if (brand != null)
                {
                    brandId = brand.BrandId;
                }
                else
                {
                    tblBrand tbl = new tblBrand()
                    {
                        BrandName = brandName,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                    };

                    db.tblBrands.Add(tbl);
                    db.SaveChanges();
                    // GETTING MAX ID
                    brandId = db.tblBrands.Max(z => z.BrandId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return brandId;
        }

        /// <summary>
        /// Check for Category, if not exist create new one
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public int CheckForCategory(string categoryName)
        {
            int categoryId = 0;
            try
            {
                var category = db.tblCategories.Where(z => z.CategoryName == categoryName).FirstOrDefault();
                if (category != null)
                {
                    categoryId = category.CategoryId;
                }
                else
                {
                    tblCategory tbl = new tblCategory()
                    {
                        CategoryName = categoryName,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                    };

                    db.tblCategories.Add(tbl);
                    db.SaveChanges();
                    // GETTING MAX ID
                    categoryId = db.tblCategories.Max(z => z.CategoryId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return categoryId;
        }

        /// <summary>
        /// Check for SubCategory, if not exist create new one
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="subCategoryName"></param>
        /// <returns></returns>
        public int CheckForSubCategory(int categoryId, string subCategoryName)
        {
            int subCategoryId = 0;
            try
            {
                var subCategory = db.tblSubCategories.Where(z => z.CategoryId == categoryId && z.SubCategoryName == subCategoryName).FirstOrDefault();
                if (subCategory != null)
                {
                    subCategoryId = subCategory.SubCategoryId;
                }
                else
                {
                    tblSubCategory tbl = new tblSubCategory()
                    {
                        CategoryId = Convert.ToInt16(categoryId),
                        SubCategoryName = subCategoryName,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                    };

                    db.tblSubCategories.Add(tbl);
                    db.SaveChanges();
                    // GETTING MAX ID
                    subCategoryId = db.tblSubCategories.Max(z => z.SubCategoryId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return subCategoryId;
        }
        #endregion
    }
}
