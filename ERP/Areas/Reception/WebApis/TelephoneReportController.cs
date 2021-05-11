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
    public class TelephoneReportController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Telephone Report";

        public TelephoneReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/TelephoneReport
        /// retrieve EX Type list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveExtTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt = General.GetDatatableQuery_Teledata("select Status as type from tblCall_Log group by Status order by Status");
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
        /// GET api/TelephoneReport
        /// return telephone outer report with sorting and filtering  functionalities
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public ApiResponse GetTelephoneReportOuter(TeleOuterReportViewModel reportModel)
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

                    #region "Set StartDate-EndDate"
                    DateTime fromDate, toDate;

                    string startDate = reportModel.StartDate;
                    string endDate = reportModel.EndDate;

                    if (!string.IsNullOrEmpty(startDate))
                    {
                        string[] fdate = startDate.Split('/');
                        fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                    }
                    else
                    {
                        fromDate = Convert.ToDateTime("1/1/1900");
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        string[] tdate = endDate.Split('/');
                        toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0])).AddDays(1);
                    }
                    else
                    {
                        toDate = Convert.ToDateTime("1/1/1900");
                    }
                    #endregion

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    DataTable dt = new DataTable();
                    try
                    {
                        string Qry = "";

                        //Filter Ext-Type
                        if (!string.IsNullOrEmpty(reportModel.ExtType) && reportModel.ExtType != "undefined" && reportModel.ExtType != "0")
                        {
                            Qry += " and Status = '" + reportModel.ExtType + "'";
                        }

                        //Filter Ext-No
                        if (!string.IsNullOrEmpty(reportModel.ExtNo) && reportModel.ExtNo != "undefined" && reportModel.ExtNo != "0")
                        {
                            Qry += " and FromExt = '" + reportModel.ExtNo + "'";
                        }

                        //Filter Ext-No
                        if (!string.IsNullOrEmpty(reportModel.OutNo) && reportModel.OutNo != "undefined" && reportModel.OutNo != "0")
                        {
                            Qry += " and OutLineNo like '%" + reportModel.OutNo + "%'";
                        }

                        //Filter PDate
                        if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        {
                            //var from = reportModel.StartDate.Split('/');
                            //var f = from[1] + "/" + from[0] + "/" + from[2];
                            var f = fromDate.Date.ToString("MM/dd/yyyy");
                            //var to = reportModel.EndDate.Split('/');
                            //var t = to[1] + "/" + (Convert.ToInt16(to[0]) + 1) + "/" + to[2];
                            var t = toDate.Date.ToString("MM/dd/yyyy");
                            Qry += "and pdate Between '" + f + "' And '" + t + "'";
                        }

                        dt = General.GetDatatableQuery_Teledata("select srno [SrNo], Status [Type], pdate, Line [Outline], duration [Duration], FromExt as [ExtNo], OutLineNo [OutNo] from tblCall_Log where 1=1   " + Qry + " order by pdate desc");
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

                        //Filter Type
                        if (!string.IsNullOrEmpty(filter1) && filter1 != "undefined" && filter1 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Type").ToString().ToLower().Contains(filter1.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Outline
                        if (!string.IsNullOrEmpty(filter2) && filter2 != "undefined" && filter2 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Outline").ToString().ToLower().Contains(filter2.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Duration
                        if (!string.IsNullOrEmpty(filter3) && filter3 != "undefined" && filter3 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<decimal>("Duration").ToString().ToLower().Contains(filter3.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter ExtNo
                        if (!string.IsNullOrEmpty(filter4) && filter4 != "undefined" && filter4 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("ExtNo").ToString().ToLower().Contains(filter4.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter OutNo
                        if (!string.IsNullOrEmpty(filter5) && filter5 != "undefined" && filter5 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("OutNo").ToString().ToLower().Contains(filter5.ToLower())
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

        /// <summary>
        /// GET api/TelephoneReport
        /// return telephone inter report with sorting and filtering  functionalities
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public ApiResponse GetTelephoneReportInter(TeleInterReportViewModel reportModel)
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

                    #region "Set StartDate-EndDate"
                    DateTime fromDate, toDate;

                    string startDate = reportModel.StartDate;
                    string endDate = reportModel.EndDate;

                    if (!string.IsNullOrEmpty(startDate))
                    {
                        string[] fdate = startDate.Split('/');
                        fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                    }
                    else
                    {
                        fromDate = Convert.ToDateTime("1/1/1900");
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        string[] tdate = endDate.Split('/');
                        toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0])).AddDays(1);
                    }
                    else
                    {
                        toDate = Convert.ToDateTime("1/1/1900");
                    }
                    #endregion

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    DataTable dt = new DataTable();
                    try
                    {
                        string Qry = "";

                        //Filter FromExt
                        if (!string.IsNullOrEmpty(reportModel.FromExt) && reportModel.FromExt != "undefined" && reportModel.FromExt != "0")
                        {
                            Qry += " and FromExt = '" + reportModel.FromExt + "'";
                        }

                        //Filter ToExt
                        if (!string.IsNullOrEmpty(reportModel.ToExt) && reportModel.ToExt != "undefined" && reportModel.ToExt != "0")
                        {
                            Qry += " and ToExt = '" + reportModel.ToExt + "'";
                        }

                        //Filter PDate
                        if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        {
                            //var from = reportModel.StartDate.Split('/');
                            //var f = from[1] + "/" + from[0] + "/" + from[2];
                            var f = fromDate.Date.ToString("MM/dd/yyyy");
                            //var to = reportModel.EndDate.Split('/');
                            //var t = to[1] + "/" + (Convert.ToInt16(to[0]) + 1) + "/" + to[2];
                            var t = toDate.Date.ToString("MM/dd/yyyy");
                            Qry += "and PDate Between '" + f + "' And '" + t + "'";
                        }

                        dt = General.GetDatatableQuery_Teledata("select top 1000 srno [SrNo],  pdate, Line, FromExt , ToExt,duration [Duration] from tblIntercomLog where 1=1   " + Qry + " order by pdate desc");

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

                        //Filter Line
                        if (!string.IsNullOrEmpty(filter1) && filter1 != "undefined" && filter1 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("Line").ToString().ToLower().Contains(filter1.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter FromExt
                        if (!string.IsNullOrEmpty(filter2) && filter2 != "undefined" && filter2 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("FromExt").ToString().ToLower().Contains(filter2.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter ToExt
                        if (!string.IsNullOrEmpty(filter3) && filter3 != "undefined" && filter3 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<string>("ToExt").ToString().ToLower().Contains(filter3.ToLower())
                                       select myRow;
                            dt = Rows.AsDataView().ToTable();
                        }
                        //Filter Duration
                        if (!string.IsNullOrEmpty(filter4) && filter4 != "undefined" && filter4 != "")
                        {
                            var Rows = from myRow in dt.AsEnumerable()
                                       where myRow.Field<decimal>("Duration").ToString().ToLower().Contains(filter4.ToLower())
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

    public partial class TeleOuterReportViewModel
    {
        public string ExtType { get; set; }
        public string ExtNo { get; set; }
        public string OutNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public partial class TeleInterReportViewModel
    {
        public string FromExt { get; set; }
        public string ToExt { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }




}
