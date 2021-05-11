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
    public class HRDTrainingsAndMeetingController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Training And Meeting Information";

        public HRDTrainingsAndMeetingController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);    
        
        }
        
        [HttpPost]
        public ApiResponse SaveTrainingAndMeeting(tblHRDTrainingsAndMeeting trainingMeeting)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (trainingMeeting.SrNo == 0)
                    {// Mode == Add
                        tblHRDTrainingsAndMeeting d = new tblHRDTrainingsAndMeeting
                        {
                            Department=trainingMeeting.Department,
                            Manager=trainingMeeting.Manager,
                            Subject=trainingMeeting.Subject,
                            NoOfParticipant=trainingMeeting.NoOfParticipant,
                            Intercom=trainingMeeting.Intercom,
                            Date=trainingMeeting.Date,
                            Attachment = trainingMeeting.Attachment,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblHRDTrainingsAndMeetings.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDTrainingsAndMeetings.Where(z => z.SrNo == trainingMeeting.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.Department = trainingMeeting.Department;
                            line.Manager = trainingMeeting.Manager;
                            line.Subject = trainingMeeting.Subject;
                            line.NoOfParticipant = trainingMeeting.NoOfParticipant;
                            line.Intercom = trainingMeeting.Intercom;
                            line.Date = trainingMeeting.Date;
                            line.Attachment = trainingMeeting.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    MoveFile(trainingMeeting.Attachment);
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
        public ApiResponse DeleteTrainingAndMeeting([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDTrainingsAndMeetings.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDTrainingsAndMeetings.Remove(line);
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

        [HttpGet]
        public ApiResponse GetTrainingAndMeetingList()
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
                    string Department = nvc["Department"];
                    string Manager = nvc["Manager"];
                    string Subject = nvc["Subject"];
                    string NoOfParticipant = nvc["NoOfParticipant"];
                    string Intercom = nvc["Intercom"];
                 //   string Date1 = nvc["Date"];    
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDTrainingsAndMeeting> list = null;

                    try
                    {
                        list = db.tblHRDTrainingsAndMeetings.ToList();
                        
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
                        if (!string.IsNullOrEmpty(Department) && Department != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Department.ToLower().Contains(Department.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Manager) && Manager != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Manager.ToLower().Contains(Manager.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Subject) && Subject != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Subject.Contains(Subject.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Intercom) && Intercom != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Intercom.Contains(Intercom.ToLower())).ToList();
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

        public List<tblHRDTrainingsAndMeeting> DoSorting(IEnumerable<tblHRDTrainingsAndMeeting> list, string orderBy)
        {
            try
            {
                if (orderBy == "Department")
                {
                    list = list.OrderBy(z => z.Department).ToList();
                }
                else if (orderBy == "-Department")
                {
                    list = list.OrderByDescending(z => z.Department).ToList();
                }

                else if (orderBy == "Manager")
                {
                    list = list.OrderBy(z => z.Manager).ToList();
                }
                else if (orderBy == "-Manager")
                {
                    list = list.OrderByDescending(z => z.Manager).ToList();
                }
                else if (orderBy == "Subject")
                {
                    list = list.OrderBy(z => z.Subject).ToList();
                }
                else if (orderBy == "-Subject")
                {
                    list = list.OrderByDescending(z => z.Subject).ToList();
                }
                else if (orderBy == "NoOfParticipant")
                {
                    list = list.OrderBy(z => z.NoOfParticipant).ToList();
                }
                else if (orderBy == "-NoOfParticipant")
                {
                    list = list.OrderByDescending(z => z.NoOfParticipant).ToList();
                }
                else if (orderBy == "Intercom")
                {
                    list = list.OrderBy(z => z.Intercom).ToList();
                }
                else if (orderBy == "-Intercom")
                {
                    list = list.OrderByDescending(z => z.Intercom).ToList();
                }
                else if (orderBy == "Date")
                {
                    list = list.OrderBy(z => z.Date).ToList();
                }
                else if (orderBy == "-Date")
                {
                    list = list.OrderByDescending(z => z.Date).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }

                return list.ToList<tblHRDTrainingsAndMeeting>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadTrainingAndMeeting"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadTrainingAndMeeting"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion
    }
}