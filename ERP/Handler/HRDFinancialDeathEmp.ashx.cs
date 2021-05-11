using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{

    public class HRDFinancialDeathEmp : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var financialDepthEmp_list = (from  financialDeathEmp in db.tblHRDFinancialAssisToDeathEmployees
                                select new { financialDeathEmp.Ecode,financialDeathEmp.EmployeeName, financialDeathEmp.DateOfDeath,financialDeathEmp.Amount,financialDeathEmp.ChequeNumber,financialDeathEmp.ReceiveBy,financialDeathEmp.Relation,financialDeathEmp.FamilyBackgroundDetail }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    financialDepthEmp_list = financialDepthEmp_list.Where(z => z.DateOfDeath.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    financialDepthEmp_list = financialDepthEmp_list.Where(z => z.DateOfDeath.Date >= fromDate.Date && z.DateOfDeath.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(financialDepthEmp_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Financial Death Employee List", "Financial Death Employee List", "Financial Death Employee List");
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