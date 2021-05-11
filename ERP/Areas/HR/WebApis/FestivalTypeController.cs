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

namespace ERP.WebApis
{
    public class FestivalTypeController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Festiva Type";


        public FestivalTypeController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/festivaltype
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveFestivalType(tblFestivalType fes)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (fes.FestivalTypeId == 0)
                    {// Mode == Add
                        tblFestivalType f = new tblFestivalType
                        {
                            FestivalType = fes.FestivalType,
                            IsWorkingDay = fes.IsWorkingDay,
                            PartFullTime = fes.PartFullTime,
                            DisplayColorCode = fes.DisplayColorCode,
                            IsActive = fes.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblFestivalTypes.Add(f);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //if (!DependancyStatus.FestivalTypeStatus(fes.FestivalTypeId))
                        //{
                        //    return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //}
                        // Mode == Edit
                        var line = db.tblFestivalTypes.Where(z => z.FestivalTypeId == fes.FestivalTypeId).SingleOrDefault();
                        if (line != null)
                        {

                            line.FestivalType = fes.FestivalType;
                            line.IsWorkingDay = fes.IsWorkingDay;
                            line.PartFullTime = fes.PartFullTime;
                            line.DisplayColorCode = fes.DisplayColorCode;
                            //line.IsActive = fes.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
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

        /// <summary>
        /// POST api/festivaltype
        /// delete festival type 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteFestivalType([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.FestivalTypeStatus(id))
                    {
                        var line = db.tblFestivalTypes.Where(z => z.FestivalTypeId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblFestivalTypes.Remove(line);
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

        /// <summary>
        /// POST api/festivaltype
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblFestivalType fes)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.FestivalTypeStatus(fes.FestivalTypeId))
                    {
                        var line = db.tblFestivalTypes.Where(z => z.FestivalTypeId == fes.FestivalTypeId).SingleOrDefault();
                        if (line != null)
                        {
                            if (fes.IsActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!fes.IsActive)
                            {
                                line.IsActive = true;
                            }
                        }
                        line.ChgDate = DateTime.Now.ToUniversalTime();
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

        /// <summary>
        /// POST api/festivaltype
        /// return festival type list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetFestivalTypeList(tblFestivalType fType)
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

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblFestivalType> list = null;

                    try
                    {
                        list = db.tblFestivalTypes.ToList();

                        //1. filter on col if exists
                        bool isFilter = false;
                        if (!string.IsNullOrEmpty(fType.FestivalType))
                        {
                            isFilter = true;
                            list = list.Where(z => z.FestivalType.ToLower().Contains(fType.FestivalType.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(fType.PartFullTime))
                        {
                            isFilter = true;
                            list = list.Where(z => z.PartFullTime.ToLower().Contains(fType.PartFullTime.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(fType.DisplayColorCode))
                        {
                            isFilter = true;
                            list = list.Where(z => z.DisplayColorCode.ToLower().Contains(fType.DisplayColorCode.ToLower())).ToList();
                        }

                        if (isFilter)
                        {
                            iDisplayStart = 0; //reset pagging if method call is filter data
                        }

                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var FestivalType = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = FestivalType.ToList()
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

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblFestivalType> DoSorting(IEnumerable<tblFestivalType> list, string orderBy)
        {
            try
            {
                if (orderBy == "FestivalType")
                {
                    list = list.OrderBy(z => z.FestivalType).ToList();
                }
                else if (orderBy == "-FestivalType")
                {
                    list = list.OrderByDescending(z => z.FestivalType).ToList();
                }
                if (orderBy == "IsWorkingDay")
                {
                    list = list.OrderBy(z => z.IsWorkingDay).ToList();
                }
                else if (orderBy == "-IsWorkingDay")
                {
                    list = list.OrderByDescending(z => z.IsWorkingDay).ToList();
                }
                if (orderBy == "PartFullTime")
                {
                    list = list.OrderBy(z => z.PartFullTime).ToList();
                }
                else if (orderBy == "-PartFullTime")
                {
                    list = list.OrderByDescending(z => z.PartFullTime).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblFestivalType>();
            }
            catch
            {
                return null;
            }
        }

    }
}
