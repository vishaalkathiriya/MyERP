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
    /// Summary description for HRDChemicalStorageInspactionLog
    /// </summary>
    public class HRDChemicalStorageInspactionLog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var ChemicalInspaction = (from chemi_insp in db.tblHRDChemicalStorageInspectionLogs
                             select new
                             {
                                 chemi_insp.DateOfInspection,
                                 chemi_insp.CheckedyBy,
                                 chemi_insp.Findings,
                                 chemi_insp.RootCause,
                                 chemi_insp.CorrectiveAction,
                                 Remark=chemi_insp.Remark==null?"":chemi_insp.Remark
                             }).ToList();


            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    ChemicalInspaction = ChemicalInspaction.Where(z => z.DateOfInspection.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    ChemicalInspaction = ChemicalInspaction.Where(z => z.DateOfInspection.Date >= fromDate.Date && z.DateOfInspection.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(ChemicalInspaction);
            ERPUtilities.ExportExcel(context, timezone, dt, "Chemical Storage Inspaction Log", "Chemical Storage Inspaction Log", "Chemical Storage Inspaction Log");
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