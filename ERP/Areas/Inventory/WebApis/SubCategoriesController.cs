using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class SubCategoriesController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "SubCategory";

        public SubCategoriesController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        /// <summary>
        /// Create or Update SubCategory
        /// </summary>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateSubCategory(tblSubCategory subCategory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblSubCategory tbl = null;
                    try
                    {
                        // ADD MODE
                        if (subCategory.SubCategoryId == 0)
                        {
                            tbl = new tblSubCategory
                            {
                                CategoryId = subCategory.CategoryId,
                                SubCategoryName = subCategory.SubCategoryName,
                                IsActive = subCategory.IsActive,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblSubCategories.Add(tbl);
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        // EDIT MODE
                        else
                        {
                            //if (subCategory.IsActive == false)
                            //{
                            //    if (!DependancyStatus.SubCategoryStatus(subCategory.SubCategoryId))
                            //    {
                            //        return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                            //    }
                            //}
                            tbl = db.tblSubCategories.Where(z => z.SubCategoryId == subCategory.SubCategoryId).FirstOrDefault();
                            tbl.CategoryId = subCategory.CategoryId;
                            tbl.SubCategoryName = subCategory.SubCategoryName;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            // tbl.IsActive = subCategory.IsActive;
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                        }
                        db.SaveChanges();
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
                ERPUtilities.UnAuthorizedAccess(apiResponse);
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
                    List<tblCategory> list = null;
                    try
                    {
                        list = db.tblCategories.Where(z => z.IsActive == true).OrderBy(z => z.CategoryName).ToList();
                        apiResponse.IsValidUser = true;
                        apiResponse.MessageType = 1;
                        apiResponse.Message = "Success";
                        apiResponse.DataList = list;
                    }
                    catch (Exception ex)
                    {
                        //apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                        //apiResponse.IsValidUser = true;
                        //apiResponse.MessageType = 0;
                        //apiResponse.Message = ex.Message;
                        //apiResponse.DataList = null;
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
                //apiResponse.IsValidUser = false;
                //apiResponse.MessageType = 0;
                //apiResponse.Message = "UnAuthorized User";
                //apiResponse.DataList = null;
            }
            return apiResponse;

        }

        /// <summary>
        /// Retrieves list of SubCategories available
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveSubCategories()
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
                    int topFilter = Convert.ToInt32(nvc["topFilter"]); //top main filter by TechnologyGroup combo selection 
                    string categoryName = nvc["categoryName"];
                    string subCategoryName = nvc["subCategoryName"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    IEnumerable<tblSubCategory> list = null;

                    try
                    {
                        list = db.tblSubCategories.AsEnumerable();

                        if (topFilter > 0)
                        {
                            list = list.Where(z => z.CategoryId == topFilter).ToList();
                        }

                        foreach (var item in list)
                        {
                            item.tblCategory = db.tblCategories.Where(z => z.CategoryId == item.CategoryId).FirstOrDefault();
                        }

                        // FILTERING DATA ON BASIS OF SUBCATEGORY
                        if (!string.IsNullOrEmpty(subCategoryName) && subCategoryName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.SubCategoryName.ToLower().Contains(subCategoryName.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF CATEGORY
                        if (!string.IsNullOrEmpty(categoryName) && categoryName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.tblCategory.CategoryName.ToLower().Contains(categoryName.ToLower())).ToList();
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

        [HttpPost]
        public ApiResponse DeleteSubCategory(tblSubCategory subCategory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    tblSubCategory tbl = null;
                    try
                    {
                        if (DependancyStatus.SubCategoryStatus(subCategory.SubCategoryId))
                        {
                            tbl = db.tblSubCategories.Where(z => z.SubCategoryId == subCategory.SubCategoryId).FirstOrDefault();
                            db.tblSubCategories.Remove(tbl);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }
                        else
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
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
        /// Change status of SubCategory i.e. Active or Inactive
        /// </summary>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ChangeStatus(tblSubCategory subCategory)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblSubCategory tbl = null;
                    try
                    {
                        if (DependancyStatus.SubCategoryStatus(subCategory.SubCategoryId))
                        {
                            tbl = db.tblSubCategories.Where(z => z.SubCategoryId == subCategory.SubCategoryId).FirstOrDefault();
                            if (tbl != null)
                            {
                                if (tbl.IsActive)
                                    tbl.IsActive = false;
                                else if (!tbl.IsActive)
                                    tbl.IsActive = true;
                            }
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                        }
                        else
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
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
        /// Return sorted list based on passed column
        /// </summary>
        public IEnumerable<tblSubCategory> DoSorting(IEnumerable<tblSubCategory> list, string orderBy)
        {
            try
            {
                // SETTING ORDER ON CATEGORYNAME
                if (orderBy == "CategoryName")
                {
                    list = list.OrderBy(z => z.tblCategory.CategoryName);
                }
                else if (orderBy == "-CategoryName")
                {
                    list = list.OrderByDescending(z => z.tblCategory.CategoryName);
                }

                // SETTING ORDER ON SUBCATEGORYNAME
                else if (orderBy == "SubCategoryName")
                {
                    list = list.OrderBy(z => z.SubCategoryName).ToList();
                }
                else if (orderBy == "-SubCategoryName")
                {
                    list = list.OrderByDescending(z => z.SubCategoryName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}