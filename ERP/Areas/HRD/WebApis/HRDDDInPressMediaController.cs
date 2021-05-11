using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Configuration;
using System.IO;

namespace ERP.WebApis
{
    public class HRDDDInPressMediaController : ApiController
    {

        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "DD Press Media Information";


        public HRDDDInPressMediaController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveDDPressMedia(tblHRDDDInPressMedia DDPressMedia)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DDPressMedia.SrNo == 0)
                    {// Mode == Add
                        tblHRDDDInPressMedia d = new tblHRDDDInPressMedia
                        {
                            Date=DDPressMedia.Date,
                            NameOfNewspaper=DDPressMedia.NameOfNewspaper,
                            EventName=DDPressMedia.EventName,
                            Website=DDPressMedia.Website,
                            Attachment = DDPressMedia.Attachment,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblHRDDDInPressMedias.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDDDInPressMedias.Where(z => z.SrNo == DDPressMedia.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.Date = DDPressMedia.Date;
                            line.NameOfNewspaper = DDPressMedia.NameOfNewspaper;
                            line.EventName = DDPressMedia.EventName;
                            line.Website = DDPressMedia.Website;
                            line.Attachment = DDPressMedia.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    MoveFile(DDPressMedia.Attachment);
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
        public ApiResponse DeleteDDPressMedia([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDDDInPressMedias.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDDDInPressMedias.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                        DeleteProfilePicture(line.Attachment);
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }



        [HttpGet]
        public ApiResponse GetDDPressMediaList()
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

                    string Date = nvc["Date1"];
                    string NameOfNewspaper = nvc["NameOfNewspaper"];
                    string EventName = nvc["EventName"];
                    string Website = nvc["Website"]; 

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDDDInPressMedia> list = null;
                    try
                    {
                        list = db.tblHRDDDInPressMedias.ToList();


                        //top filter
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.Date.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.Date.Date >= fromDate.Date && z.Date.Date <= toDate.Date).ToList();
                            }
                        }

                        //1. filter data
                        if (!string.IsNullOrEmpty(NameOfNewspaper) && NameOfNewspaper != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfNewspaper.ToLower().Contains(NameOfNewspaper.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(EventName) && EventName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EventName.ToLower().Contains(EventName.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Website) && Website != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Website.Contains(Website.ToLower())).ToList();
                        }

                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var Modules = list.Select(i =>
                        {
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = Modules
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


        public List<tblHRDDDInPressMedia> DoSorting(IEnumerable<tblHRDDDInPressMedia> list, string orderBy)
        {
            try
            {

                if (orderBy == "Date")
                {
                    list = list.OrderBy(z => z.Date).ToList();
                }
                else if (orderBy == "-Date")
                {
                    list = list.OrderByDescending(z => z.Date).ToList();
                }

                else if (orderBy == "NameOfNewspaper")
                {
                    list = list.OrderBy(z => z.NameOfNewspaper).ToList();
                }
                else if (orderBy == "-NameOfNewspaper")
                {
                    list = list.OrderByDescending(z => z.NameOfNewspaper).ToList();
                }
                else if (orderBy == "EventName")
                {
                    list = list.OrderBy(z => z.EventName).ToList();
                }
                else if (orderBy == "-EventName")
                {
                    list = list.OrderByDescending(z => z.EventName).ToList();
                }
                else if (orderBy == "Website")
                {
                    list = list.OrderBy(z => z.Website).ToList();
                }
                else if (orderBy == "-Website")
                {
                    list = list.OrderByDescending(z => z.Website).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }


                return list.ToList<tblHRDDDInPressMedia>();
            }
            catch
            {
                return null;
            }
        }



        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadDDPressMedia"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadDDPressMedia"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion

     
    }
}