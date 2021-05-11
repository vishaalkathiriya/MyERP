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
    public class HRDAccidentRecordsController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Accident Records";

        public HRDAccidentRecordsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }


        //BEGIN CREATE AND UPDATE ACCIDENT RECORDS WITH WEB API        
        [HttpPost]
        public ApiResponse SaveAccidentRecords(tblHRDAccidentRecord AccidentRecords)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (AccidentRecords.Hospitalized == true)
                    {
                        if (AccidentRecords.SrNo == 0)
                        {// Mode == Add
                            tblHRDAccidentRecord d = new tblHRDAccidentRecord
                            {
                                TypeOfAccident = AccidentRecords.TypeOfAccident,
                                Department = AccidentRecords.Department,
                                ManagerName = AccidentRecords.ManagerName,
                                NameOfInjuredPerson = AccidentRecords.NameOfInjuredPerson,
                                RootCauseOfAccident = AccidentRecords.RootCauseOfAccident,
                                NoOfCasualities = AccidentRecords.NoOfCasualities,
                                CorrectiveActionTaken = AccidentRecords.CorrectiveActionTaken,
                                NameOfHospital = AccidentRecords.NameOfHospital,
                                TreatmentExpenses = (System.Decimal)AccidentRecords.TreatmentExpenses,
                                Attachment = AccidentRecords.Attachment,
                                Hospitalized = AccidentRecords.Hospitalized,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };
                            db.tblHRDAccidentRecords.Add(d);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        else
                        {// Mode == Edit
                            var line = db.tblHRDAccidentRecords.Where(z => z.SrNo == AccidentRecords.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.TypeOfAccident = AccidentRecords.TypeOfAccident;
                                line.Department = AccidentRecords.Department;
                                line.ManagerName = AccidentRecords.ManagerName;
                                line.NameOfInjuredPerson = AccidentRecords.NameOfInjuredPerson;
                                line.RootCauseOfAccident = AccidentRecords.RootCauseOfAccident;
                                line.NoOfCasualities = AccidentRecords.NoOfCasualities;
                                line.CorrectiveActionTaken = AccidentRecords.CorrectiveActionTaken;
                                line.NameOfHospital = AccidentRecords.NameOfHospital;
                                line.Attachment = AccidentRecords.Attachment;
                                line.TreatmentExpenses = AccidentRecords.TreatmentExpenses;
                                line.Hospitalized = AccidentRecords.Hospitalized;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }

                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);

                        }
                    }
                    else
                    {

                        if (AccidentRecords.SrNo == 0)
                        {// Mode == Add
                            tblHRDAccidentRecord d = new tblHRDAccidentRecord
                            {
                                TypeOfAccident = AccidentRecords.TypeOfAccident,
                                Department = AccidentRecords.Department,
                                ManagerName = AccidentRecords.ManagerName,
                                NameOfInjuredPerson = AccidentRecords.NameOfInjuredPerson,
                                RootCauseOfAccident = AccidentRecords.RootCauseOfAccident,
                                NoOfCasualities = AccidentRecords.NoOfCasualities,
                                CorrectiveActionTaken = AccidentRecords.CorrectiveActionTaken,


                                TreatmentExpenses = 0,
                                NameOfHospital = string.Empty,
                                Hospitalized = false,
                                Attachment = AccidentRecords.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())

                            };
                            db.tblHRDAccidentRecords.Add(d);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        else
                        {// Mode == Edit
                            var line = db.tblHRDAccidentRecords.Where(z => z.SrNo == AccidentRecords.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.TypeOfAccident = AccidentRecords.TypeOfAccident;
                                line.Department = AccidentRecords.Department;
                                line.ManagerName = AccidentRecords.ManagerName;
                                line.NameOfInjuredPerson = AccidentRecords.NameOfInjuredPerson;
                                line.RootCauseOfAccident = AccidentRecords.RootCauseOfAccident;
                                line.NoOfCasualities = AccidentRecords.NoOfCasualities;
                                line.CorrectiveActionTaken = AccidentRecords.CorrectiveActionTaken;
                                line.NameOfHospital = AccidentRecords.NameOfHospital;
                                line.Attachment = AccidentRecords.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());


                                line.TreatmentExpenses = 0;
                                line.NameOfHospital = string.Empty;
                                line.Hospitalized = false;
                            }

                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                        }


                    }
                    MoveFile(AccidentRecords.Attachment);
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
        //END CREATE AND UPDATE ACCIDENT INFORMATION WITH WEB API


        //BEGIN DELETE ACCIDENT INFORMATION BASE SRNO
        [HttpPost]
        public ApiResponse DeleteAccidentRecords([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDAccidentRecords.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDAccidentRecords.Remove(line);
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                        DeleteAccidentRecordsAttachment(line.Attachment);
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
        //END DELETE ACCIDENT INFORMATION BASE SRNO

        //BEGIN A LIST OF ACCIDENT INFORMATION WITH SORTING AND FILTERING
        [HttpGet]
        public ApiResponse GetAccidentRecords()
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

                    string TypeOfAccident = nvc["TypeOfAccident"];
                    string Department = nvc["Department"];
                    string ManagerName = nvc["ManagerName"];
                    string NameOfInjuredPerson = nvc["NameOfInjuredPerson"];
                    string RootCauseOfAccident = nvc["RootCauseOfAccident"];
                    string NoOfCasualities = nvc["NoOfCasualities"];
                    string CorrectiveActionTaken = nvc["CorrectiveActionTaken"];
                    string Hospitalized = nvc["Hospitalized"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDAccidentRecord> list = null;
                    try
                    {
                        list = db.tblHRDAccidentRecords.ToList();

                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.CreDate.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.CreDate.Date >= fromDate.Date && z.CreDate.Date <= toDate.Date).ToList();
                            }
                        }



                        if (Hospitalized == "Y")
                        {

                            list = list.Where(z => z.Hospitalized.Equals(true)).ToList();
                        }

                        if (Hospitalized == "N")
                        {

                            list = list.Where(z => z.Hospitalized.Equals(false)).ToList();
                        }


                        //1. filter data
                        if (!string.IsNullOrEmpty(TypeOfAccident) && TypeOfAccident != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypeOfAccident.ToLower().Contains(TypeOfAccident.ToLower())).ToList();
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

                        if (!string.IsNullOrEmpty(NameOfInjuredPerson) && NameOfInjuredPerson != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfInjuredPerson.ToLower().Contains(NameOfInjuredPerson.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(RootCauseOfAccident) && RootCauseOfAccident != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.RootCauseOfAccident.ToLower().Contains(RootCauseOfAccident.ToLower())).ToList();
                        }



                        if (!string.IsNullOrEmpty(CorrectiveActionTaken) && CorrectiveActionTaken != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CorrectiveActionTaken.Contains(CorrectiveActionTaken.ToLower())).ToList();
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
        //END A LIST OF ACCIDENT INFORMATION WITH SORTING AND FILTERING

        //BEGIN A LIST OF ACCIDENT INFORMATION BASE COLUMN WISE SORTING
        public List<tblHRDAccidentRecord> DoSorting(IEnumerable<tblHRDAccidentRecord> list, string orderBy)
        {
            try
            {
                if (orderBy == "TypeOfAccident")
                {
                    list = list.OrderBy(z => z.TypeOfAccident).ToList();
                }
                else if (orderBy == "-TypeOfAccident")
                {
                    list = list.OrderByDescending(z => z.TypeOfAccident).ToList();
                }
                else if (orderBy == "Department")
                {
                    list = list.OrderBy(z => z.Department).ToList();
                }
                else if (orderBy == "-Department")
                {
                    list = list.OrderByDescending(z => z.Department).ToList();
                }
                else if (orderBy == "ManagerName")
                {
                    list = list.OrderBy(z => z.ManagerName).ToList();
                }
                else if (orderBy == "-ManagerName")
                {
                    list = list.OrderByDescending(z => z.NameOfInjuredPerson).ToList();
                }
                else if (orderBy == "NameOfInjuredPerson")
                {
                    list = list.OrderBy(z => z.NameOfInjuredPerson).ToList();
                }
                else if (orderBy == "-NameOfInjuredPerson")
                {
                    list = list.OrderByDescending(z => z.NameOfInjuredPerson).ToList();
                }
                else if (orderBy == "RootCauseOfAccident")
                {
                    list = list.OrderBy(z => z.RootCauseOfAccident).ToList();
                }
                else if (orderBy == "-RootCauseOfAccident")
                {
                    list = list.OrderByDescending(z => z.RootCauseOfAccident).ToList();
                }
                else if (orderBy == "NoOfCasualities")
                {
                    list = list.OrderBy(z => z.NoOfCasualities).ToList();
                }
                else if (orderBy == "-NoOfCasualities")
                {
                    list = list.OrderByDescending(z => z.NoOfCasualities).ToList();
                }
                else if (orderBy == "CorrectiveActionTaken")
                {
                    list = list.OrderBy(z => z.CorrectiveActionTaken).ToList();
                }
                else if (orderBy == "-CorrectiveActionTaken")
                {
                    list = list.OrderByDescending(z => z.CorrectiveActionTaken).ToList();
                }
                else if (orderBy == "Hospitalized")
                {
                    list = list.OrderBy(z => z.Hospitalized).ToList();
                }
                else if (orderBy == "-Hospitalized")
                {
                    list = list.OrderByDescending(z => z.Hospitalized).ToList();
                }





                return list.ToList<tblHRDAccidentRecord>();
            }
            catch
            {
                return null;
            }
        }
        //END A LIST OF ACCIDENT INFORMATION BASE COLUMN WISE SORTING



        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadAccidentRecords"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteAccidentRecordsAttachment(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadAccidentRecords"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion
    }
}