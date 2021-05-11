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
    public class HRDPressMediaExpController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Press Media Expense";

        public HRDPressMediaExpController() {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        
        [HttpPost]
        public ApiResponse SavePressMediaExp(tblHRDPressMediaExpens pressMediaExp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (pressMediaExp.SrNo == 0)
                    {// Mode == Add
                        tblHRDPressMediaExpens d = new tblHRDPressMediaExpens
                        {
                            NameOfPressMedia=pressMediaExp.NameOfPressMedia,
                            RepresentativeName=pressMediaExp.RepresentativeName,
                            Date=pressMediaExp.Date,
                            MobileNumber=pressMediaExp.MobileNumber,
                            Amount=pressMediaExp.Amount,
                            ApprovedBy=pressMediaExp.ApprovedBy,
                            Occasion=pressMediaExp.Occasion,
                            Attachment=pressMediaExp.Attachment,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };

                        db.tblHRDPressMediaExpenses.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDPressMediaExpenses.Where(z => z.SrNo == pressMediaExp.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.NameOfPressMedia = pressMediaExp.NameOfPressMedia;
                             line.RepresentativeName=pressMediaExp.RepresentativeName;
                             line.Date = pressMediaExp.Date;
                             line.MobileNumber = pressMediaExp.MobileNumber;
                             line.Amount = pressMediaExp.Amount;
                             line.ApprovedBy = pressMediaExp.ApprovedBy;
                             line.Occasion = pressMediaExp.Occasion;
                             line.Attachment = pressMediaExp.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        //    UpdateSequence(medHelp.SrNo);
                        //   UpdateSequence(mod.SeqNo, mod.ModuleId);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    MoveFile(pressMediaExp.Attachment);
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
        public ApiResponse DeletePressMediaExp([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDPressMediaExpenses.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDPressMediaExpenses.Remove(line);
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
        public ApiResponse GetPressmediaExpList()
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

                    string NameOfPressMedia = nvc["NameOfPressMedia"];
                    string RepresentativeName = nvc["RepresentativeName"]; 
                    string Date1 = nvc["Date1"]; 
                    string MobileNumber = nvc["MobileNumber"]; 
                    string Amount = nvc["Amount"]; 
                    string ApprovedBy = nvc["ApprovedBy"];
                    string Occasion = nvc["Occasion"]; 

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDPressMediaExpens> list = null;
                    try
                    {
                        list = db.tblHRDPressMediaExpenses.ToList();
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
                        if (!string.IsNullOrEmpty(NameOfPressMedia) && NameOfPressMedia != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfPressMedia.ToLower().Contains(NameOfPressMedia.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(RepresentativeName) && RepresentativeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.RepresentativeName.ToLower().Contains(RepresentativeName.ToLower())).ToList();
                        }


                        //if (!string.IsNullOrEmpty(Date1) && Date != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.Date.ToLower().Contains(Date1.ToLower())).ToList();
                        //}

                        if (!string.IsNullOrEmpty(ApprovedBy) && ApprovedBy != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ApprovedBy.ToLower().Contains(ApprovedBy.ToLower())).ToList();
                        }

                        //if (!string.IsNullOrEmpty(ExpenseAmount) && ExpenseAmount != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.ExpenseAmount.ToLower().Contains(ExpenseAmount.ToLower())).ToList();
                        //}

                        if (!string.IsNullOrEmpty(Occasion) && Occasion != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Occasion.Contains(Occasion.ToLower())).ToList();
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


        public List<tblHRDPressMediaExpens> DoSorting(IEnumerable<tblHRDPressMediaExpens> list, string orderBy)
        {
            try
            {

                if (orderBy == "NameOfPressMedia")
                {
                    list = list.OrderBy(z => z.NameOfPressMedia).ToList();
                }
                else if (orderBy == "-NameOfPressMedia")
                {
                    list = list.OrderByDescending(z => z.NameOfPressMedia).ToList();
                }

                else if (orderBy == "RepresentativeName")
                {
                    list = list.OrderBy(z => z.RepresentativeName).ToList();
                }
                else if (orderBy == "-RepresentativeName")
                {
                    list = list.OrderByDescending(z => z.RepresentativeName).ToList();
                }
                else if (orderBy == "Date")
                {
                    list = list.OrderBy(z => z.Date).ToList();
                }
                else if (orderBy == "-Date")
                {
                    list = list.OrderByDescending(z => z.Date).ToList();
                }
                else if (orderBy == "MobileNumber")
                {
                    list = list.OrderBy(z => z.MobileNumber).ToList();
                }
                else if (orderBy == "-MobileNumber")
                {
                    list = list.OrderByDescending(z => z.MobileNumber).ToList();
                }
                else if (orderBy == "Amount")
                {
                    list = list.OrderBy(z => z.Amount).ToList();
                }
                else if (orderBy == "-Amount")
                {
                    list = list.OrderByDescending(z => z.Amount).ToList();
                }
                else if (orderBy == "ApprovedBy")
                {
                    list = list.OrderBy(z => z.ApprovedBy).ToList();
                }
                else if (orderBy == "-ApprovedBy")
                {
                    list = list.OrderByDescending(z => z.ApprovedBy).ToList();
                }
                else if (orderBy == "Occasion")
                {
                    list = list.OrderBy(z => z.Occasion).ToList();
                }
                else if (orderBy == "-Occasion")
                {
                    list = list.OrderByDescending(z => z.Occasion).ToList();
                }

                return list.ToList<tblHRDPressMediaExpens>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadPressMediaExpense"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadPressMediaExpense"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion
    }
}