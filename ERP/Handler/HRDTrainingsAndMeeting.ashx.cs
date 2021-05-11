using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for HRDTrainingsAndMeeting
    /// </summary>
    public class HRDTrainingsAndMeeting : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var traingAndMeeting_list = (from traingAndMeeting in db.tblHRDTrainingsAndMeetings
                                         select new { traingAndMeeting.Department, traingAndMeeting.Manager, traingAndMeeting.Subject, traingAndMeeting.NoOfParticipant, traingAndMeeting.Intercom, traingAndMeeting.Date }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    traingAndMeeting_list = traingAndMeeting_list.Where(z => z.Date.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    traingAndMeeting_list = traingAndMeeting_list.Where(z => z.Date.Date >= fromDate.Date && z.Date.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(traingAndMeeting_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Training And Meeting List", "Training And Meeting List", "Training And Meeting List");
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