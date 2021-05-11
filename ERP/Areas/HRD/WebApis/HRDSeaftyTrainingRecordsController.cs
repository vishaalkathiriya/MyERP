using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.HRD.WebApis
{
    public class HRDSeaftyTrainingRecordsController : ApiController
    {
    ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Seafty training records";


        public HRDSeaftyTrainingRecordsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveSeftyTrainingRecords(tblHRDSafetyTrainingRecord SeaftyTrainingRecords)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (SeaftyTrainingRecords.SrNo == 0)
                    {
                            tblHRDSafetyTrainingRecord d = new tblHRDSafetyTrainingRecord
                            {
                                SubjectOfTraining = SeaftyTrainingRecords.SubjectOfTraining,
                                DateOfTraining = SeaftyTrainingRecords.DateOfTraining,
                                Department = SeaftyTrainingRecords.Department,
                                ManagerName = SeaftyTrainingRecords.ManagerName,
                                NoOfParticipants = SeaftyTrainingRecords.NoOfParticipants,
                                TrainersName = SeaftyTrainingRecords.TrainersName,
                                Attachment = SeaftyTrainingRecords.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDSafetyTrainingRecords.Add(d);
                            MoveFile(SeaftyTrainingRecords.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDSafetyTrainingRecords.Where(z => z.SrNo == SeaftyTrainingRecords.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.SubjectOfTraining = SeaftyTrainingRecords.SubjectOfTraining;
                                line.DateOfTraining = SeaftyTrainingRecords.DateOfTraining;
                                line.Department = SeaftyTrainingRecords.Department;
                                line.ManagerName = SeaftyTrainingRecords.ManagerName;
                                line.NoOfParticipants = SeaftyTrainingRecords.NoOfParticipants;
                                line.TrainersName = SeaftyTrainingRecords.TrainersName;
                                line.Attachment = SeaftyTrainingRecords.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(SeaftyTrainingRecords.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
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
        //END CREATE OR UPDATE   SEAFTY TRAINING RECORDS



        //DELETE    SEAFTY TRAINING RECORDS
        [HttpPost]
        public ApiResponse DeleteSaftyTrainingRecords([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDSafetyTrainingRecords.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDSafetyTrainingRecords.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteSeaftyTrainingRecordsAttch(line.Attachment);
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
        //END DELETE




        //BEGIN A LIST OF   SEAFTY TRAINING RECORDS FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetSeaftyTrainigRecords()
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

                    string SubjectOfTraining = nvc["SubjectOfTraining"];
                    string DateOfTraining = nvc["DateOfTraining"];
                    string Department = nvc["Department"];
                    string ManagerName = nvc["ManagerName"];
                    string NameOfParticipants = nvc["NameOfParticipants"];
                    string TrainersName = nvc["TrainersName"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDSafetyTrainingRecord> list = null;
                    try
                    {
                        list = db.tblHRDSafetyTrainingRecords.ToList();

                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfTraining.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfTraining.Date >= fromDate.Date && z.DateOfTraining.Date <= toDate.Date).ToList();
                            }
                        }



                       
                        //1. filter data
                        if (!string.IsNullOrEmpty(SubjectOfTraining) && SubjectOfTraining != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.SubjectOfTraining.ToLower().Contains(SubjectOfTraining.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(Department) && Department != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Department.ToLower().Contains(Department.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(ManagerName) && ManagerName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ManagerName.ToLower().Contains(ManagerName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(TrainersName) && TrainersName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TrainersName.ToLower().Contains(TrainersName.ToLower())).ToList();
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
                    catch (Exception aa)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(aa, _pageName, true);
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

        //end get a data
        public List<tblHRDSafetyTrainingRecord> DoSorting(IEnumerable<tblHRDSafetyTrainingRecord> list, string orderBy)
        {
            try
            {
                if (orderBy == "SubjectOfTraining")
                {
                    list = list.OrderByDescending(z => z.SubjectOfTraining).ToList();
                }
                else if (orderBy == "DateOfTraining")
                {
                    list = list.OrderBy(z => z.DateOfTraining).ToList();
                }
                else if (orderBy == "Department")
                {
                    list = list.OrderBy(z => z.Department).ToList();
                }
                
                return list.ToList<tblHRDSafetyTrainingRecord>();
            }
            catch
            {
                return null;
            }
        }
        
        #region COMMON FUNCTION

        protected void MoveFile(string file)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + file))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + file;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadSeaftyTrainingRecords"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteSeaftyTrainingRecordsAttch(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadSeaftyTrainingRecords"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }

}