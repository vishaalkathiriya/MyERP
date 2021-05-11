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
    public class HRDFireHydrantSprinklerSystemController : ApiController
    {
    ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Fire Hydrant Sprinkler System";


        public HRDFireHydrantSprinklerSystemController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveFireHydrantSprinklerSystem(tblHRDFireHydrantandSprinklerSystem FireHydrantSprinklerSystem)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (FireHydrantSprinklerSystem.SrNo == 0)
                    {
                        tblHRDFireHydrantandSprinklerSystem d = new tblHRDFireHydrantandSprinklerSystem
                            {
                                BuildingName = FireHydrantSprinklerSystem.BuildingName,
                                DateOfInspection = FireHydrantSprinklerSystem.DateOfInspection,
                                CheckedBy = FireHydrantSprinklerSystem.CheckedBy,
                                Findings = FireHydrantSprinklerSystem.Findings,
                                RootCause = FireHydrantSprinklerSystem.RootCause,
                                CorrectiveActionTaken = FireHydrantSprinklerSystem.CorrectiveActionTaken,
                                Remark = FireHydrantSprinklerSystem.Remark,
                                Attachment = FireHydrantSprinklerSystem.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDFireHydrantandSprinklerSystems.Add(d);
                            MoveFile(FireHydrantSprinklerSystem.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDFireHydrantandSprinklerSystems.Where(z => z.SrNo == FireHydrantSprinklerSystem.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.BuildingName = FireHydrantSprinklerSystem.BuildingName;
                                line.DateOfInspection = FireHydrantSprinklerSystem.DateOfInspection;
                                line.CheckedBy = FireHydrantSprinklerSystem.CheckedBy;
                                line.Findings = FireHydrantSprinklerSystem.Findings;
                                line.RootCause = FireHydrantSprinklerSystem.RootCause;
                                line.CorrectiveActionTaken = FireHydrantSprinklerSystem.CorrectiveActionTaken;
                                line.Remark = FireHydrantSprinklerSystem.Remark;
                                line.Attachment = FireHydrantSprinklerSystem.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(FireHydrantSprinklerSystem.Attachment);
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
        //END CREATE OR UPDATE Fire Hydrant Sprinkler 



        //DELETE  Fire Hydrant Sprinkler 
        [HttpPost]
        public ApiResponse DeleteFireHydrantSprinklerSystem([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDFireHydrantandSprinklerSystems.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDFireHydrantandSprinklerSystems.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteFireHydrantSprinklerSystem(line.Attachment);
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




        //BEGIN A LIST OF Fire Hydrant Sprinkler  FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetFireHydrantSprinklerSystem()
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

                    string BuildingName = nvc["BuildingName"];
                    string DateOfInspection = nvc["DateOfInspection"];
                    string CheckedBy = nvc["CheckedyBy"];
                    string Findings = nvc["Findings"];
                    string RootCause = nvc["RootCause"];
                    string CorrectiveActionTaken = nvc["CorrectiveActionTaken"];
                    string Remark = nvc["Remark"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDFireHydrantandSprinklerSystem> list = null;
                    try
                    {
                        list = db.tblHRDFireHydrantandSprinklerSystems.ToList();

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
                        if (!string.IsNullOrEmpty(BuildingName) && BuildingName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.BuildingName.ToLower().Contains(BuildingName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(CheckedBy) && CheckedBy != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CheckedBy.ToLower().Contains(CheckedBy.ToLower())).ToList();
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
                        if (!string.IsNullOrEmpty(CorrectiveActionTaken) && CorrectiveActionTaken != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CorrectiveActionTaken.ToLower().Contains(CorrectiveActionTaken.ToLower())).ToList();
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
        public List<tblHRDFireHydrantandSprinklerSystem> DoSorting(IEnumerable<tblHRDFireHydrantandSprinklerSystem> list, string orderBy)
        {
            try
            {
                if (orderBy == "BuildingName")
                {
                    list = list.OrderByDescending(z => z.BuildingName).ToList();
                }
                else if (orderBy == "DateOfInspection")
                {
                    list = list.OrderByDescending(z => z.DateOfInspection).ToList();
                }
                else if (orderBy == "CheckedyBy")
                {
                    list = list.OrderBy(z => z.CheckedBy).ToList();
                }
                else if (orderBy == "Findings")
                {
                    list = list.OrderBy(z => z.Findings).ToList();
                }
                else if (orderBy == "RootCause")
                {
                    list = list.OrderByDescending(z => z.RootCause).ToList();
                }
               





                return list.ToList<tblHRDFireHydrantandSprinklerSystem>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadFireHydrantSprinklerSystem"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteFireHydrantSprinklerSystem(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadFireHydrantSprinklerSystem"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }

}