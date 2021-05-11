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
    /// Summary description for HRDSeftyTrainingRecords
    /// </summary>
    public class HRDSeftyTrainingRecords : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var SeftyTraining = (from sefty in db.tblHRDSafetyTrainingRecords
                             select new
                             {
                                 sefty.SubjectOfTraining,
                                 //  DateOfCleaining=boilplant.DateOfCleaining.(),
                                 //   boilplant = boilplant.DateOfCleaining.ToShortDateString(),.
                                 sefty.DateOfTraining,
                                 sefty.Department,
                                 sefty.ManagerName,
                                 sefty.NoOfParticipants,
                                 sefty.TrainersName,
                             }).ToList();


            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    SeftyTraining = SeftyTraining.Where(z => z.DateOfTraining.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    SeftyTraining = SeftyTraining.Where(z => z.DateOfTraining.Date >= fromDate.Date && z.DateOfTraining.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(SeftyTraining);
            ERPUtilities.ExportExcel(context, timezone, dt, "Sefty Training Records", "Sefty Training Records", "Sefty Training Records");
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