using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;
using System.Collections.Generic;
using ERP.Areas.Reception.Classes;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for TelephoneReport
    /// </summary>
    public class TelephoneReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());

            #region "Set StartDate-EndDate"
            DateTime fromDate, toDate;

            string startDate = context.Request.QueryString["StartDate"];
            string endDate = context.Request.QueryString["EndDate"];

            if (!string.IsNullOrEmpty(startDate) && startDate != "undefined")
            {
                string[] fdate = startDate.Split('/');
                fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
            }
            else
            {
                fromDate = Convert.ToDateTime("1/1/1900");
            }

            if (!string.IsNullOrEmpty(endDate) && startDate != "undefined")
            {
                string[] tdate = endDate.Split('/');
                toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
            }
            else
            {
                toDate = Convert.ToDateTime("1/1/1900");
            }
            #endregion

            string ExtType = context.Request.QueryString["ExtType"];
            string ExtNo = context.Request.QueryString["ExtNo"];
            string OutNo = context.Request.QueryString["OutNo"];
            string StartDate = context.Request.QueryString["StartDate"];
            string EndDate = context.Request.QueryString["EndDate"];

            DataTable dt = new DataTable();
            try
            {
                string Qry = "";

                //Filter Ext-Type
                if (!string.IsNullOrEmpty(ExtType) && ExtType != "undefined" && ExtType != "0")
                {
                    Qry += " and Status = '" + ExtType + "'";
                }

                //Filter Ext-No
                if (!string.IsNullOrEmpty(ExtNo) && ExtNo != "undefined" && ExtNo != "0")
                {
                    Qry += " and FromExt = '" + ExtNo + "'";
                }

                //Filter Ext-No
                if (!string.IsNullOrEmpty(OutNo) && OutNo != "undefined" && OutNo != "0")
                {
                    Qry += " and OutLineNo = '" + OutNo + "'";
                }

                //Filter PDate
                if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                {
                    var from = StartDate.Split('/');
                    var f = from[1] + "/" + from[0] + "/" + from[2];
                    var to = EndDate.Split('/');
                    var t = to[1] + "/" + (Convert.ToInt16(to[0]) + 1) + "/" + to[2];
                    Qry += "and pdate Between '" + f + "' And '" + t + "'";
                }

                dt = General.GetDatatableQuery_Teledata("select srno [SrNo], Status [Type], pdate, Line [Outline], duration [Duration], FromExt as [ExtNo], OutLineNo [OutNo] from tblCall_Log where 1=1   " + Qry + " order by pdate desc");

                ERPUtilities.ExportExcel(context, timezone, dt, "TelephoneReport", "TelephoneReport", "TelephoneReport");
                context = null;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}