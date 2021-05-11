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
    public class HRDFirstAIdLogBookController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "First Aid Log Book";


        public HRDFirstAIdLogBookController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveFirstAIdLogBook(tblHRDFirstAIdLogBook FirstAIdLogBook)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (FirstAIdLogBook.SrNo == 0)
                    {
                            tblHRDFirstAIdLogBook d = new tblHRDFirstAIdLogBook
                            {
                                NameOfIssuer = FirstAIdLogBook.NameOfIssuer,
                                NameOfReceiver = FirstAIdLogBook.NameOfReceiver,
                                NameOfFirstAIdItems = FirstAIdLogBook.NameOfFirstAIdItems,
                                DateOfIssue = FirstAIdLogBook.DateOfIssue,
                                Quanity = FirstAIdLogBook.Quanity,
                                Size = FirstAIdLogBook.Size,
                                ManagerName = FirstAIdLogBook.ManagerName,
                                LocationOfFirstAIdBox = FirstAIdLogBook.LocationOfFirstAIdBox,
                                Price = FirstAIdLogBook.Price,
                                ExpiryDate = FirstAIdLogBook.ExpiryDate,
                                Remarks = FirstAIdLogBook.Remarks,
                                Attachment=FirstAIdLogBook.Attachment,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };

                            db.tblHRDFirstAIdLogBooks.Add(d);
                            MoveFile(FirstAIdLogBook.Attachment);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {
                            var line = db.tblHRDFirstAIdLogBooks.Where(z => z.SrNo == FirstAIdLogBook.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.NameOfIssuer = FirstAIdLogBook.NameOfIssuer;
                                line.NameOfReceiver = FirstAIdLogBook.NameOfReceiver;
                                line.NameOfFirstAIdItems = FirstAIdLogBook.NameOfFirstAIdItems;
                                line.DateOfIssue = FirstAIdLogBook.DateOfIssue;
                                line.Quanity = FirstAIdLogBook.Quanity;
                                line.Size = FirstAIdLogBook.Size;
                                line.ManagerName = FirstAIdLogBook.ManagerName;
                                line.LocationOfFirstAIdBox = FirstAIdLogBook.LocationOfFirstAIdBox;
                                line.Price = FirstAIdLogBook.Price;
                                line.ExpiryDate = FirstAIdLogBook.ExpiryDate;
                                line.Remarks = FirstAIdLogBook.Remarks;
                                line.Attachment = FirstAIdLogBook.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            MoveFile(FirstAIdLogBook.Attachment);
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
        //END CREATE OR UPDATE FIRSTAID LOG BOOK


       
        //DELETE FIRST AID LOG BOOK
        [HttpPost]
        public ApiResponse DeleteFirstAIdLogBook([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDFirstAIdLogBooks.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDFirstAIdLogBooks.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                      DeleteFirstAIdLogBookAttachment(line.Attachment);
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




        //BEGIN A LIST OF FIRST AID LOG BOOK FILTERING AND SORTING
        [HttpGet]
        public ApiResponse GetFirstAIdLogBook()
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

                    string NameOfIssuer = nvc["NameOfIssuer"];
                    string NameOfReceiver = nvc["NameOfReceiver"];
                    string NameOfFirstAIdItems = nvc["NameOfFirstAIdItems"];
                    string DateOfIssue = nvc["DateOfIssue"];
                    string Quanity = nvc["Quanity"];
                    string Size = nvc["Size"];
                    string ManagerName = nvc["ManagerName"];
                    string LocationOfFirstAIdBox = nvc["LocationOfFirstAIdBox"];
                    string Price = nvc["Price"];
                    string ExpiryDate = nvc["ExpiryDate"];
                    string Remarks = nvc["Remarks"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDFirstAIdLogBook> list = null;
                    try
                    {
                        list = db.tblHRDFirstAIdLogBooks.ToList();

                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfIssue.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfIssue.Date >= fromDate.Date && z.DateOfIssue.Date <= toDate.Date).ToList();
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
                        if (!string.IsNullOrEmpty(NameOfIssuer) && NameOfIssuer != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfIssuer.ToLower().Contains(NameOfIssuer.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(NameOfReceiver) && NameOfReceiver != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfReceiver.ToLower().Contains(NameOfReceiver.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(NameOfFirstAIdItems) && NameOfFirstAIdItems != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfFirstAIdItems.ToLower().Contains(NameOfFirstAIdItems.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(ManagerName) && ManagerName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ManagerName.ToLower().Contains(ManagerName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(LocationOfFirstAIdBox) && LocationOfFirstAIdBox != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.LocationOfFirstAIdBox.ToLower().Contains(LocationOfFirstAIdBox.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(Remarks) && Remarks != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Remarks.ToLower().Contains(Remarks.ToLower())).ToList();
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
        public List<tblHRDFirstAIdLogBook> DoSorting(IEnumerable<tblHRDFirstAIdLogBook> list, string orderBy)
        {
            try
            {
                if(orderBy=="NameOfIssuer")
                {
                    list = list.OrderBy(z => z.NameOfIssuer).ToList();
                }
                else if (orderBy == "NameOfReceiver")
                {
                    list = list.OrderBy(z => z.NameOfReceiver).ToList();
                }
                else if (orderBy == "NameOfFirstAIdItems")
                {
                    list = list.OrderBy(z => z.NameOfFirstAIdItems).ToList();
                }
                else if (orderBy == "DateOfIssue")
                {
                    list = list.OrderBy(z => z.DateOfIssue).ToList();
                }
                else if (orderBy == "Quanity")
                {
                    list = list.OrderBy(z => z.Quanity).ToList();
                }
                else if (orderBy == "Size")
                {
                    list = list.OrderBy(z => z.Size).ToList();
                }
                else if (orderBy == "ManagerName")
                {
                    list = list.OrderBy(z => z.ManagerName).ToList();
                }
                else if (orderBy == "LocationOfFirstAIdBox")
                {
                    list = list.OrderBy(z => z.LocationOfFirstAIdBox).ToList();
                }
                else if (orderBy == "Price")
                {
                    list = list.OrderBy(z => z.Price).ToList();
                }
                else if (orderBy == "ExpiryDate")
                {
                    list = list.OrderBy(z => z.ExpiryDate).ToList();
                }






                return list.ToList<tblHRDFirstAIdLogBook>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadFirstAIdLogBook"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeleteFirstAIdLogBookAttachment(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadFirstAIdLogBook"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion
    }
}
