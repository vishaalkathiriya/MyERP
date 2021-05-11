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
    /// Summary description for EmployeeDetailReport
    /// </summary>
    public class EmployeeDetailReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());

            string FirstName = context.Request.QueryString["FirstName"];
            string MiddleName = context.Request.QueryString["MiddleName"];
            string LastName = context.Request.QueryString["LastName"];
            string Department = context.Request.QueryString["Department"];

            DataTable dt = new DataTable();
            try
            {
                string Qry = "";

                //Filter FirstName
                if (!string.IsNullOrEmpty(FirstName) && FirstName != "undefined")
                {
                    Qry += "  and FirstName like '%" + FirstName + "%'";
                }

                //Filter MiddleName
                if (!string.IsNullOrEmpty(MiddleName) && MiddleName != "undefined")
                {
                    Qry += " and MiddleName like '%" + MiddleName + "%'";
                }

                //Filter LastName
                if (!string.IsNullOrEmpty(LastName) && LastName != "undefined")
                {
                    Qry += " and LastName like '%" + LastName + "%'";
                }

                //Filter Dept
                if (!string.IsNullOrEmpty(Department) && Department != "undefined" && Department != "0")
                {
                    Qry += " and Department like '%" + Department + "%'"; //Reception
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

                var Rows = (from row in dt.AsEnumerable()
                            orderby row["ECode"] ascending
                            select row);
                dt = Rows.AsDataView().ToTable();

                ERPUtilities.ExportExcel(context, timezone, dt, "EmployeeDetailReport", "EmployeeDetailReport", "EmployeeDetailReport");
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