using ERP.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Text;

namespace ERP.Utilities
{
    public class ERPUtils
    {
        #region CALCULATE HOURS BASED ON 60 MINUTES
        public List<decimal> CalculateHoursForModule(List<PMSTodo> list)
        {
            List<decimal> response = new List<decimal>();
            try
            {
                int _actualHours = 0, _actualMinutes = 0, _assignedHours = 0, _assignedMinutes = 0;
                foreach (var item in list)
                {
                    if (item.ActualHours != null)
                    {
                        _actualHours += Convert.ToInt32(item.ActualHours.ToString().Split('.')[0]);
                        _actualMinutes += Convert.ToInt32(item.ActualHours.ToString().Split('.')[1]);
                    }
                    if (item.AssignedHours != null)
                    {
                        _assignedHours += Convert.ToInt32(item.AssignedHours.ToString().Split('.')[0]);
                        _assignedMinutes += Convert.ToInt32(item.AssignedHours.ToString().Split('.')[1]);
                    }
                }

                _actualHours += _actualMinutes / 60;
                _actualMinutes = _actualMinutes % 60;

                _assignedHours += _assignedMinutes / 60;
                _assignedMinutes = _assignedMinutes % 60;

                decimal finalActualHours = Convert.ToDecimal(_actualHours + "." + _actualMinutes);
                response.Add(finalActualHours);

                decimal finalAssignedHours = Convert.ToDecimal(_assignedHours + "." + _assignedMinutes);
                response.Add(finalAssignedHours);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        //public List<decimal> CalculateHoursForProject(int projectId)
        //{
        //    List<decimal> response = new List<decimal>();
        //    try
        //    {
        //        var listModule = db.tblPMSModules.Where(z => z.ProjectId == projectId && z.IsArchived == false).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return response;
        //}
        #endregion

        #region "Log Page Activities"
        public bool SaveLogActivity(tblLOGPageActivity logActivity)
        {
            try
            {
                using (ERPContext db = new ERPContext())
                {
                    db.tblLOGPageActivities.Add(logActivity);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        public static string GetMacAddress()
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || nic.NetworkInterfaceType == NetworkInterfaceType.GenericModem)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        macAddresses += nic.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }
            return macAddresses;
        }
        public static string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        public static string GetComputerName()
        {
            return Environment.MachineName.ToString();
        }

        public static string GetDiffHourAndMinute(DateTime inTime, DateTime outTime)
        {
            string str;
            TimeSpan duration = outTime.Subtract(inTime);
            str = duration.Hours.ToString() + "." + duration.Minutes.ToString() + "." + duration.Seconds.ToString();
            return str;
        }
    }
    public static class ERPUtilities
    {
        private static ERPContext db = new ERPContext();

        #region "ConvertToDataTable Extension Method"

        public static DataTable ConvertToDataTable<T>(this IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }

        //public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        //{
        //    IList<T> list = null;

        //    if (rows != null)
        //    {
        //        list = new List<T>();

        //        foreach (DataRow row in rows)
        //        {
        //            T item = CreateItem<T>(row);
        //            list.Add(item);
        //        }
        //    }

        //    return list;
        //}

        //public static IList<T> ConvertTo<T>(DataTable table)
        //{
        //    if (table == null)
        //    {
        //        return null;
        //    }

        //    List<DataRow> rows = new List<DataRow>();

        //    foreach (DataRow row in table.Rows)
        //    {
        //        rows.Add(row);
        //    }

        //    return ConvertTo<T>(rows);
        //}

        #endregion

        #region ApiResponse For UnAuthorized User Access
        public static ApiResponse UnAuthorizedAccess(ApiResponse apiResponse)
        {
            apiResponse.IsValidUser = false;
            apiResponse.MessageType = 0;
            apiResponse.Message = "UnAuthorized User";
            apiResponse.DataList = null;

            return apiResponse;
        }

        public static ApiResponse GenerateApiResponse()
        {
            ApiResponse apiResponse = new ApiResponse();
            apiResponse.IsValidUser = false;
            apiResponse.MessageType = 0;
            apiResponse.Message = ConfigurationManager.AppSettings["msgAuthorization"].ToString();
            apiResponse.DataList = null;
            return apiResponse;
        }
        public static ApiResponse GenerateApiResponse(bool IsValidUser, int MessageType, string Message, object DataList)
        {
            ApiResponse apiResponse = new ApiResponse();
            apiResponse.IsValidUser = IsValidUser;
            apiResponse.MessageType = MessageType;
            apiResponse.Message = Message.ToString();
            apiResponse.DataList = DataList;
            return apiResponse;
        }
        #endregion

        #region "Export Excel Methods"
        /// <summary>
        /// For Exporting Data into Excel
        /// </summary>
        /// <param name="context">Httpcontext for parameter</param>
        /// <param name="timezone">timezone</param>
        /// <param name="dt">Datatable</param>
        /// <param name="HederTitle">Title in First Row of Excel</param>
        /// <param name="SheetName">Excel worksheet name</param>
        /// <param name="fileName">Excel File Name</param>
        public static void ExportExcel(HttpContext context, int timezone, DataTable dt, string HederTitle, string SheetName, string fileName)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                ERPUtilities.SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = ERPUtilities.CreateSheet(p, SheetName);

                ws.Cells[1, 1].Value = HederTitle;
                ws.Cells[1, 1, 1, dt.Columns.Count].Merge = true;
                ws.Cells[1, 1, 1, dt.Columns.Count].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int rowIndex = 2;

                ERPUtilities.CreateHeader(ws, ref rowIndex, dt);
                ERPUtilities.CreateData(ws, ref rowIndex, dt, timezone);

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                context.Response.BinaryWrite(p.GetAsByteArray());
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("content-disposition", "attachment;  filename=" + fileName + ".xlsx");
            }
        }
        /// Sets the workbook properties and adds a default sheet.
        public static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "DRC Infotech";
            p.Workbook.Properties.Title = "DRC ERP";
        }

        public static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            return ws;
        }

        //Create Columns Header
        public static void CreateHeader(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            int colIndex = 1;
            foreach (DataColumn dc in dt.Columns) //Creating Headings
            {
                var cell = ws.Cells[rowIndex, colIndex];
                //Setting the background color of header cells to Gray
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.Gray);

                //Setting Top/left,right/bottom borders.
                var border = cell.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                //Setting Value in cell
                cell.Value = dc.ColumnName;

                colIndex++;
            }
        }

        //Excel Data
        public static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            int colIndex = 0;
            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                foreach (DataColumn dc in dt.Columns)
                {
                    var cell = ws.Cells[rowIndex, colIndex];
                    //Setting Value in cell
                    if (dc.DataType.ToString() == "System.DateTime")
                    {
                        if (dr[dc.ColumnName] != DBNull.Value)
                            cell.Value = (Convert.ToDateTime(dr[dc.ColumnName])).ToString("dd-MMM-yyyy HH:mm");
                        else
                            cell.Value = "";
                    }
                    else
                    {
                        cell.Value = dr[dc.ColumnName];
                    }
                    //Setting borders of cell
                    var border = cell.Style.Border;
                    border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    colIndex++;
                }
            }
        }

        public static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt, int timezone)
        {
            int colIndex = 0;
            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                foreach (DataColumn dc in dt.Columns)
                {
                    var cell = ws.Cells[rowIndex, colIndex];
                    //Setting Value in cell
                    if (dc.DataType.ToString() == "System.DateTime")
                    {
                        if (dr[dc.ColumnName] != DBNull.Value)
                            cell.Value = (Convert.ToDateTime(dr[dc.ColumnName]).AddMinutes(-1 * timezone)).ToString("dd-MMM-yyyy HH:mm");
                        else
                            cell.Value = "";
                    }
                    else
                    {
                        cell.Value = dr[dc.ColumnName];
                    }
                    //Setting borders of cell
                    var border = cell.Style.Border;
                    border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    colIndex++;
                }
            }
        }
        #endregion

        #region "Database Exceptions"
        public static ApiResponse GenerateExceptionResponse(Exception ex, string PageName, bool IsValidUser)
        {
            var apiResponse = new ApiResponse();
            var strExType = ex.GetType().Name;
            GeneralMessages generalMessages = new GeneralMessages(PageName);

            if (strExType == "DbUpdateException")
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    var number = sqlException.Number;
                    if (number == 547)  //For Cascading Delete Exception
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
                    else if (number == 2627) //Exception For Duplicate Entry
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgEntryExists, null);
                    else
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
            }
            return apiResponse;
        }
        #endregion

        public static bool ToggleUserLoginStatus(int employeeId, bool status)
        {
            try
            {
                var user = db.tblEmpLoginInformations.SingleOrDefault(u => u.EmployeeId == employeeId);
                if (user != null && user.EmployeeId > 0)
                {
                    user.IsLogin = status;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// send mail to single address
        /// </summary>
        public static ApiResponse SendMessage(string sendTo, string sendFrom, string sendSubject, string sendbody, string pagename)
        {
            ApiResponse apiResponse = new ApiResponse();
            GeneralMessages generalMessages = new GeneralMessages(pagename);
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(sendFrom, sendTo, sendSubject, sendbody);
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtp"].ToString());
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                client.Send(message);
                message.Dispose();
                client.Dispose();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
            }
            catch (Exception ex)
            {
                apiResponse = GenerateExceptionResponse(ex, pagename, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// send mail to many with drill up reporting
        /// </summary>
        public static ApiResponse SendMailToMany(int employeeId, string sendFrom, string sendTo, string sendSubject, string sendbody, string pagename)
        {
            ApiResponse apiResponse = new ApiResponse();
            GeneralMessages generalMessages = new GeneralMessages(pagename);
            try
            {
                List<tblEmpCompanyInformation> cList = new List<tblEmpCompanyInformation>();
                //take reportingTo id and pass it in place of employeeid; Don't take entry of same employee as we are sending mail in To address also
                var line = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                if (line != null)
                {
                    EmployeeUtils employeeUtils = new EmployeeUtils();
                    //cList = DrillUpReporting(line.ReportingTo, cList);    
                    cList = employeeUtils.DrillUpReporting(line.ReportingTo, cList);
                }

                MailAddress mFrom = new MailAddress(sendFrom);
                MailAddress mTo = new MailAddress(sendTo);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(mFrom, mTo);
                message.Subject = sendSubject;
                message.ReplyTo = mTo;
                message.Body = sendbody;

                foreach (var l in cList)
                {
                    var email = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.CompanyEmailId).SingleOrDefault();
                    if (!string.IsNullOrEmpty(email))
                    {
                        MailAddress ccEmail = new MailAddress(email);
                        message.CC.Add(ccEmail);
                    }
                }

                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtp"].ToString());
                client.EnableSsl = false;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                client.Send(message);
                message.Dispose();
                client.Dispose();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
            }
            catch (Exception ex)
            {
                apiResponse = GenerateExceptionResponse(ex, pagename, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// enum for access rights permission set
        /// </summary>
        public enum AccessPermission
        {
            Insert = 1,
            Update = 2,
            Delete = 3,
            View = 13
        }
        public enum AccessPermissionEmployeeTab
        {
            Personal = 4,
            Qualification = 5,
            WorkExperience = 6,
            Document = 7,
            CompanyInfo = 8,
            PayRoll = 9,
            Relatives = 10,
            CompanyCredentials = 11,
            Login = 12
        }
        public enum AccessPermissionInvoiceTab
        {
            Open = 14,
            Inquiry = 15,
            Project = 16,
            Milestone = 17,
            Invoice = 18,
            Payment = 19
        }

        public enum DashboardWidget
        {
            Calendar = 1,
            Reminder = 2,
            InOut = 3,
            PendingLeave = 14
        }
        public enum ModuleType
        {
            Bug = 1,
            CR = 2,
            New = 3
        }
        public enum PMSStatus
        {
            Active = 1,
            Hold = 2,
            Finished = 3,
            Deleted = 4 //; we are using this status for archived entry; we used it for project filter
        }

        public enum BusinessType
        {
            Test1 = 1,
            Test2 = 2,
            Test3 = 3
        }


        public enum DocumentType
        {
            Identity = 1,
            CompanyRegistration = 2,
            Address = 3,
            Degree = 4,
            Income = 5,
            Experience = 6,
            Agreement = 7,
            Conversation = 15 //Type > 7, we are not using it in document master, It is for Invoice section.
        }

        public enum InquiryStatus
        {
            Received = 1,
            UnderAnalysis = 2,
            ProposalSubmitted = 3,
            Rejected = 4,
            Confirmed = 5,
            ProposalUpdated = 6,
            UnderClientReview = 7
        }

        public enum ProjectStatus
        {
            Running = 1,
            Completed = 2,
            Stopped = 3,
        }

        public enum ProjectTypes
        {
            Hourly = 1,
            Dedicated = 2,
            Fixed = 3,
        }

        /// <summary>
        /// Return menu list based on logged in user
        /// </summary>
        public static List<ARMenu> GetMenuList()
        {
            SessionUtils sessionUtils = new SessionUtils();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    ERPContext db = new ERPContext();
                    List<ARMenu> list = new List<ARMenu>();
                    //IF USER IS ADMIN THEN TAKE ALL MENUS
                    int employeeid = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    if (employeeid == 1)
                    { //ADMIN
                        var mList = db.tblARModules.Where(z => z.IsActive == true).ToList();
                        foreach (var m in mList)
                        {
                            if (db.tblARSubModules.Where(z => z.ModuleId == m.ModuleId && z.IsActive == true).Count() > 0)
                            {
                                list.Add(new ARMenu
                                {
                                    ModuleId = m.ModuleId,
                                    ModuleName = m.ModuleName,
                                    ModuleSeqNo = m.SeqNo,
                                    SubModules = db.tblARSubModules.Where(z => z.ModuleId == m.ModuleId && z.IsActive == true).OrderBy(z => z.SeqNo).ToList()
                                });
                            }
                        }

                    }
                    else
                    {//NORMAL USER
                        //GET: role id using logged in user
                        var cLine = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeid).SingleOrDefault();
                        if (cLine != null)
                        {
                            var pList = db.tblARPermissionAssigneds.Where(z => z.RoleId == cLine.RolesId).Select(z => z.ModuleId).Distinct().ToList();
                            if (pList != null)
                            {
                                foreach (var p in pList)
                                {
                                    var aList = db.tblARPermissionAssigneds.Where(z => z.ModuleId == p && z.IsActive == true && z.RoleId == cLine.RolesId).ToList();
                                    List<tblARSubModule> lstSM = new List<tblARSubModule>();
                                    foreach (var a in aList)
                                    {
                                        if (db.tblARSubModules.Where(z => z.ModuleId == a.ModuleId && z.SubModuleId == a.SubModuleId && z.IsActive == true).Count() > 0)
                                        {
                                            lstSM.Add(db.tblARSubModules.Where(z => z.ModuleId == a.ModuleId && z.SubModuleId == a.SubModuleId).SingleOrDefault());
                                        }
                                    }

                                    //CHECK: if sub module entry is exists && active
                                    if (db.tblARSubModules.Where(z => z.ModuleId == p && z.IsActive == true).Count() > 0)
                                    {
                                        var mLine = db.tblARModules.Where(z => z.ModuleId == p).Select(z => new { z.ModuleId, z.ModuleName, z.SeqNo }).FirstOrDefault();

                                        list.Add(new ARMenu
                                        {
                                            ModuleId = mLine.ModuleId,
                                            ModuleName = mLine.ModuleName,
                                            ModuleSeqNo = mLine.SeqNo,
                                            SubModules = lstSM.OrderBy(z => z.SeqNo).ToList()
                                        });
                                    }
                                }
                            }
                        }
                    }

                    //RESPONSE: return all active modules/submodules for assigned role
                    return list.OrderBy(z => z.ModuleSeqNo).ToList();
                }
                catch
                {
                    return null;
                }
            }
            HttpContext.Current.Response.Redirect("/Login");
            return null;
        }

        /// <summary>
        /// check insert, update, delete access permission
        /// </summary>
        public static bool HasAccessPermission(int action, string ctrl)
        {
            SessionUtils sessionUtils = new SessionUtils();
            ERPContext db = new ERPContext();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int employeeid = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    if (employeeid == 1)
                    {
                        return true;
                    }
                    else
                    {
                        var cLine = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeid).SingleOrDefault();
                        if (cLine != null)
                        {
                            var line = db.tblARSubModules.AsEnumerable().Where(z => z.URL.Replace("/", "").Trim().ToLower() == ctrl.Trim().ToLower()).FirstOrDefault();
                            if (line != null)
                            {
                                var pLine = db.tblARPermissionAssigneds.Where(z => z.ModuleId == line.ModuleId && z.SubModuleId == line.SubModuleId && z.RoleId == cLine.RolesId).SingleOrDefault();
                                if (pLine != null)
                                {
                                    if (!string.IsNullOrEmpty(pLine.Permission))
                                    {
                                        foreach (var p in pLine.Permission.Split(','))
                                        {
                                            if (action == Convert.ToInt32(p))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static Boolean isTeamLeader(int id)
        {
            Boolean isTl = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == id).Select(z => z.IsTL).FirstOrDefault();
            return isTl;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string GetClientCode(int clientId, string companyName, string countryCode)
        {
            try
            {
                string year = DateTime.Now.Year.ToString().Substring(2, 2);
                string month = DateTime.Now.Month.ToString();
                string cname = ERPUtilities.RemoveSpecialCharacters(companyName);
                cname = cname.Length > 3 ? cname.Substring(0, 3).ToUpper() : cname.ToUpper();

                string yearInChar = string.Empty;
                string monthInChar = string.Empty;

                XElement root = XElement.Load(HttpContext.Current.Server.MapPath("~/Content/xml/Alphabates.xml"));
                var dataYear = from el in root.Elements("char")
                               where (string)el.Attribute("number") == year
                               select el.Value;

                var dataMonth = from el in root.Elements("char")
                                where (string)el.Attribute("number") == month
                                select el.Value;

                yearInChar = dataYear.First();
                monthInChar = dataMonth.First();

                return string.Format("{0}{1}{2}{3}{4}", cname, yearInChar, monthInChar, countryCode.ToUpper(), clientId);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GenerateKey()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 2);
            }

            return string.Format("{0}{1}", GetRandomKey(4), string.Format("{0:x}", i - DateTime.Now.Ticks));
        }

        private static string GetRandomKey(int length)
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            string key = string.Empty;
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int x = random.Next(1, chars.Length);
                //Don't Allow Repetation of Characters
                if (!key.Contains(chars.GetValue(x).ToString()))
                    key += chars.GetValue(x);
                else
                    i--;
            }
            return key;
        }
    }
}