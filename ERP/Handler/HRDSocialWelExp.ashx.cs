using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    public class SocialWelExp : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var socialWelExp_list = (from socialWelExp in db.tblHRDSocialWelfareExpenses
                                     select new { socialWelExp.ProgrammeName, socialWelExp.Venue, socialWelExp.Date, socialWelExp.Time, socialWelExp.ExpenseAmount, socialWelExp.GuestName}).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    socialWelExp_list = socialWelExp_list.Where(z => z.Date.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    socialWelExp_list = socialWelExp_list.Where(z => z.Date.Date >= fromDate.Date && z.Date.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(socialWelExp_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Social Welfare Expense List", "Social Welfare Expense List", "Social Welfare Expense List");
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