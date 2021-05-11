using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Http;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http.Headers;


namespace ERP.WebApis
{
    public class HRDQuarterlyManagementMeetingController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Quarterly Meeting Management";

        public HRDQuarterlyManagementMeetingController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        //BEGIN CREATE AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION WITH WEB API        
        [HttpPost]
        public ApiResponse CreateUpdateMeeting(tblHRDQuarterlyManagementMeeting quarterMeeting)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (quarterMeeting.SrNo == 0)
                    {// Mode == Add
                        tblHRDQuarterlyManagementMeeting d = new tblHRDQuarterlyManagementMeeting
                        {
                            Title=quarterMeeting.Title,
                            DateOfMeeting=quarterMeeting.DateOfMeeting,
                            ListOfParticipants=quarterMeeting.ListOfParticipants,
                            AgendaOfTraining=quarterMeeting.AgendaOfTraining,
                            DecisionTakenDuringMeeting=quarterMeeting.DecisionTakenDuringMeeting,
                            Attachment=quarterMeeting.Attachment,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblHRDQuarterlyManagementMeetings.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDQuarterlyManagementMeetings.Where(z => z.SrNo == quarterMeeting.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.Title = quarterMeeting.Title;
                            line.DateOfMeeting = quarterMeeting.DateOfMeeting;
                            line.ListOfParticipants = quarterMeeting.ListOfParticipants;
                            line.AgendaOfTraining = quarterMeeting.AgendaOfTraining;
                            line.DecisionTakenDuringMeeting = quarterMeeting.DecisionTakenDuringMeeting;
                            line.Attachment = quarterMeeting.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        //    UpdateSequence(medHelp.SrNo);
                        //   UpdateSequence(mod.SeqNo, mod.ModuleId);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    MoveFile(quarterMeeting.Attachment);
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
        public ApiResponse DeleteMeeting([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDQuarterlyManagementMeetings.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDQuarterlyManagementMeetings.Remove(line);
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(line.Attachment)) {
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
        //END DELETE SOCIAL WELFARE EXPENSE INFORMATION BASE SRNO


        //[HttpPost]
        //public HttpResponseMessage ShowMeetingInfo([FromBody]int id)
        //{
           
            

        // }





        //BEGIN A LIST OF SOCIAL WELFARE EXPENSE INFORMATION WITH SORTING AND FILTERING
        [HttpGet]
        public ApiResponse GetMeetingList()
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
                    string Title = nvc["Title"]; 
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];
                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDQuarterlyManagementMeeting> list = null;
                    try
                    {
                        list = db.tblHRDQuarterlyManagementMeetings.ToList();

                        //top filter
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfMeeting.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfMeeting.Date >= fromDate.Date && z.DateOfMeeting.Date <= toDate.Date).ToList();
                            }
                        }

                        //1. filter data
                        if (!string.IsNullOrEmpty(Title) && Title != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Title.ToLower().Contains(Title.ToLower())).ToList();
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
        //END A LIST OF SOCIAL WELFARE EXPENSE INFORMATION WITH SORTING AND FILTERING

        public List<tblHRDQuarterlyManagementMeeting> DoSorting(IEnumerable<tblHRDQuarterlyManagementMeeting> list, string orderBy)
        {
            try
            {

                if (orderBy == "Title")
                {
                    list = list.OrderBy(z => z.Title).ToList();
                }
                else if (orderBy == "-Title")
                {
                    list = list.OrderByDescending(z => z.Title).ToList();
                }
                else if (orderBy == "DateOfMeeting")
                {
                    list = list.OrderBy(z => z.DateOfMeeting).ToList();
                }
                else if (orderBy == "-DateOfMeeting")
                {
                    list = list.OrderByDescending(z => z.DateOfMeeting).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }


                return list.ToList<tblHRDQuarterlyManagementMeeting>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadQuarterlyMeeting"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadQuarterlyMeeting"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion
        

    }
}
