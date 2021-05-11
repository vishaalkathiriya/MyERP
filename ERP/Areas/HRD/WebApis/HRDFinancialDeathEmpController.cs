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

namespace ERP.WebApis
{
    public class HRDFinancialDeathEmpController : ApiController
    {


        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Financial Assis to Death Employee";

        public HRDFinancialDeathEmpController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse SaveFinancialDeathEmp(tblHRDFinancialAssisToDeathEmployee financialDeathEmp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (financialDeathEmp.SrNo == 0)
                    {// Mode == Add
                        tblHRDFinancialAssisToDeathEmployee d = new tblHRDFinancialAssisToDeathEmployee
                        {
                            Ecode = financialDeathEmp.Ecode,
                            EmployeeName = financialDeathEmp.EmployeeName,
                            DateOfDeath = financialDeathEmp.DateOfDeath,
                            Amount = financialDeathEmp.Amount,
                            ChequeNumber = financialDeathEmp.ChequeNumber,
                            ChequeIssueDate = financialDeathEmp.ChequeIssueDate,
                            ReceiveBy = financialDeathEmp.ReceiveBy,
                            Relation = financialDeathEmp.Relation,
                            FamilyBackgroundDetail = financialDeathEmp.FamilyBackgroundDetail,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())


                        };

                        db.tblHRDFinancialAssisToDeathEmployees.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);

                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDFinancialAssisToDeathEmployees.Where(z => z.SrNo == financialDeathEmp.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.Ecode = financialDeathEmp.Ecode;
                            line.EmployeeName = financialDeathEmp.EmployeeName;
                            line.DateOfDeath = financialDeathEmp.DateOfDeath;
                            line.Amount = financialDeathEmp.Amount;
                            line.ChequeNumber = financialDeathEmp.ChequeNumber;
                            line.ChequeIssueDate = financialDeathEmp.ChequeIssueDate;
                            line.ReceiveBy = financialDeathEmp.ReceiveBy;
                            line.Relation = financialDeathEmp.Relation;
                            line.FamilyBackgroundDetail = financialDeathEmp.FamilyBackgroundDetail;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
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
        public ApiResponse DeleteFinancialDeathEmp([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblHRDFinancialAssisToDeathEmployees.Where(z => z.SrNo == id).SingleOrDefault();
                if (line != null)
                {
                    db.tblHRDFinancialAssisToDeathEmployees.Remove(line);
                    db.SaveChanges();
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
        public ApiResponse GetFinancialDeathEmpList()
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


                    string Ecode = nvc["Ecode"];
                    string EmployeeName = nvc["EmployeeName"];
                    string DateOfDeath = nvc["DateOfDeath"];
                    string Amount = nvc["Amount"];
                    string ReceiveBy = nvc["ReceiveBy"];
                    string Relation = nvc["Relation"];

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDFinancialAssisToDeathEmployee> list = null;
                    try
                    {
                        list = db.tblHRDFinancialAssisToDeathEmployees.ToList();


                        //top filter
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.DateOfDeath.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.DateOfDeath.Date >= fromDate.Date && z.DateOfDeath.Date <= toDate.Date).ToList();
                            }
                        }

                        //1. filter data
                        if (!string.IsNullOrEmpty(Ecode) && Ecode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Ecode.ToLower().Contains(Ecode.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(EmployeeName) && EmployeeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EmployeeName.ToLower().Contains(EmployeeName.ToLower())).ToList();
                        }


                        if (!string.IsNullOrEmpty(ReceiveBy) && ReceiveBy != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ReceiveBy.ToLower().Contains(ReceiveBy.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Relation) && Relation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Relation.ToLower().Contains(Relation.ToLower())).ToList();
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


        public List<tblHRDFinancialAssisToDeathEmployee> DoSorting(IEnumerable<tblHRDFinancialAssisToDeathEmployee> list, string orderBy)
        {
            try
            {

                if (orderBy == "Ecode")
                {
                    list = list.OrderBy(z => z.Ecode).ToList();
                }
                else if (orderBy == "-Ecode")
                {
                    list = list.OrderByDescending(z => z.Ecode).ToList();
                }

                else if (orderBy == "EmployeeName")
                {
                    list = list.OrderBy(z => z.EmployeeName).ToList();
                }
                else if (orderBy == "-EmployeeName")
                {
                    list = list.OrderByDescending(z => z.EmployeeName).ToList();
                }
                else if (orderBy == "DateOfDeath")
                {
                    list = list.OrderBy(z => z.DateOfDeath).ToList();
                }
                else if (orderBy == "-DateOfDeath")
                {
                    list = list.OrderByDescending(z => z.DateOfDeath).ToList();
                }
                else if (orderBy == "Amount")
                {
                    list = list.OrderBy(z => z.Amount).ToList();
                }
                else if (orderBy == "-Amount")
                {
                    list = list.OrderByDescending(z => z.Amount).ToList();
                }
                else if (orderBy == "ReceiveBy")
                {
                    list = list.OrderBy(z => z.ReceiveBy).ToList();
                }
                else if (orderBy == "-ReceiveBy")
                {
                    list = list.OrderByDescending(z => z.ReceiveBy).ToList();
                }
                else if (orderBy == "Relation")
                {
                    list = list.OrderBy(z => z.Relation).ToList();
                }
                else if (orderBy == "-Relation")
                {
                    list = list.OrderByDescending(z => z.Relation).ToList();
                }

                else if (orderBy == "FamilyBackgroundDetail")
                {
                    list = list.OrderBy(z => z.FamilyBackgroundDetail).ToList();
                }
                else if (orderBy == "-FamilyBackgroundDetail")
                {
                    list = list.OrderByDescending(z => z.FamilyBackgroundDetail).ToList();
                }
                return list.ToList<tblHRDFinancialAssisToDeathEmployee>();
            }
            catch
            {
                return null;
            }
        }
    }
}