using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class CurrencyController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Currency";


        public CurrencyController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        //BEGIN ADD/EDIT CURRENCY INFORMATION
        [HttpPost]
        public ApiResponse SaveCurrency(tblCurrency currency)
        {
            ApiResponse apiResponse = new ApiResponse();

            if (sessionUtils.HasUserLogin())
            {

                try
                {
                    if (currency.Id == 0)
                    {
                        //add
                        tblCurrency curr = new tblCurrency
                        {
                            CurrencyName = currency.CurrencyName,
                            CurrencyCode = currency.CurrencyCode,
                            CountryId = currency.CountryId,
                            Remark = currency.Remark,
                            IsActive = currency.IsActive,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblCurrencies.Add(curr);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //edit
                        var line = db.tblCurrencies.Where(z => z.Id == currency.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.CurrencyName = currency.CurrencyName;
                            line.CurrencyCode = currency.CurrencyCode;
                            line.CountryId = currency.CountryId;
                            line.Remark = currency.Remark;
                            line.IsActive = currency.IsActive;
                            line.IsDeleted = false;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
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
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        //END ADD/EDIT CURRENCY INFORMATION

        //BEGIN DELETE CURRENCY INFORMATION
        [HttpGet]
        public ApiResponse DeleteCurrency()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int id = Convert.ToInt32(nvc["id"]);
                    //if you check parent to add sopme code
                    var line = db.tblCurrencies.Where(z => z.Id == id).FirstOrDefault();
                    if (line != null)
                    {
                        line.IsDeleted = true;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
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
        //END DELETE CURRENCY INFORMATION


        //BEGIN CHANGE CURRENCY STATUS INFORMATION
        [HttpGet]
        public ApiResponse ChangeStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int id = Convert.ToInt32(nvc["id"]);
                    var line = db.tblCurrencies.Where(z => z.Id == id).FirstOrDefault();
                    if (line != null)
                    {
                        if (line.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!line.IsActive)
                        {
                            line.IsActive = true;
                        }
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
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
        //END CHANGE CURRENCY STATUS INFORMATION

        //BEGIN GET COUNTRY LIST
        [HttpGet]
        public ApiResponse CountryList()
        {
            ApiResponse apiResponse = new ApiResponse();

            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var List = db.tblCountries.Select(z => new
                    {
                        Id = z.CountryId,
                        CountryName = z.CountryName

                    }).ToList().OrderBy(z => z.CountryName).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", List);
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
        //END GET COUNTRY LIST

        //BEGIN GET CURRENCY INFORMATION
        [HttpGet]
        public ApiResponse GetCurrencyList()
        {
            ApiResponse apiResponse = new ApiResponse();

            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                    // string filter = nvc["filter"];
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    int page = Convert.ToInt32(nvc["page"]);
                    string orderBy = nvc["orderby"];

                    string CurrencyName = nvc["CurrencyName"];
                    string CurrencyCode = nvc["CurrencyCode"];
                    string CountryName = nvc["CountryName"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;


                    List<tblCurrency> list = null;

                    try
                    {
                        list = db.tblCurrencies.Where(z => z.IsDeleted == false).ToList();

                        //1. filter Technologies column if exists
                        if (!string.IsNullOrEmpty(CurrencyName) && CurrencyName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CurrencyName.ToLower().Contains(CurrencyName.ToLower())).ToList();
                        }

                        //1. filter Technologies column if exists
                        if (!string.IsNullOrEmpty(CurrencyCode) && CurrencyCode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CurrencyCode.ToLower().Contains(CurrencyCode.ToLower())).ToList();
                        }


                        List<CurrencyViewModel> currencyList = new List<CurrencyViewModel>();
                        foreach (var j in list)
                        {
                            CurrencyViewModel c = new CurrencyViewModel
                            {
                                Id = j.Id,
                                CurrencyName=j.CurrencyName,
                                CurrencyCode=j.CurrencyCode,
                                CountryId=j.CountryId,
                                CountryName=db.tblCountries.Where(z=>z.CountryId == j.CountryId).Select(z=>z.CountryName).FirstOrDefault(),
                                Remark=j.Remark,
                                IsActive=j.IsActive,
                                IsDeleted=j.IsDeleted,
                                CreDate=j.CreDate,
                                ChgDate=j.ChgDate,
                                CreBy=j.CreBy,
                                ChgBy=j.ChgBy
                            };
                            currencyList.Add(c);
                        }



                        //3. filter on TechnologiesGroup col if exists
                        if (!string.IsNullOrEmpty(CountryName) && CountryName != "undefined")
                        {
                            iDisplayStart = 0;
                            currencyList = currencyList.Where(z => z.CountryName.ToLower().Contains(CountryName.ToLower())).ToList();
                        }


                        currencyList = DoSorting(currencyList, orderBy.Trim());


                        //3. take total count to return for ng-table
                        var Count = currencyList.Count();

                        ////4. convert returned datetime to local timezone
                        var CurrencyInfo = currencyList.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = CurrencyInfo.ToList()
                        };


                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            return apiResponse;
        }
        //END GET CURRENCY INFORMATION

        //BEGIN APPLY SORTING IN LIST
        public List<CurrencyViewModel> DoSorting(IEnumerable<CurrencyViewModel> list, string orderBy)
        {

            try
            {
                if (orderBy == "CurrencyName")
                {
                    list = list.OrderBy(z => z.CurrencyName).ToList();
                }
                else if (orderBy == "-CurrencyName")
                {
                    list = list.OrderByDescending(z => z.CurrencyName).ToList();
                }
                else if (orderBy == "CurrencyCode")
                {
                    list = list.OrderBy(z => z.CurrencyCode).ToList();
                }
                else if (orderBy == "-CurrencyCode")
                {
                    list = list.OrderByDescending(z => z.CurrencyCode).ToList();
                }
                else if (orderBy == "CountryName")
                {
                    list = list.OrderBy(z => z.CountryName).ToList();
                }
                else if (orderBy == "-CountryName")
                {
                    list = list.OrderByDescending(z => z.CountryName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<CurrencyViewModel>();
            }
            catch
            {
                return null;
            }

        }
        //END APPLY SORTING IN LIST

        [HttpGet]
        public ApiResponse IsCurrencyCodeExists()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string code = nvc["code"];
                    bool isExists = false;
                    if (db.tblCurrencies.Where(z => z.CurrencyCode.ToLower() == code.ToLower()).Count() > 0) {
                        isExists = true;
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", isExists);
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