using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for HRDPPEIssueRegister
    /// </summary>
    public class HRDPPEIssueRegister : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();


            List<tblHRDPPEIssueRegister> filteredData = db.tblHRDPPEIssueRegisters.ToList();
            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    filteredData = filteredData.AsEnumerable().Where(z => z.CreDate.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    filteredData = filteredData.AsEnumerable().Where(z => z.CreDate.Date >= fromDate.Date && z.CreDate.Date <= toDate.Date).ToList();
                }
            }

            var PPEIssueRegister = (from ppeissue in filteredData
                                    select new
                                    {
                                        ppeissue.NameOfIssuer,
                                        ppeissue.NameOfRecievr,
                                        ppeissue.TypeOfPPE,
                                        ppeissue.Quanity,
                                        ppeissue.Department,
                                        ppeissue.ManagerName,
                                        ppeissue.Price,
                                        ppeissue.Remarks
                                    }).ToList();
            

            DataTable dt = ERPUtilities.ConvertToDataTable(PPEIssueRegister);
            ERPUtilities.ExportExcel(context, timezone, dt, "PPE Issue Register", "PPE Issue Register", "PPE Issue Register");
            context = null;


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