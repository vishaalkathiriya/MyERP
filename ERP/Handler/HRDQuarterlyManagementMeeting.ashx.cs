using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Configuration;



namespace ERP.Handler
{
  
    public class HRDQuarterlyManagementMeeting : IHttpHandler
    {
        

        public void ProcessRequest(HttpContext context)
        {

            String temp = ConfigurationManager.AppSettings["TempUploadPath"].ToString();


            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var meeting_list = (from meeting in db.tblHRDQuarterlyManagementMeetings
                                     select new { meeting.Title ,meeting.DateOfMeeting,meeting.AgendaOfTraining,meeting.DecisionTakenDuringMeeting }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    meeting_list = meeting_list.Where(z => z.DateOfMeeting.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    meeting_list = meeting_list.Where(z => z.DateOfMeeting.Date >= fromDate.Date && z.DateOfMeeting.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(meeting_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Quarterly Meeting Management", "Quarterly Meeting Management", "Quarterly Meeting Management");
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