
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
    public class HRDFireExtinguiserLogBookController : ApiController
    {
    ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Fire Extinguiser Log Book";


        public HRDFireExtinguiserLogBookController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveFireExtinuiserLogBook(tblHRDFireExtinguiserLogBook FireExtinguiser)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (FireExtinguiser.SrNo == 0)
                    {
                        tblHRDFireExtinguiserLogBook d = new tblHRDFireExtinguiserLogBook
                            {
                                TypeOfFireExtinguiser = FireExtinguiser.TypeOfFireExtinguiser,
                                Capacity = FireExtinguiser.Capacity,
                                Location = FireExtinguiser.Location,
                                DateOfInspection = FireExtinguiser.DateOfInspection,
                                UsedOfFireExtinguiser = FireExtinguiser.UsedOfFireExtinguiser,
                                DateOfRefilling = FireExtinguiser.DateOfRefilling,
                                DueDateForNextRefilling = FireExtinguiser.DueDateForNextRefilling,
                                Reason = FireExtinguiser.Reason,
                                Remark = FireExtinguiser.Remark,
                                Attachment = FireExtinguiser.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDFireExtinguiserLogBooks.Add(d);
                            MoveFile(FireExtinguiser.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDFireExtinguiserLogBooks.Where(z => z.SrNo == FireExtinguiser.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.TypeOfFireExtinguiser = FireExtinguiser.TypeOfFireExtinguiser;
                                line.Capacity = FireExtinguiser.Capacity;
                                line.Location = FireExtinguiser.Location;
                                line.DateOfInspection = FireExtinguiser.DateOfInspection;
                                line.UsedOfFireExtinguiser = FireExtinguiser.UsedOfFireExtinguiser;
                                line.DateOfRefilling = FireExtinguiser.DateOfRefilling;
                                line.DueDateForNextRefilling = FireExtinguiser.DueDateForNextRefilling;
                                line.Reason = FireExtinguiser.Reason;
                                line.Remark = FireExtinguiser.Remark;
                                line.Attachment = FireExtinguiser.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(FireExtinguiser.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                }
                catch (Exception ee)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ee, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        //END CREATE OR UPDATE FIRE EXTINGUISER


       
        //DELETE FIRE EXTINGUISER LOG BOOK
        [HttpPost]
        public ApiResponse DeleteFireExtinuiserLogBook([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDFireExtinguiserLogBooks.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDFireExtinguiserLogBooks.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteFireExtensibleAttech(line.Attachment);
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




        //BEGIN A LIST OF FIRE EXTENSIBLE FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetFireExtinguiserLogBook()
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

                    string TypeOfFireExtinguiser = nvc["TypeOfFireExtinguiser"];
                    string Capacity = nvc["Capacity"];
                    string Location = nvc["Location"];
                    string DateOfInspection = nvc["DateOfInspection"];
                    string UsedOfFireExtinguiser = nvc["UsedOfFireExtinguiser"];
                    string DateOfRefilling = nvc["DateOfRefilling"];
                    string DueDateForNextRefilling = nvc["DueDateForNextRefilling"];
                    string Reason = nvc["Reason"];
                    string Remark = nvc["Remark"];
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDFireExtinguiserLogBook> list = null;
                    try
                    {
                        list = db.tblHRDFireExtinguiserLogBooks.ToList();

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
                        if (!string.IsNullOrEmpty(TypeOfFireExtinguiser) && TypeOfFireExtinguiser != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypeOfFireExtinguiser.ToLower().Contains(TypeOfFireExtinguiser.ToLower())).ToList();
                        }
                        
                        if (!string.IsNullOrEmpty(Location) && Location != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Location.ToLower().Contains(Location.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(UsedOfFireExtinguiser) && UsedOfFireExtinguiser != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.UsedOfFireExtinguiser.ToLower().Contains(UsedOfFireExtinguiser.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(Reason) && Reason != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Reason.ToLower().Contains(Reason.ToLower())).ToList();
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
        public List<tblHRDFireExtinguiserLogBook> DoSorting(IEnumerable<tblHRDFireExtinguiserLogBook> list, string orderBy)
        {
            try
            {
                if (orderBy == "TypeOfFireExtinguiser")
                {
                    list = list.OrderByDescending(z => z.TypeOfFireExtinguiser).ToList();
                }
                else if (orderBy == "Capacity")
                {
                    list = list.OrderBy(z => z.Capacity).ToList();
                }
                else if (orderBy == "Location")
                {
                    list = list.OrderByDescending(z => z.Location).ToList();
                }
                else if (orderBy == "DateOfInspection")
                {
                    list = list.OrderBy(z => z.DateOfInspection).ToList();
                }
                else if (orderBy == "UsedOfFireExtinguiser")
                {
                    list = list.OrderBy(z => z.UsedOfFireExtinguiser).ToList();
                }
                else if (orderBy == "DateOfRefilling")
                {
                    list = list.OrderBy(z => z.DateOfRefilling).ToList();
                }
                else if (orderBy == "DueDateForNextRefilling")
                {
                    list = list.OrderBy(z => z.DueDateForNextRefilling).ToList();
                }
                






                return list.ToList<tblHRDFireExtinguiserLogBook>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadFireExtenguiserLogBook"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteFireExtensibleAttech(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadFireExtenguiserLogBook"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }
}