using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class InventoriesController : ApiController
    {
        #region VARIABLES
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Inventory";
        #endregion

        #region CONSTRUCTOR
        public InventoriesController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        #endregion

        #region DROPDOWN BINDING
        /// <summary>
        /// Retrives list of Vendors for binding in dropdown control
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveVendors()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    //List<tblVendor> list = null;
                    try
                    {
                        var list = db.tblVendors.Select(z => new { z.VendorId, z.VendorName, z.IsActive }).Where(z => z.IsActive == true).OrderBy(z => z.VendorName).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        //list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;

        }

        /// <summary>
        /// Retrives list of Location for binding in dropdown control
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveLocations()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    //List<tblLocation> list = null;

                    try
                    {
                        var list = db.tblLocations.Select(z => new { z.LocationId, z.LocationName, z.IsActive }).Where(a => a.IsActive == true).OrderBy(z => z.LocationName).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {

                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;

        }

        /// <summary>
        /// Retrives list of Brands for binding in dropdown control
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveBrands()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    //List<tblBrand> list = null;
                    try
                    {
                        //list = db.tblBrands.Where(z => z.IsActive == true).OrderBy(z => z.BrandName).ToList();
                        var list = db.tblBrands.Select(z => new { z.BrandId, z.BrandName, z.IsActive }).Where(z => z.IsActive == true).OrderBy(z => z.BrandName).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        //list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;

        }

        /// <summary>
        /// Retrives list of Categories for binding in dropdown control
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveCategories()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    //List<tblCategory> list = null;
                    try
                    {
                        var list = db.tblCategories.Select(z => new { z.CategoryId, z.CategoryName, z.IsActive }).Where(z => z.IsActive == true).OrderBy(z => z.CategoryName).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        //list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;

        }

        /// <summary>
        /// Retrives list of SubCategory for binding in dropdown control
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse RetrieveSubCategories(tblCategory category)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    //List<tblSubCategory> list = null;
                    try
                    {
                        var list = db.tblSubCategories.Select(z => new { z.SubCategoryId, z.SubCategoryName, z.CategoryId, z.IsActive }).Where(z => z.CategoryId == category.CategoryId && z.IsActive == true).OrderBy(z => z.SubCategoryName).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        //list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        #endregion]

        #region INEVNTORY
        /// <summary>
        /// Create or Update Inventory
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateInventories(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblInvetory tbl = null;
                    int inventoryId;
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    try
                    {
                        // ADD MODE
                        if (inventory.InventoryId == 0)
                        {
                            tbl = new tblInvetory
                            {
                                InventoryName = inventory.InventoryName,
                                IssueTo = inventory.IssueTo,
                                VendorId = inventory.VendorId,
                                LocationId = inventory.LocationId,
                                BrandId = inventory.BrandId,
                                CategoryId = inventory.CategoryId,
                                SubCategoryId = inventory.SubCategoryId,
                                PurchaseDate = inventory.PurchaseDate.AddMinutes(-1 * timezone),
                                Amount = inventory.Amount,
                                SerialNumber = inventory.SerialNumber,
                                IsAvailable = inventory.IsAvailable,
                                IsScrap = false,
                                Remarks = inventory.Remarks,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblInvetories.Add(tbl);
                            db.SaveChanges();
                            // GETTING MAX ID FOR INSERTING INVENTORY DETAIL
                            inventoryId = db.tblInvetories.Max(z => z.InventoryId);
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, inventoryId);
                        }
                        else
                        {
                            tbl = db.tblInvetories.Where(z => z.InventoryId == inventory.InventoryId).FirstOrDefault();
                            tbl.InventoryName = inventory.InventoryName;
                            tbl.IssueTo = inventory.IssueTo;
                            tbl.VendorId = inventory.VendorId;
                            tbl.LocationId = inventory.LocationId;
                            tbl.BrandId = inventory.BrandId;
                            tbl.CategoryId = inventory.CategoryId;
                            tbl.SubCategoryId = inventory.SubCategoryId;
                            tbl.PurchaseDate = inventory.PurchaseDate.AddMinutes(-1 * timezone);
                            tbl.Amount = inventory.Amount;
                            tbl.SerialNumber = inventory.SerialNumber;
                            tbl.IsAvailable = inventory.IsAvailable;
                            //tbl.IsScrap = inventory.IsScrap;
                            tbl.IsScrap = false;
                            tbl.Remarks = inventory.Remarks;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            inventoryId = tbl.InventoryId;
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, inventoryId);
                        }
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpPost]
        public ApiResponse RetriveInventory(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    tblInvetory tbl = new tblInvetory();
                    try
                    {
                        tbl = db.tblInvetories.Include("tblInvetoryDetails").Where(z => z.InventoryId == inventory.InventoryId).FirstOrDefault();
                        if (tbl != null)
                        {
                            tbl.CreDate = Convert.ToDateTime(tbl.CreDate).AddMinutes(-1 * timezone);
                            tbl.ChgDate = Convert.ToDateTime(tbl.ChgDate).AddMinutes(-1 * timezone);
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", tbl);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }


        /// <summary>
        /// Retrives list of Invemtories & InventoryDetails
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse RetrieveInventories(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string inventoryName = nvc["InventoryName"];
                    string issueTo = nvc["IssueTo"];
                    //string filters = nvc["filters"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblInvetory> list = null;
                    try
                    {
                        list = db.tblInvetories.Include("tblInvetoryDetails").ToList();
                        if (inventory.VendorId > 0)
                            list = list.Where(z => z.VendorId == inventory.VendorId).ToList();
                        if (inventory.LocationId > 0)
                            list = list.Where(z => z.LocationId == inventory.LocationId).ToList();
                        if (inventory.BrandId > 0)
                            list = list.Where(z => z.BrandId == inventory.BrandId || z.tblInvetoryDetails.Any(x => x.BrandId == inventory.BrandId)).ToList();
                        if (inventory.CategoryId > 0)
                            list = list.Where(z => z.CategoryId == inventory.CategoryId || z.tblInvetoryDetails.Any(x => x.CategoryId == inventory.CategoryId)).ToList();
                        if (inventory.SubCategoryId > 0)
                            list = list.Where(z => z.SubCategoryId == inventory.SubCategoryId || z.tblInvetoryDetails.Any(x => x.SubCategoryId == inventory.SubCategoryId)).ToList();

                        // FILTERING DATA FOR INVENTORY NAME
                        if (!string.IsNullOrEmpty(inventoryName) && inventoryName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.InventoryName.ToLower().Contains(inventoryName.ToLower())).ToList();
                        }

                        // FILTERING DATA FOR ISSUE TO
                        if (!string.IsNullOrEmpty(issueTo) && issueTo != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.IssueTo.ToLower().Contains(issueTo.ToLower())).ToList();
                        }

                        // SORTING DATA
                        list = DoSorting(list, orderBy.Trim());

                        // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                        var Count = list.Count();

                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrive Inventory for showing in popup
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse RetriveInventoriesForPopup(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    tblInvetory tbl = null;
                    try
                    {
                        tbl = db.tblInvetories.Include("tblVendor").Include("tblCategory").Include("tblBrand").Where(z => z.InventoryId == inventory.InventoryId).FirstOrDefault();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", tbl);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Delete specified Inventory and associated Inventory Details
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteInventory(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db = new ERPContext())
                {
                    tblInvetory tbl = null;
                    try
                    {
                        tbl = db.tblInvetories.Where(z => z.InventoryId == inventory.InventoryId).FirstOrDefault();

                        if (tbl != null)
                        {
                            // DELETING CHILD ENTRY EXIST IF ANY
                            List<tblInvetoryDetail> list = db.tblInvetoryDetails.Where(z => z.InventoryId == inventory.InventoryId).ToList();
                            foreach (var item in list)
                            {
                                DeleteInventoryDetail(item);
                            }

                            db.tblInvetories.Remove(tbl);
                            db.SaveChanges();
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, tbl);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        #endregion

        #region INVENTORY DETAILS
        /// <summary>
        /// Create or Update inventory details
        /// </summary>
        /// <param name="inventoryDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateInventoriesDetail(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblInvetoryDetail tbl = null;
                    try
                    {
                        // ADD MODE
                        if (inventoryDetail.SrNo == 0)
                        {
                            tbl = new tblInvetoryDetail
                            {
                                InventoryId = inventoryDetail.InventoryId,
                                BrandId = inventoryDetail.BrandId,
                                CategoryId = inventoryDetail.CategoryId,
                                SubCategoryId = inventoryDetail.SubCategoryId,
                                SerialNumber = inventoryDetail.SerialNumber,
                                IsAvailable = inventoryDetail.IsAvailable,
                                IsScrap = inventoryDetail.IsScrap,
                                Status = inventoryDetail.Status,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblInvetoryDetails.Add(tbl);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, tbl.InventoryId);
                        }
                        // EDIT MODE
                        else
                        {
                            tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                            tbl.BrandId = inventoryDetail.BrandId;
                            tbl.CategoryId = inventoryDetail.CategoryId;
                            tbl.SubCategoryId = inventoryDetail.SubCategoryId;
                            tbl.SerialNumber = inventoryDetail.SerialNumber;
                            tbl.IsAvailable = inventoryDetail.IsAvailable;
                            tbl.IsScrap = inventoryDetail.IsScrap;
                            //tbl.IsAvailable = true;
                            tbl.IsAvailable = false;
                            //IsScrap = inventoryDetail.IsScrap,
                            tbl.IsScrap = false;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, tbl.InventoryId);
                        }
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrive Inventory Details for showing in popup
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse RetriveInventoryDetailForPopup(tblInvetory inventory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    List<tblInvetoryDetail> list = null;
                    try
                    {
                        list = db.tblInvetoryDetails.Include("tblCategory").Include("tblSubCategory").Include("tblBrand").Where(z => z.InventoryId == inventory.InventoryId).ToList();
                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Delete InventoryDetails for specified Inventory
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteInventoryDetail(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                //using (db = new ERPContext())
                //{
                tblInvetoryDetail tbl = null;
                try
                {
                    // DELETING EXISTING DATA FROM DETAIL
                    tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                    db.tblInvetoryDetails.Remove(tbl);
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, tbl.InventoryId);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                }
                finally
                {
                    tbl = null;
                }
                //}
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Receive Inventory Detail
        /// </summary>
        /// <param name="inventoryDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ReceiveInventoryDetail(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db = new ERPContext())
                {
                    tblInvetoryDetail tbl = null;
                    try
                    {
                        // RECEIVE INVENTORY DETAIL
                        tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                        tbl.IsScrap = false;
                        tbl.Status = "R";
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Inventory received successfully", null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Scrap Inventory Detail
        /// </summary>
        /// <param name="inventoryDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ScarpInventoryDetail(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db = new ERPContext())
                {
                    tblInvetoryDetail tbl = null;
                    try
                    {
                        // SCRAP INVENTORY DETAIL
                        tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                        tbl.Status = "R";
                        tbl.IsScrap = true;
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Add detail to stock
        /// </summary>
        /// <param name="inventoryDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse AddToStock(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db = new ERPContext())
                {
                    tblInvetoryDetail tbl = null;
                    try
                    {
                        // SCRAP INVENTORY DETAIL
                        tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                        tbl.Status = "I";
                        tbl.IsScrap = false;
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpPost]
        public ApiResponse ChangeStatus(tblInvetoryDetail inventoryDetail)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db = new ERPContext())
                {
                    tblInvetoryDetail tbl = null;
                    try
                    {
                        // SCRAP INVENTORY DETAIL
                        tbl = db.tblInvetoryDetails.Where(z => z.SrNo == inventoryDetail.SrNo).FirstOrDefault();
                        tbl.Status = "I";
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Status changes successfully", null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        #endregion

        #region FUNCTIONS
        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public List<tblInvetory> DoSorting(List<tblInvetory> list, string orderBy)
        {
            try
            {
                if (orderBy == "InventoryName")
                {
                    list = list.OrderBy(z => z.InventoryName).ToList();
                }
                else if (orderBy == "-InventoryName")
                {
                    list = list.OrderByDescending(z => z.InventoryName).ToList();
                }
                else if (orderBy == "IssueTo")
                {
                    list = list.OrderBy(z => z.IssueTo).ToList();
                }
                else if (orderBy == "-IssueTo")
                {
                    list = list.OrderByDescending(z => z.IssueTo).ToList();
                }
                else if (orderBy == "PurchaseDate")
                {
                    list = list.OrderBy(z => z.PurchaseDate).ToList();
                }
                else if (orderBy == "-PurchaseDate")
                {
                    list = list.OrderByDescending(z => z.PurchaseDate).ToList();
                }
                else if (orderBy == "Amount")
                {
                    list = list.OrderBy(z => z.Amount).ToList();
                }
                else if (orderBy == "-Amount")
                {
                    list = list.OrderByDescending(z => z.Amount).ToList();
                }
                else if (orderBy == "SerialNo")
                {
                    list = list.OrderBy(z => z.SerialNumber).ToList();
                }
                else if (orderBy == "-SerialNo")
                {
                    list = list.OrderByDescending(z => z.SerialNumber).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblInvetory>();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}