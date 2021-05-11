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
using System.Data.Entity.Infrastructure;

namespace ERP.Areas.Invoice.WebApis
{
    public class INVClientSourceController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Client Source";


        public INVClientSourceController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }


        [HttpPost]
        public ApiResponse SaveClientSource(tblINVClientSource clientSource)
        {
            ApiResponse apiResponse = new ApiResponse();

            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (clientSource.PKSourceId == 0)
                    {
                        tblINVClientSource clSource = new tblINVClientSource
                        {
                            SourceName = clientSource.SourceName,
                            IsActive = clientSource.IsActive,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };

                        db.tblINVClientSources.Add(clSource);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        var line = db.tblINVClientSources.Where(z => z.PKSourceId == clientSource.PKSourceId).SingleOrDefault();
                        if (line != null) {
                            line.SourceName = clientSource.SourceName;
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


        [HttpPost]
        public ApiResponse DeleteClientSource([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.INVClientSource(id))
                    {
                        var line = db.tblINVClientSources.Where(z => z.PKSourceId == id).SingleOrDefault();
                        if (line != null)
                        {
                            line.IsDeleted = true;
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }
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

        [HttpPost]
        public ApiResponse ChangeStatus(tblINVClientSource clientSource)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.INVClientSource(clientSource.PKSourceId))
                    {
                        var line = db.tblINVClientSources.Where(z => z.PKSourceId == clientSource.PKSourceId).SingleOrDefault();
                        if (line != null)
                        {
                            if (clientSource.IsActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!clientSource.IsActive)
                            {
                                line.IsActive = true;
                            }
                        }

                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
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


        [HttpGet]
        public ApiResponse GetClientSourceList()
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

                    List<tblINVClientSource> list = null;

                    try
                    {
                        list = db.tblINVClientSources.Where(z=>z.IsDeleted == false).ToList();
                        
                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.SourceName.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var DesignationGroup = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = DesignationGroup.ToList()
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
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        public List<tblINVClientSource> DoSorting(IEnumerable<tblINVClientSource> list, string orderBy)
        {
            try
            {
                if (orderBy == "SourceName")
                {
                    list = list.OrderBy(z => z.SourceName).ToList();
                }
                else if (orderBy == "-SourceName")
                {
                    list = list.OrderByDescending(z => z.SourceName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblINVClientSource>();
            }
            catch
            {
                return null;
            }
        }

    }
}