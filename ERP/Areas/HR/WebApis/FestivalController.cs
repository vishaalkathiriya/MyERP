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
    public class FestivalController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "Festival";

        public FestivalController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/festival
        /// create and update festival
        /// </summary>
        [HttpPost]
        public ApiResponse SaveFestival(FestivalViewModel fes)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //parse date range 
                    string[] dates = fes.FestivalDate.Split('$');

                    if (fes.FestivalId == 0)
                    {// Mode == Add
                        //create unique guid
                        Guid uniqueId = Guid.NewGuid();
                        SaveData(dates, uniqueId, fes);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        //delete all the entries
                        var line = db.tblFestivals.Where(z => z.FestivalGroupId == fes.FestivalGroupId).ToList();
                        Guid grpId = line.Take(1).Select(z=>z.FestivalGroupId).FirstOrDefault();

                        if (line != null) 
                        {
                            foreach (var l in line) {
                                db.tblFestivals.Remove(l);
                            }
                        }
                        db.SaveChanges();

                        //insert new entry with same group id
                        SaveData(dates, grpId, fes);
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

        protected void SaveData(string[] dates, Guid uniqueId, FestivalViewModel fes)
        {
            if (dates[0] == dates[1])
            { //single date
                //insert into table
                tblFestival f = new tblFestival
                {
                    FestivalName = fes.FestivalName,
                    FestivalDate = Convert.ToDateTime(dates[0]).Date, //startDate
                    FestivalTypeId = fes.FestivalTypeId,
                    FestivalGroupId = uniqueId,
                    IsActive = fes.IsActive,
                    CreDate = DateTime.Now.ToUniversalTime(),
                    ChgDate = DateTime.Now.ToUniversalTime()
                };
                db.tblFestivals.Add(f);
            }
            else
            { //range of date
                for (DateTime dt = Convert.ToDateTime(dates[0]); dt <= Convert.ToDateTime(dates[1]); dt = dt.AddDays(1))
                {
                    //insert into table
                    tblFestival f = new tblFestival
                    {
                        FestivalName = fes.FestivalName,
                        FestivalDate = dt.Date, //date will change in every iteration 
                        FestivalTypeId = fes.FestivalTypeId,
                        FestivalGroupId = uniqueId,
                        IsActive = fes.IsActive,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        ChgDate = DateTime.Now.ToUniversalTime()
                    };
                    db.tblFestivals.Add(f);
                }
            }
        }

        /// <summary>
        /// GET api/festival
        /// retrieve festival type list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveFestivalTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblFestivalTypes.OrderBy(z => z.FestivalType)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.FestivalTypeId,
                            Label = z.FestivalType
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
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
        /// POST api/festival
        /// Change festival status 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblFestival fes)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblFestivals.Where(z => z.FestivalGroupId == fes.FestivalGroupId).ToList();
                    if (line.Count() > 0)
                    {
                        foreach(var l in line)
                        {
                            if (fes.IsActive)
                            {
                                l.IsActive = false;
                            }
                            else if (!fes.IsActive)
                            {
                                l.IsActive = true;
                            }
                            l.ChgDate = DateTime.Now.ToUniversalTime();
                        }

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


        /// <summary>
        /// POST api/festival
        /// delete festival 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteFestival(tblFestival fes)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblFestivals.Where(z => z.FestivalGroupId == fes.FestivalGroupId).ToList();
                    if (line != null)
                    {
                        foreach(var f in line){
                            db.tblFestivals.Remove(f);
                        }
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

        /// <summary>
        /// GET api/role
        /// return festival list with sorting and filtering functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetFestivalList()
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
                    int topFilter = Convert.ToInt32(nvc["topfilter"]); //main filter by Festival Type dropdown 
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<FestivalViewModel> list = new List<FestivalViewModel>();
                    try
                    {
                        var fGroupList = db.tblFestivals.GroupBy(p => p.FestivalGroupId,(key, g) => new { FestivalGroupId = key }).ToList();
                        foreach (var grp in fGroupList) 
                        {
                            var sLine = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).OrderBy(z=>z.FestivalId).Take(1).SingleOrDefault();
                            var eLine = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).OrderByDescending(z => z.FestivalId).Take(1).SingleOrDefault();
                            
                            FestivalViewModel f = new FestivalViewModel
                            {
                                FestivalId = sLine.FestivalId,
                                FestivalName = sLine.FestivalName,
                                FestivalDate =  string.Format("{0} - {1}", sLine.FestivalDate.ToShortDateString(), eLine.FestivalDate.ToShortDateString()),
                                FestivalTypeId = sLine.FestivalTypeId,
                                FestivalType = db.tblFestivalTypes.Where(z => z.FestivalTypeId == sLine.FestivalTypeId).Select(z => z.FestivalType).SingleOrDefault(),
                                FestivalGroupId = sLine.FestivalGroupId,
                                ChgDate = Convert.ToDateTime(sLine.ChgDate).AddMinutes(-1 * timezone),
                                IsActive = sLine.IsActive,
                                totalDays = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).Count(),
                                DisplayColorCode = db.tblFestivalTypes.Where(z=>z.FestivalTypeId == sLine.FestivalTypeId).Select(z=>z.DisplayColorCode).SingleOrDefault(),
                                StartDate = sLine.FestivalDate,
                                EndDate = eLine.FestivalDate
                            };
                            list.Add(f);
                        }

                        //top filter
                        if (topFilter > 0)
                        {
                            list = list.Where(z => z.FestivalTypeId == topFilter).ToList();
                        }

                        //filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.FestivalName.ToLower().Contains(filter.ToLower())).ToList();
                        }

                        //do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
        public List<FestivalViewModel> DoSorting(IEnumerable<FestivalViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "FestivalName")
                {
                    list = list.OrderBy(z => z.FestivalName).ToList();
                }
                else if (orderBy == "-FestivalName")
                {
                    list = list.OrderByDescending(z => z.FestivalName).ToList();
                }
                if (orderBy == "FestivalType")
                {
                    list = list.OrderBy(z => z.FestivalType).ToList();
                }
                else if (orderBy == "-FestivalType")
                {
                    list = list.OrderByDescending(z => z.FestivalType).ToList();
                }
                if (orderBy == "FestivalDate")
                {
                    list = list.OrderBy(z => z.StartDate).ToList();
                }
                else if (orderBy == "-FestivalDate")
                {
                    list = list.OrderByDescending(z => z.EndDate).ToList();
                }

                if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<FestivalViewModel>();
            }
            catch
            {
                return null;
            }
        }
    }
}