using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using ERP.Utilities;
using System.Collections.Specialized;
using System.Web;

namespace ERP.WebApis
{
    public class BrandsController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Brand";

        public BrandsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// Create or Update brand.
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateBrand(tblBrand brand)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    // CREATE MODE
                    if (brand.BrandId == 0)
                    {
                        tblBrand tbl = new tblBrand
                        {
                            BrandName = brand.BrandName,
                            IsActive = brand.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblBrands.Add(tbl);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    // UPDATE MODE
                    else
                    {
                        //if (brand.IsActive == false)
                        //{
                        //    if (!DependancyStatus.BrandStatus(brand.BrandId))
                        //    {
                        //        return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //    }
                        //}
                        tblBrand tbl = db.tblBrands.Where(z => z.BrandId == brand.BrandId).FirstOrDefault();
                        tbl.BrandName = brand.BrandName;
                        //tbl.IsActive = brand.IsActive;
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrieive list brands.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveBrand()
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

                    List<tblBrand> list = null;

                    try
                    {
                        list = db.tblBrands.ToList();

                        // FILTERING DATA
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.BrandName.ToLower().Contains(filter.ToLower())).ToList();
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
        /// Return sorted list based on passed column
        /// </summary>
        public List<tblBrand> DoSorting(IEnumerable<tblBrand> list, string orderBy)
        {
            try
            {
                if (orderBy == "BrandName")
                {
                    list = list.OrderBy(z => z.BrandName).ToList();
                }
                else if (orderBy == "-BrandName")
                {
                    list = list.OrderByDescending(z => z.BrandName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblBrand>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Delete specific brand.
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteBrand(tblBrand brand)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.BrandStatus(brand.BrandId))
                    {
                        tblBrand tbl = db.tblBrands.Where(z => z.BrandId == brand.BrandId).FirstOrDefault();
                        db.tblBrands.Remove(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                    }else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
                    }
                }
                catch (Exception ex){
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Change status of brand i.e. Active or Inactive
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ChangeStatus(tblBrand brand)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.BrandStatus(brand.BrandId))
                    {
                        tblBrand tbl = db.tblBrands.Where(z => z.BrandId == brand.BrandId).FirstOrDefault();
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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;


        }
    }
}