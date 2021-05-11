using ERP.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ERP.Utilities;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class CategoriesController : ApiController
    {

        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Category";

        public CategoriesController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        // GET api/<controller>
        public IEnumerable<tblCategory> GetCategoryList([FromBody]int timezone)
        {
            //return db.tblCategories.ToList();
            try
            {
                var list = db.tblCategories.ToList();
                return list.Select(i =>
                {
                    i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                    i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                    return i;
                }).OrderBy(z => z.CategoryName).ToList();
            }
            catch
            {
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        public IEnumerable<tblCategory> GetCategory(tblCategory category)
        {
            try
            {
                return db.tblCategories.Where(z => z.CategoryId == category.CategoryId).ToList();
                //return JsonConvert.SerializeObject(t);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse SaveCategory(tblCategory category)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                tblCategory tbl = new tblCategory();
                try
                {
                    if (category.CategoryId > 0)
                    {
                        tbl = db.tblCategories.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();
                        //if (category.IsActive == false)
                        //{
                        //    if (!DependancyStatus.CategoryStatus(category.CategoryId))
                        //    {
                        //        return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //    }
                        //}
                    }
                    tbl.CategoryName = category.CategoryName;
                    //tbl.IsActive = category.IsActive;
                    tbl.ChgDate = DateTime.Now.ToUniversalTime();
                    if (category.CategoryId <= 0)
                    {
                        tbl.IsActive = category.IsActive;
                        tbl.CreDate = DateTime.Now.ToUniversalTime();
                        db.tblCategories.Add(tbl);
                        //Create Record Messege
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //Update Record Messege
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
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse DeleteCategory(tblCategory category)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.CategoryStatus(category.CategoryId))
                    {
                        var line = db.tblCategories.Where(z => z.CategoryId == category.CategoryId).SingleOrDefault();
                        if (line != null)
                            db.tblCategories.Remove(line);

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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse ChangeStatus(tblCategory category)
        {
            ApiResponse apiResponse = new ApiResponse();
            tblCategory tbl;
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.CategoryStatus(category.CategoryId))
                    {
                        tbl = db.tblCategories.Where(z => z.CategoryId == category.CategoryId).FirstOrDefault();
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
                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
                finally
                {
                    tbl = null;
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse GetCatList()
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
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblCategory> list = null;

                    try
                    {
                        list = db.tblCategories.ToList();
                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CategoryName.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
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
                        //apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);

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
        //// POST api/<controller>
        //[HttpPost]
        //public bool DeleteSelectedCat(int[] strIds)
        //{
        //    try
        //    {
        //        foreach (int id in strIds)
        //        {
        //            var line = db.tblCategories.Where(z => z.CategoryId == id).SingleOrDefault();
        //            if (line != null)
        //            {
        //                db.tblCategories.Remove(line);
        //            }
        //        }
        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblCategory> DoSorting(List<tblCategory> list, string orderBy)
        {
            try
            {
                if (orderBy == "CategoryName")
                {
                    list = list.OrderBy(z => z.CategoryName).ToList();
                }
                else if (orderBy == "-CategoryName")
                {
                    list = list.OrderByDescending(z => z.CategoryName).ToList();
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
public class Category
{
    public tblCategory category { get; set; }
    public int CategoryId { get; set; }
    public string Action { get; set; }
    public int TimeZone { get; set; }
}