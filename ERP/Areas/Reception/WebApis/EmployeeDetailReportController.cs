using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data;
using System.Data.SqlClient;
using ERP.Areas.Reception.Classes;

namespace ERP.Areas.Reception.WebApis
{
    public class EmployeeDetailReportController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Employee-Detail Report";

        public EmployeeDetailReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/EmployeeDetailReport
        /// retrieve Dept list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDeptList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();
                    //For ReceptionContext_Emp
                    //dt = General.GetDatatableQuery_EmpDtl("select dept from Employee where gname in ('dd','mg','xi') group by Dept order by Dept");

                    //ERPContext
                    dt = General.GetDatatableQuery_EmpDtl("select department from EmployeeMaster where gname in ('DD','DEC','DI','SG','FM') group by department order by department");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
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

        /// <summary>
        /// GET api/EmployeeDetailReport
        /// return employee detail report with sorting and filtering  functionalities
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public ApiResponse GetEmployeeDetailReport(EmployeeDetailReportViewModel reportModel)
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
                    string filter1 = nvc["filter1"];
                    string filter2 = nvc["filter2"];
                    string filter3 = nvc["filter3"];
                    string filter4 = nvc["filter4"];
                    string filter5 = nvc["filter5"];
                    string filter6 = nvc["filter6"];
                    string filter7 = nvc["filter7"];
                    string filter8 = nvc["filter8"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    DataTable dt = new DataTable();
                    try
                    {
                        string Qry = "";

                        //Filter FirstName
                        if (!string.IsNullOrEmpty(reportModel.FirstName) && reportModel.FirstName != "undefined")
                        {
                            Qry += "  and FirstName like '%" + reportModel.FirstName + "%'";
                        }

                        //Filter MiddleName
                        if (!string.IsNullOrEmpty(reportModel.MiddleName) && reportModel.MiddleName != "undefined")
                        {
                            Qry += " and MiddleName like '%" + reportModel.MiddleName + "%'";
                        }

                        //Filter LastName
                        if (!string.IsNullOrEmpty(reportModel.LastName) && reportModel.LastName != "undefined")
                        {
                            Qry += " and LastName like '%" + reportModel.LastName + "%'";
                        }

                        //Filter Dept
                        if (!string.IsNullOrEmpty(reportModel.Department) && reportModel.Department != "undefined" && reportModel.Department != "0")
                        {
                            Qry += " and Department like '%" + reportModel.Department + "%'"; //Reception
                        }

                        if (Qry == "")
                        {
                            dt = General.GetDatatableQuery_EmpDtl("select ECode,Replace(Replace(Replace(FirstName,' ',''),'BHAI',''),'BEN','') + ' ' + MiddleName + ' ' + LastName as FullName,Designation,Department,Manager,ManCode,Intercom as ICom,case when (select count(*) From inoutentry where ecode = q1.ecode and status = 'in'  and dateadd(d,0,datediff(d,0,InOutTime)) = Convert(varchar(10),getdate(),101)) > 0 then 'Yes' else 'No' end as Present From EmployeeMaster q1 where q1.Designation <> 'Worker' and q1.GName in ('DD','DEC','DI','SG','FM') and (Leavedate is null or LeaveDate = '01/01/1900') and 1=0" + Qry);
                        }
                        else
                        {
                            //For ERPContext
                            dt = General.GetDatatableQuery_EmpDtl("select top 1000 ECode,Replace(Replace(Replace(FirstName,' ',''),'BHAI',''),'BEN','') + ' ' + MiddleName + ' ' + LastName as FullName,Designation,Department,Manager,ManCode,Intercom as ICom,case when (select count(*) From inoutentry where ecode = q1.ecode and status = 'in'  and dateadd(d,0,datediff(d,0,InOutTime)) = Convert(varchar(10),getdate(),101)) > 0 then 'Yes' else 'No' end as Present From EmployeeMaster q1 where q1.Designation <> 'Worker' and q1.GName in ('DD','DEC','DI','SG','FM') and (Leavedate is null or LeaveDate = '01/01/1900') " + Qry);
                        }

                        ////Sorting Datatable
                        if (orderBy.Trim().Contains("-"))
                        {//descending
                            orderBy = orderBy.Trim().Replace("-", "");
                            var Rows = (from row in dt.AsEnumerable()
                                        orderby row[orderBy] descending
                                        select row);
                            dt = Rows.AsDataView().ToTable();
                        }
                        else
                        {//ascending
                            orderBy = orderBy.Trim().Replace("-", "");
                            var Rows = (from row in dt.AsEnumerable()
                                        orderby row[orderBy] ascending
                                        select row);
                            dt = Rows.AsDataView().ToTable();
                        }

                        //Filter Ecode
                        if (!string.IsNullOrEmpty(filter1) && filter1 != "undefined" && filter1 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<decimal>("ECode") == Convert.ToDecimal(filter1)
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter FullName
                        if (!string.IsNullOrEmpty(filter2) && filter2 != "undefined" && filter2 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("FullName").ToString().ToLower().Contains(filter2.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Post
                        if (!string.IsNullOrEmpty(filter3) && filter3 != "undefined" && filter3 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Designation").ToString().ToLower().Contains(filter3.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Department
                        if (!string.IsNullOrEmpty(filter4) && filter4 != "undefined" && filter4 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Department").ToString().ToLower().Contains(filter4.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Manager
                        if (!string.IsNullOrEmpty(filter5) && filter5 != "undefined" && filter5 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Manager").ToString().ToLower().Contains(filter5.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }

                        //Filter ManCode
                        if (!string.IsNullOrEmpty(filter6) && filter6 != "undefined" && filter6 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("ManCode").ToString().ToLower().Equals(filter6.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }

                        //Filter ICom
                        if (!string.IsNullOrEmpty(filter7) && filter7 != "undefined" && filter7 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("ICom").ToString().ToLower().Equals(filter7.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }

                        //Filter Present
                        if (!string.IsNullOrEmpty(filter8) && filter8 != "undefined" && filter8 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Present").ToString().ToLower().Equals(filter8.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }

                        var count = dt.Rows.Count;

                        var resultData = new
                        {
                            total = count,
                            result = count > 0 ? dt.AsEnumerable().Skip(iDisplayStart).Take(iDisplayLength).CopyToDataTable() : dt
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);

                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {

                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

    }

    public partial class EmployeeDetailReportViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
    }
}
