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
    public class HRDChemicalStorageInspactionLogController : ApiController
    {
   
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Chemical storage inspaction log";


        public HRDChemicalStorageInspactionLogController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveChemicalStorageInspactionLog(tblHRDChemicalStorageInspectionLog ChemicalStorageInspaction)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (ChemicalStorageInspaction.SrNo == 0)
                    {
                            tblHRDChemicalStorageInspectionLog d = new tblHRDChemicalStorageInspectionLog
                            {
                                DateOfInspection = ChemicalStorageInspaction.DateOfInspection,
                                CheckedyBy = ChemicalStorageInspaction.CheckedyBy,
                                Findings = ChemicalStorageInspaction.Findings,
                                RootCause = ChemicalStorageInspaction.RootCause,
                                CorrectiveAction = ChemicalStorageInspaction.CorrectiveAction,
                                Remark = ChemicalStorageInspaction.Remark,
                                Attachment = ChemicalStorageInspaction.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDChemicalStorageInspectionLogs.Add(d);
                            MoveFile(ChemicalStorageInspaction.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDChemicalStorageInspectionLogs.Where(z => z.SrNo == ChemicalStorageInspaction.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.DateOfInspection = ChemicalStorageInspaction.DateOfInspection;
                                line.CheckedyBy = ChemicalStorageInspaction.CheckedyBy;
                                line.Findings = ChemicalStorageInspaction.Findings;
                                line.RootCause = ChemicalStorageInspaction.RootCause;
                                line.CorrectiveAction = ChemicalStorageInspaction.CorrectiveAction;
                                line.Remark = ChemicalStorageInspaction.Remark;
                                line.Attachment = ChemicalStorageInspaction.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(ChemicalStorageInspaction.Attachment);
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
        //END CREATE OR UPDATE CHEMICAL STORAGE INSPACTION LOG 



        //DELETE  CHEMICAL STORAGE INSPACTION LOG 
        [HttpPost]
        public ApiResponse DeleteChemicalStorageInspactionLog([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDChemicalStorageInspectionLogs.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDChemicalStorageInspectionLogs.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteChemicalStorageInspaction(line.Attachment);
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




        //BEGIN A LIST OF CHEMICAL STORAGE INSPACTION LOG  FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetChemicalStorageInspactionLog()
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

                    string DateOfInspection = nvc["DateOfInspection"];
                    string CheckedyBy = nvc["CheckedyBy"];
                    string Findings = nvc["Findings"];
                    string RootCause = nvc["RootCause"];
                    string CorrectiveAction = nvc["CorrectiveAction"];
                    string Remark = nvc["Remark"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDChemicalStorageInspectionLog> list = null;
                    try
                    {
                        list = db.tblHRDChemicalStorageInspectionLogs.ToList();

                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfInspection.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfInspection.Date >= fromDate.Date && z.DateOfInspection.Date <= toDate.Date).ToList();
                            }
                        }



                       
                        //1. filter data
                        if (!string.IsNullOrEmpty(CheckedyBy) && CheckedyBy != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CheckedyBy.ToLower().Contains(CheckedyBy.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(Findings) && Findings != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Findings.ToLower().Contains(Findings.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(RootCause) && RootCause != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.RootCause.ToLower().Contains(RootCause.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(CorrectiveAction) && CorrectiveAction != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CorrectiveAction.ToLower().Contains(CorrectiveAction.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(Remark) && Remark != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Remark.ToLower().Contains(Remark.ToLower())).ToList();
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
        public List<tblHRDChemicalStorageInspectionLog> DoSorting(IEnumerable<tblHRDChemicalStorageInspectionLog> list, string orderBy)
        {
            try
            {
                if (orderBy == "DateOfInspection")
                {
                    list = list.OrderByDescending(z => z.DateOfInspection).ToList();
                }
                else if (orderBy == "CheckedyBy")
                {
                    list = list.OrderBy(z => z.CheckedyBy).ToList();
                }
                else if (orderBy == "Findings")
                {
                    list = list.OrderBy(z => z.Findings).ToList();
                }
                else if (orderBy == "RootCause")
                {
                    list = list.OrderByDescending(z => z.RootCause).ToList();
                }
               





                return list.ToList<tblHRDChemicalStorageInspectionLog>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadChemicalStorageInspactionLog"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteChemicalStorageInspaction(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadChemicalStorageInspactionLog"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }

}