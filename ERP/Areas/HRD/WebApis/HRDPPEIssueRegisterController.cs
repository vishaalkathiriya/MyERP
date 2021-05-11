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

namespace ERP.WebApis
{
    public class HRDPPEIssueRegisterController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "PPE Issue Register";

        public HRDPPEIssueRegisterController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SavePPEIssueRegister(tblHRDPPEIssueRegister PPEIssueRegister)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (PPEIssueRegister.SrNo == 0)
                    {// Mode == Add
                        tblHRDPPEIssueRegister d = new tblHRDPPEIssueRegister
                        {
                            NameOfIssuer = PPEIssueRegister.NameOfIssuer,
                            NameOfRecievr = PPEIssueRegister.NameOfRecievr,
                            TypeOfPPE = PPEIssueRegister.TypeOfPPE,
                            Quanity = PPEIssueRegister.Quanity,
                            Department = PPEIssueRegister.Department,
                            ManagerName = PPEIssueRegister.ManagerName,
                            Price = PPEIssueRegister.Price,
                            Remarks = PPEIssueRegister.Remarks,
                            Attachment = PPEIssueRegister.Attachment,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())


                        };

                        db.tblHRDPPEIssueRegisters.Add(d);
                        MoveFile(PPEIssueRegister.Attachment);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDPPEIssueRegisters.Where(z => z.SrNo == PPEIssueRegister.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.NameOfIssuer = PPEIssueRegister.NameOfIssuer;
                            line.NameOfRecievr = PPEIssueRegister.NameOfRecievr;
                            line.TypeOfPPE = PPEIssueRegister.TypeOfPPE;
                            line.Quanity = PPEIssueRegister.Quanity;
                            line.Department = PPEIssueRegister.Department;
                            line.ManagerName = PPEIssueRegister.ManagerName;
                            line.Price = PPEIssueRegister.Price;
                            line.Remarks = PPEIssueRegister.Remarks;
                            line.Attachment = PPEIssueRegister.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        MoveFile(PPEIssueRegister.Attachment);
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

        [HttpPost]
        public ApiResponse DeletePPEIssueRegister([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDPPEIssueRegisters.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDPPEIssueRegisters.Remove(line);
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                        DeletePPEIssueRegister(line.Attachment);
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
        public ApiResponse GetPPEIssueRegister()
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
                    string NameOfRecievr = nvc["NameOfRecievr"];
                    string TypeOfPPE = nvc["TypeOfPPE"];
                    string Quanity = nvc["Quanity"];
                    string Department = nvc["Department"];
                    string ManagerName = nvc["ManagerName"];
                    string Price = nvc["Price"];
                    string Remarks = nvc["Remarks"];
                    // string Attachment = nvc["Attachment"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDPPEIssueRegister> list = null;
                    try
                    {
                        list = db.tblHRDPPEIssueRegisters.ToList();

                        //top filter
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

                        //1. filter data

                        if (!string.IsNullOrEmpty(NameOfIssuer) && NameOfIssuer != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfIssuer.ToLower().Contains(NameOfIssuer.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(NameOfRecievr) && NameOfRecievr != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.NameOfRecievr.ToLower().Contains(NameOfRecievr.ToLower())).ToList();
                        }


                        if (!string.IsNullOrEmpty(TypeOfPPE) && TypeOfPPE != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypeOfPPE.ToLower().Contains(TypeOfPPE.ToLower())).ToList();
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


        public List<tblHRDPPEIssueRegister> DoSorting(IEnumerable<tblHRDPPEIssueRegister> list, string orderBy)
        {
            try
            {

                if (orderBy == "NameOfIssuer")
                {
                    list = list.OrderBy(z => z.NameOfIssuer).ToList();
                }
                else if (orderBy == "-NameOfRecievr")
                {
                    list = list.OrderByDescending(z => z.NameOfRecievr).ToList();
                }

                else if (orderBy == "TypeOfPPE")
                {
                    list = list.OrderBy(z => z.TypeOfPPE).ToList();
                }
                else if (orderBy == "-Quanity")
                {
                    list = list.OrderByDescending(z => z.Quanity).ToList();
                }
                else if (orderBy == "Department")
                {
                    list = list.OrderByDescending(z => z.Department).ToList();
                }
                else if (orderBy == "-ManagerName")
                {
                    list = list.OrderByDescending(z => z.ManagerName).ToList();
                }
                else if (orderBy == "Price")
                {
                    list = list.OrderByDescending(z => z.Price).ToList();
                }
                else if (orderBy == "-Remarks")
                {
                    list = list.OrderByDescending(z => z.Remarks).ToList();
                }
                else if (orderBy == "Attachment")
                {
                    list = list.OrderBy(z => z.Attachment).ToList();
                }

                return list.ToList<tblHRDPPEIssueRegister>();
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadPPEIssueRegister"].ToString()) + "/" + file;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }


        private void DeletePPEIssueRegister(string Attch)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadPPEIssueRegister"].ToString());

            if (File.Exists(Path.Combine(mainPath, Attch)))
            {
                File.Delete(Path.Combine(mainPath, Attch));
            }

        }
        #endregion

    }
}
