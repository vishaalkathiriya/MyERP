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
    public class HRDBoilPlantCleaningRecordsController : ApiController
    {

        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Boil Plant Cleaning Records";


        public HRDBoilPlantCleaningRecordsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveBoilPlantClearingRecords(tblHRDBoilPlantCleaningRecord BoilPlantCleaning)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (BoilPlantCleaning.SrNo == 0)
                    {
                            tblHRDBoilPlantCleaningRecord d = new tblHRDBoilPlantCleaningRecord
                            {
                                BoilPlantLocation = BoilPlantCleaning.BoilPlantLocation,
                                DateOfCleaining = BoilPlantCleaning.DateOfCleaining,
                                NameOfCleaner = BoilPlantCleaning.NameOfCleaner,
                                PlantIncharge = BoilPlantCleaning.PlantIncharge,
                                Remark = BoilPlantCleaning.Remark,
                                Attachment=BoilPlantCleaning.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDBoilPlantCleaningRecords.Add(d);
                            MoveFile(BoilPlantCleaning.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDBoilPlantCleaningRecords.Where(z => z.SrNo == BoilPlantCleaning.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.BoilPlantLocation = BoilPlantCleaning.BoilPlantLocation;
                                line.DateOfCleaining = BoilPlantCleaning.DateOfCleaining;
                                line.NameOfCleaner = BoilPlantCleaning.NameOfCleaner;
                                line.PlantIncharge = BoilPlantCleaning.PlantIncharge;
                                line.Remark = BoilPlantCleaning.Remark;
                                line.Attachment = BoilPlantCleaning.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(BoilPlantCleaning.Attachment);
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
        //END CREATE OR UPDATE BOIL PLANT CLEANING RECORDS


       
        //DELETE  BOIL PLANT CLEANING RECORDS
        [HttpPost]
        public ApiResponse DeleteBoilPlantClearingRecords([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDBoilPlantCleaningRecords.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDBoilPlantCleaningRecords.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteBoilPlantClearingAttech(line.Attachment);
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




        //BEGIN A LIST OF BOIL PLANT CLEANING RECORDS FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetBoilPlantClearingRecords()
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

                    string BoilPlantLocation = nvc["BoilPlantLocation"];
                    string DateOfCleaining = nvc["DateOfCleaining"];
                    string NameOfCleaner = nvc["NameOfCleaner"];
                    string PlantIncharge = nvc["PlantIncharge"];
                    string Remark = nvc["Remark"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDBoilPlantCleaningRecord> list = null;
                    try
                    {
                        list = db.tblHRDBoilPlantCleaningRecords.ToList();

                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfCleaining.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfCleaining.Date >= fromDate.Date && z.DateOfCleaining.Date <= toDate.Date).ToList();
                            }
                        }



                        //top filter
                        //if (!string.isnullorempty(startdate))
                        //{
                        //    //top filter
                        //    string[] fdate = startdate.split('/');
                        //    datetime fromdate = new datetime(convert.toint32(fdate[2]), convert.toint32(fdate[1]), convert.toint32(fdate[0]));
                        //    if (startdate == enddate)
                        //    {
                        //        list = list.asenumerable().where(z => z.expirydate.date == fromdate.date).tolist();
                        //    }
                        //    else
                        //    {//date range
                        //        string[] tdate = enddate.split('/');
                        //        datetime todate = new datetime(convert.toint32(tdate[2]), convert.toint32(tdate[1]), convert.toint32(tdate[0]));
                        //        list = list.asenumerable().where(z => z.expirydate.date >= fromdate.date && z.expirydate.date <= todate.date).tolist();
                        //    }
                        //}

                        //1. filter data
                        if (!string.IsNullOrEmpty(BoilPlantLocation) && BoilPlantLocation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.BoilPlantLocation.ToLower().Contains(BoilPlantLocation.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(NameOfCleaner) && NameOfCleaner != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfCleaner.ToLower().Contains(NameOfCleaner.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(PlantIncharge) && PlantIncharge != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.PlantIncharge.ToLower().Contains(PlantIncharge.ToLower())).ToList();
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
        public List<tblHRDBoilPlantCleaningRecord> DoSorting(IEnumerable<tblHRDBoilPlantCleaningRecord> list, string orderBy)
        {
            try
            {
                if(orderBy=="BoilPlantLocation")
                {
                    list = list.OrderByDescending(z => z.BoilPlantLocation).ToList();
                }
                else if (orderBy == "DateOfCleaining")
                {
                    list = list.OrderBy(z => z.DateOfCleaining).ToList();
                }
                else if (orderBy == "NameOfCleaner")
                {
                    list = list.OrderBy(z => z.NameOfCleaner).ToList();
                }
                else if (orderBy == "PlantIncharge")
                {
                    list = list.OrderByDescending(z => z.PlantIncharge).ToList();
                }
               





                return list.ToList<tblHRDBoilPlantCleaningRecord>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadBoilPlantCleaningRecords"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteBoilPlantClearingAttech(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadBoilPlantCleaningRecords"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }

}

   
