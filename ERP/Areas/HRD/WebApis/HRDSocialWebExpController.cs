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
    public class HRDSocialWebExpController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Social Welfare Expense";

        public HRDSocialWebExpController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

            //BEGIN CREATE AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION WITH WEB API        
            [HttpPost]
            public ApiResponse SaveSocialWebExp(tblHRDSocialWelfareExpense socialWebExp)
            {
                ApiResponse apiResponse = new ApiResponse();
                if (sessionUtils.HasUserLogin())
                {
                    try
                    {
                        if (socialWebExp.SrNo == 0)
                        {// Mode == Add
                            tblHRDSocialWelfareExpense d = new tblHRDSocialWelfareExpense
                            {
                              ProgrammeName=socialWebExp.ProgrammeName,
                              Venue=socialWebExp.Venue,
                              Date = socialWebExp.Date,
                              Time=socialWebExp.Time,
                          //    ExpenseAmount=socialWebExp.ExpenseAmount,
                              ExpenseAmount = 0,
                              GuestName=socialWebExp.GuestName,
                              Attachment=socialWebExp.Attachment,
                              CreDate=DateTime.Now.ToUniversalTime(),
                              ChgDate=DateTime.Now.ToUniversalTime(),
                              CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                              ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())

                                                     
                            };

                            db.tblHRDSocialWelfareExpenses.Add(d);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                        }
                        else
                        {// Mode == Edit
                            var line = db.tblHRDSocialWelfareExpenses.Where(z => z.SrNo == socialWebExp.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.ProgrammeName = socialWebExp.ProgrammeName;
                                line.Venue = socialWebExp.Venue;
                                line.Date = socialWebExp.Date;
                                line.Time = socialWebExp.Time;
                                line.GuestName = socialWebExp.GuestName;
                              //   line.ExpenseAmount = socialWebExp.ExpenseAmount;
                                line.ExpenseAmount = 0;
                                line.Attachment = socialWebExp.Attachment;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            //    UpdateSequence(medHelp.SrNo);
                            //   UpdateSequence(mod.SeqNo, mod.ModuleId);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                        }
                        MoveFile(socialWebExp.Attachment);
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
            //END CREATE AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION WITH WEB API





        //BEGIN DELETE SOCIAL WELFARE EXPENSE INFORMATION BASE SRNO
        [HttpPost]
        public ApiResponse DeleteSocialWebExp([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
                try
                {
                        var line = db.tblHRDSocialWelfareExpenses.Where(z => z.SrNo == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblHRDSocialWelfareExpenses.Remove(line);
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
        //END DELETE SOCIAL WELFARE EXPENSE INFORMATION BASE SRNO


        //BEGIN A LIST OF SOCIAL WELFARE EXPENSE INFORMATION WITH SORTING AND FILTERING
        [HttpGet]
        public ApiResponse GetSocialWebExpList()
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
                    string ProgrammeName = nvc["ProgrammeName"]; //for col Prorram name
                    string Venue = nvc["Venue"]; //for col Venue
                    string Date1 = nvc["Date1"]; //for col Date 
                    string Time = nvc["Time"]; //for col Time 
                    string ExpenseAmount = nvc["ExpenseAmount"]; //for col Expense Amount
                    string GuestName = nvc["GuestName"]; //for col Guest Name

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDSocialWelfareExpense> list = null;
                    try
                    {
                        list = db.tblHRDSocialWelfareExpenses.ToList();


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
                        if (!string.IsNullOrEmpty(ProgrammeName) && ProgrammeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ProgrammeName.ToLower().Contains(ProgrammeName.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Venue) && Venue != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Venue.ToLower().Contains(Venue.ToLower())).ToList();
                        }


                        //if (!string.IsNullOrEmpty(Date1) && Date != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.Date.ToLower().Contains(Date1.ToLower())).ToList();
                        //}

                        //if (!string.IsNullOrEmpty(Time) && Time != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.Time.ToLower().Contains(Time.ToLower())).ToList();
                        //}

                        //if (!string.IsNullOrEmpty(ExpenseAmount) && ExpenseAmount != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.ExpenseAmount.ToLower().Contains(ExpenseAmount.ToLower())).ToList();
                        //}

                        if (!string.IsNullOrEmpty(GuestName) && GuestName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.GuestName.Contains(GuestName.ToLower())).ToList();
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



        //BEGIN A LIST OF SOCIAL WELFARE EXPENSE INFORMATION BASE COLUMN WISE SORTING
        public List<tblHRDSocialWelfareExpense> DoSorting(IEnumerable<tblHRDSocialWelfareExpense> list, string orderBy)
        {
            try
            {

                if (orderBy == "ProgrammeName")
                {
                    list = list.OrderBy(z => z.ProgrammeName).ToList();
                }
                else if (orderBy == "-ProgrammeName")
                {
                    list = list.OrderByDescending(z => z.ProgrammeName).ToList();
                }

                else if (orderBy == "Venue")
                {
                    list = list.OrderBy(z => z.Venue).ToList();
                }
                else if (orderBy == "-Venue")
                {
                    list = list.OrderByDescending(z => z.Venue).ToList();
                }
                else if (orderBy == "Date")
                {
                    list = list.OrderBy(z => z.Date).ToList();
                }
                else if (orderBy == "-Date")
                {
                    list = list.OrderByDescending(z => z.Date).ToList();
                }
                else if (orderBy == "Time")
                {
                    list = list.OrderBy(z => z.Time).ToList();
                }
                else if (orderBy == "-Time")
                {
                    list = list.OrderByDescending(z => z.Time).ToList();
                }
                else if (orderBy == "ExpenseAmount")
                {
                    list = list.OrderBy(z => z.ExpenseAmount).ToList();
                }
                else if (orderBy == "-ExpenseAmount")
                {
                    list = list.OrderByDescending(z => z.ExpenseAmount).ToList();
                }
                else if (orderBy == "GuestName")
                {
                    list = list.OrderBy(z => z.GuestName).ToList();
                }
                else if (orderBy == "-GuestName")
                {
                    list = list.OrderByDescending(z => z.GuestName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }


                return list.ToList<tblHRDSocialWelfareExpense>();
            }
            catch
            {
                return null;
            }
        }
        //END A LIST OF SOCIAL WELFARE EXPENSE INFORMATION BASE COLUMN WISE SORTING


        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadSocialWelfareExpense"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
               // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadSocialWelfareExpense"].ToString());
         
            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }
         
        }
        #endregion





    }
}