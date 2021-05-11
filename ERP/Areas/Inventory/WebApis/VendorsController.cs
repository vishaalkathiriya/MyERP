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
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class VendorsController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Vendor";

        public VendorsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpGet]
        public ApiResponse GetVendorsList()
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
                    string fltrVendorName = nvc["VendorName"];
                    string fltrCompanyName = nvc["CompanyName"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblVendor> list = null;

                    try
                    {
                        list = db.tblVendors.ToList();
                        //1. filter data
                        if (!string.IsNullOrEmpty(fltrVendorName) && fltrVendorName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.VendorName.ToLower().Contains(fltrVendorName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(fltrCompanyName) && fltrCompanyName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CompanyName.ToLower().Contains(fltrCompanyName.ToLower())).ToList();
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
                    catch
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
        public ApiResponse SaveVendor(tblVendor vendor)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    tblVendor tbl = new tblVendor();
                    if (vendor.VendorId > 0)
                    {
                        tbl = db.tblVendors.Where(x => x.VendorId == vendor.VendorId).FirstOrDefault();
                        //if (vendor.IsActive == false)
                        //{
                        //    if (!DependancyStatus.VendorStatus(vendor.VendorId))
                        //    {
                        //        return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //    }
                        //}
                    }
                    tbl.VendorName = vendor.VendorName;
                    tbl.CompanyName = vendor.CompanyName;
                    tbl.Email = vendor.Email;
                    tbl.Website = vendor.Website;
                    tbl.Mobile = vendor.Mobile;
                    tbl.PhoneNo = vendor.PhoneNo;
                    tbl.Services = vendor.Services.ToString();
                    tbl.Rating = vendor.Rating;
                    tbl.HouseNo = vendor.HouseNo;
                    tbl.Location = vendor.Location;
                    tbl.Area = vendor.Area;
                    tbl.Country = vendor.Country;
                    tbl.State = vendor.State;
                    tbl.City = vendor.City;
                    tbl.PostalCode = vendor.PostalCode;
                   
                    tbl.ChgDate = DateTime.Now.ToUniversalTime();
                    tbl.ChgBy = Convert.ToInt16(HttpContext.Current.Session["UserId"]);
                    if (vendor.VendorId <= 0)
                    {
                        tbl.IsActive = vendor.IsActive;
                        tbl.CreDate = DateTime.Now.ToUniversalTime();
                        tbl.CreBy = Convert.ToInt16(HttpContext.Current.Session["UserId"]);
                        db.tblVendors.Add(tbl);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
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

        // POST api/<controller>
        [HttpPost]
        public IEnumerable<tblVendor> GetVendor(tblVendor vendor)
        {
            try
            {
                return db.tblVendors.Where(z => z.VendorId == vendor.VendorId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public ApiResponse DeleteVendor(tblVendor vendor)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.VendorStatus(vendor.VendorId))
                    {
                        tblVendor tbl = db.tblVendors.Where(z => z.VendorId == vendor.VendorId).FirstOrDefault();
                        db.tblVendors.Remove(tbl);
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

        /// Change status of Vendor i.e. Active or Inactive
        [HttpPost]
        public ApiResponse ChangeStatus(tblVendor vendor)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                tblVendor tbl;
                try
                {
                    if (DependancyStatus.VendorStatus(vendor.VendorId))
                    {
                        tbl = db.tblVendors.Where(z => z.VendorId == vendor.VendorId).FirstOrDefault();
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
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public List<tblVendor> DoSorting(List<tblVendor> list, string orderBy)
        {
            try
            {
                // ASC Orader on VenderName
                if (orderBy == "VendorName")
                {
                    list = list.OrderBy(z => z.VendorName).ToList();
                }
                else if (orderBy == "-VendorName")
                {
                    list = list.OrderByDescending(z => z.VendorName).ToList();
                }

                 // ASC Orader on CompanyName
                else if (orderBy == "CompanyName")
                {
                    list = list.OrderBy(z => z.CompanyName).ToList();
                }
                else if (orderBy == "-CompanyName")
                {
                    list = list.OrderByDescending(z => z.CompanyName).ToList();
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