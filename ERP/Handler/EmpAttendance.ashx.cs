using ERP.Models;
using ERP.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for EmpAttendance
    /// </summary>
    public class EmpAttendance : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            int filterYear = Convert.ToInt32(context.Request.QueryString["year"].ToString());
            string groupName = context.Request.QueryString["groupName"].ToString() == "null" ? null : context.Request.QueryString["groupName"].ToString();

            List<EmpAttendanceReportViewModel> spList = new List<EmpAttendanceReportViewModel>();
            SqlParameter yearParam = new SqlParameter();
            yearParam = new SqlParameter("@Year", filterYear);
            spList = db.Database.SqlQuery<EmpAttendanceReportViewModel>("usp_getEmployeeAttendanceReport  @Year ", yearParam).ToList();

            List<tblEmpPayRollInformation> lstPayroll = new List<tblEmpPayRollInformation>();
            if (String.IsNullOrEmpty(groupName))
            {
                lstPayroll = db.tblEmpPayRollInformations.Where(z => z.JoiningDate.Year <= filterYear && (z.ReLeavingDate == null || z.ReLeavingDate.Value.Year >= filterYear)).ToList();
            }
            else
            {
                lstPayroll = db.tblEmpPayRollInformations.Where(z => z.GName == groupName && z.JoiningDate.Year <= filterYear && (z.ReLeavingDate == null || z.ReLeavingDate.Value.Year >= filterYear)).ToList();
            }

            List<EmpAttendanceReportDetailViewModel> lstReportData = new List<EmpAttendanceReportDetailViewModel>();

            foreach (var item in spList)
            {
                tblEmpPayRollInformation objPayroll = (from o in lstPayroll where o.EmployeeId == item.EmployeeId select o).FirstOrDefault();

                if (objPayroll != null)
                {
                    EmpAttendanceReportDetailViewModel obj = new EmpAttendanceReportDetailViewModel();

                    #region "Personal Information"
                    obj.EmployeeId = item.EmployeeId;
                    obj.EmpName = (from l in db.tblEmpPersonalInformations.AsEnumerable() where l.EmployeeId == item.EmployeeId select new { EmpName = l.CandidateFirstName + (l.CandidateMiddleName.Length > 1 ? l.CandidateMiddleName.Substring(0, 1) : l.CandidateMiddleName) + (l.CandidateLastName.Length > 1 ? l.CandidateLastName.Substring(0, 1) : l.CandidateLastName) }).FirstOrDefault().EmpName;
                    obj.Total_P = obj.Total_A = obj.Total_L = obj.Total_O = obj.Total_H = 0;
                    #endregion

                    #region "Set Join-End Date"
                    obj.JOIN_MONTH = objPayroll.JoiningDate.Month;
                    obj.JOIN_YEAR = objPayroll.JoiningDate.Year;
                    if (objPayroll.ReLeavingDate != null)
                    {
                        obj.REL_MONTH = objPayroll.ReLeavingDate.Value.Month;
                        obj.REL_YEAR = objPayroll.ReLeavingDate.Value.Year;
                    }
                    #endregion

                    #region "Attendance Information"

                    //January
                    if (!string.IsNullOrEmpty(item.JAN))
                    {
                        string[] JAN = item.JAN.Split('|');
                        obj.JAN_P = GetValue(JAN[0]);
                        obj.JAN_A = GetValue(JAN[1]);
                        obj.JAN_L = GetValue(JAN[2]);
                        obj.JAN_O = GetValue(JAN[3]);
                        obj.JAN_H = GetValue(JAN[4]);

                        addToTotal(ref obj, JAN);
                    }
                    //February
                    if (!string.IsNullOrEmpty(item.FEB))
                    {
                        string[] FEB = item.FEB.Split('|');
                        obj.FEB_P = GetValue(FEB[0]);
                        obj.FEB_A = GetValue(FEB[1]);
                        obj.FEB_L = GetValue(FEB[2]);
                        obj.FEB_O = GetValue(FEB[3]);
                        obj.FEB_H = GetValue(FEB[4]);

                        addToTotal(ref obj, FEB);
                    }
                    //March
                    if (!string.IsNullOrEmpty(item.MAR))
                    {
                        string[] MAR = item.MAR.Split('|');
                        obj.MAR_P = GetValue(MAR[0]);
                        obj.MAR_A = GetValue(MAR[1]);
                        obj.MAR_L = GetValue(MAR[2]);
                        obj.MAR_O = GetValue(MAR[3]);
                        obj.MAR_H = GetValue(MAR[4]);

                        addToTotal(ref obj, MAR);
                    }
                    //April
                    if (!string.IsNullOrEmpty(item.APR))
                    {
                        string[] APR = item.APR.Split('|');
                        obj.APR_P = GetValue(APR[0]);
                        obj.APR_A = GetValue(APR[1]);
                        obj.APR_L = GetValue(APR[2]);
                        obj.APR_O = GetValue(APR[3]);
                        obj.APR_H = GetValue(APR[4]);

                        addToTotal(ref obj, APR);
                    }
                    //May
                    if (!string.IsNullOrEmpty(item.MAY))
                    {
                        string[] MAY = item.MAY.Split('|');
                        obj.MAY_P = GetValue(MAY[0]);
                        obj.MAY_A = GetValue(MAY[1]);
                        obj.MAY_L = GetValue(MAY[2]);
                        obj.MAY_O = GetValue(MAY[3]);
                        obj.MAY_H = GetValue(MAY[4]);

                        addToTotal(ref obj, MAY);
                    }
                    //June
                    if (!string.IsNullOrEmpty(item.JUN))
                    {
                        string[] JUN = item.JUN.Split('|');
                        obj.JUN_P = GetValue(JUN[0]);
                        obj.JUN_A = GetValue(JUN[1]);
                        obj.JUN_L = GetValue(JUN[2]);
                        obj.JUN_O = GetValue(JUN[3]);
                        obj.JUN_H = GetValue(JUN[4]);

                        addToTotal(ref obj, JUN);
                    }
                    //July
                    if (!string.IsNullOrEmpty(item.JUL))
                    {
                        string[] JUL = item.JUL.Split('|');
                        obj.JUL_P = GetValue(JUL[0]);
                        obj.JUL_A = GetValue(JUL[1]);
                        obj.JUL_L = GetValue(JUL[2]);
                        obj.JUL_O = GetValue(JUL[3]);
                        obj.JUL_H = GetValue(JUL[4]);

                        addToTotal(ref obj, JUL);
                    }
                    //August
                    if (!string.IsNullOrEmpty(item.AUG))
                    {
                        string[] AUG = item.AUG.Split('|');
                        obj.AUG_P = GetValue(AUG[0]);
                        obj.AUG_A = GetValue(AUG[1]);
                        obj.AUG_L = GetValue(AUG[2]);
                        obj.AUG_O = GetValue(AUG[3]);
                        obj.AUG_H = GetValue(AUG[4]);

                        addToTotal(ref obj, AUG);
                    }
                    //September
                    if (!string.IsNullOrEmpty(item.SEP))
                    {
                        string[] SEP = item.SEP.Split('|');
                        obj.SEP_P = GetValue(SEP[0]);
                        obj.SEP_A = GetValue(SEP[1]);
                        obj.SEP_L = GetValue(SEP[2]);
                        obj.SEP_O = GetValue(SEP[3]);
                        obj.SEP_H = GetValue(SEP[4]);

                        addToTotal(ref obj, SEP);
                    }
                    //October
                    if (!string.IsNullOrEmpty(item.OCT))
                    {
                        string[] OCT = item.OCT.Split('|');
                        obj.OCT_P = GetValue(OCT[0]);
                        obj.OCT_A = GetValue(OCT[1]);
                        obj.OCT_L = GetValue(OCT[2]);
                        obj.OCT_O = GetValue(OCT[3]);
                        obj.OCT_H = GetValue(OCT[4]);

                        addToTotal(ref obj, OCT);
                    }
                    //November
                    if (!string.IsNullOrEmpty(item.NOV))
                    {
                        string[] NOV = item.NOV.Split('|');
                        obj.NOV_P = GetValue(NOV[0]);
                        obj.NOV_A = GetValue(NOV[1]);
                        obj.NOV_L = GetValue(NOV[2]);
                        obj.NOV_O = GetValue(NOV[3]);
                        obj.NOV_H = GetValue(NOV[4]);

                        addToTotal(ref obj, NOV);
                    }
                    //December
                    if (!string.IsNullOrEmpty(item.DEC))
                    {
                        string[] DEC = item.DEC.Split('|');
                        obj.DEC_P = GetValue(DEC[0]);
                        obj.DEC_A = GetValue(DEC[1]);
                        obj.DEC_L = GetValue(DEC[2]);
                        obj.DEC_O = GetValue(DEC[3]);
                        obj.DEC_H = GetValue(DEC[4]);

                        addToTotal(ref obj, DEC);
                    }

                    //Pending Leave Calculation
                    if (lstPayroll.Where(z => z.EmployeeId == item.EmployeeId && z.PermanentFromDate != null).Count() > 0)
                    {
                        obj.PendingLeave = new EmployeeUtils().GetEmployeePendingLeave(item.EmployeeId, Convert.ToInt32(filterYear));
                    }
                    //End Pending Leave Calculation

                    lstReportData.Add(obj);

                    #endregion
                }
            }

            //OrderBy EmpName
            lstReportData = lstReportData.OrderBy(z => z.EmpName).ToList();

            var data = from o in lstReportData
                       select new
                       {
                           o.Total_P,
                           o.Total_A,
                           o.Total_L,
                           o.Total_O,
                           o.Total_H,
                           PL = o.PendingLeave,
                           o.EmpName,

                           o.JAN_P,
                           o.JAN_A,
                           o.JAN_L,
                           o.JAN_O,
                           o.JAN_H,

                           o.FEB_P,
                           o.FEB_A,
                           o.FEB_L,
                           o.FEB_O,
                           o.FEB_H,

                           o.MAR_P,
                           o.MAR_A,
                           o.MAR_L,
                           o.MAR_O,
                           o.MAR_H,

                           o.APR_P,
                           o.APR_A,
                           o.APR_L,
                           o.APR_O,
                           o.APR_H,

                           o.MAY_P,
                           o.MAY_A,
                           o.MAY_L,
                           o.MAY_O,
                           o.MAY_H,

                           o.JUN_P,
                           o.JUN_A,
                           o.JUN_L,
                           o.JUN_O,
                           o.JUN_H,

                           o.JUL_P,
                           o.JUL_A,
                           o.JUL_L,
                           o.JUL_O,
                           o.JUL_H,

                           o.AUG_P,
                           o.AUG_A,
                           o.AUG_L,
                           o.AUG_O,
                           o.AUG_H,

                           o.SEP_P,
                           o.SEP_A,
                           o.SEP_L,
                           o.SEP_O,
                           o.SEP_H,

                           o.OCT_P,
                           o.OCT_A,
                           o.OCT_L,
                           o.OCT_O,
                           o.OCT_H,

                           o.NOV_P,
                           o.NOV_A,
                           o.NOV_L,
                           o.NOV_O,
                           o.NOV_H,

                           o.DEC_P,
                           o.DEC_A,
                           o.DEC_L,
                           o.DEC_O,
                           o.DEC_H,
                           JOINMONTH = o.JOIN_MONTH,
                           JOINYEAR = o.JOIN_YEAR,
                           RELEAVEMONTH = o.REL_MONTH,
                           RELEAVEYEAR = o.REL_YEAR
                       };

            DataTable dt = ERPUtilities.ConvertToDataTable(data.ToList());

            ExportExcel(context, timezone, dt, "Employee Attendance Report", "Employee Attendance Report", "EmpAttendanceReport", filterYear);
            context = null;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        //Convert Value To String And Format it
        public string GetValue(string value)
        {
            return Convert.ToDouble(value) == 0 ? "" : value.TrimEnd('0').TrimEnd('.');
        }

        //Add Monthly P,A,L,O,H to Total P,A,L,O,H
        public void addToTotal(ref EmpAttendanceReportDetailViewModel obj, string[] arr)
        {
            obj.Total_P += Convert.ToDouble(arr[0]);
            obj.Total_A += Convert.ToDouble(arr[1]);
            obj.Total_L += Convert.ToDouble(arr[2]);
            obj.Total_O += Convert.ToDouble(arr[3]);
            obj.Total_H += Convert.ToDouble(arr[4]);
        }

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
        /// <param name="selectedYear">Currently Filtered Year</param>
        public static void ExportExcel(HttpContext context, int timezone, DataTable dt, string HederTitle, string SheetName, string fileName, int selectedYear)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                ERPUtilities.SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = ERPUtilities.CreateSheet(p, SheetName);

                ws.Cells[1, 1].Value = HederTitle;
                ws.Cells[1, 1, 1, dt.Columns.Count - 4].Merge = true;
                ws.Cells[1, 1, 1, dt.Columns.Count - 4].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, dt.Columns.Count - 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                FormatHeaderCell(ws.Cells[2, 1, 2, 5], "TOTAL");
                FormatHeaderCell(ws.Cells[2, 6], "PL");
                FormatHeaderCell(ws.Cells[2, 7], "EMPNAME");
                FormatHeaderCell(ws.Cells[2, 8, 2, 12], "JAN");
                FormatHeaderCell(ws.Cells[2, 13, 2, 17], "FEB");
                FormatHeaderCell(ws.Cells[2, 18, 2, 22], "MAR");
                FormatHeaderCell(ws.Cells[2, 23, 2, 27], "APR");
                FormatHeaderCell(ws.Cells[2, 28, 2, 32], "MAY");
                FormatHeaderCell(ws.Cells[2, 33, 2, 37], "JUN");
                FormatHeaderCell(ws.Cells[2, 38, 2, 42], "JUL");
                FormatHeaderCell(ws.Cells[2, 43, 2, 47], "AUG");
                FormatHeaderCell(ws.Cells[2, 48, 2, 52], "SEP");
                FormatHeaderCell(ws.Cells[2, 53, 2, 57], "OCT");
                FormatHeaderCell(ws.Cells[2, 58, 2, 62], "NOV");
                FormatHeaderCell(ws.Cells[2, 63, 2, 67], "DEC");

                ws.View.FreezePanes(4, 8);

                int rowIndex = 3;
                CreateHeader(ws, ref rowIndex, dt);
                CreateData(ws, ref rowIndex, dt, timezone, selectedYear);

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                context.Response.BinaryWrite(p.GetAsByteArray());
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("content-disposition", "attachment;  filename=" + fileName + ".xlsx");
            }
        }

        //Create Columns Header
        public static void CreateHeader(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            int colIndex = 1;
            foreach (DataColumn dc in dt.Columns) //Creating Headings
            {
                //Condition Removes join month-year and releave month-year columns from display in sheet
                if (!dc.ColumnName.ToUpper().Contains("JOIN") && !dc.ColumnName.ToUpper().Contains("RELEAVE"))
                {
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Setting Top/left,right/bottom borders.
                    var border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    //Set header's horizontal alignment to center
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Setting Value in cell
                    if (!dc.ColumnName.ToUpper().Contains("PL") && !dc.ColumnName.ToUpper().Contains("EMPNAME"))
                    {
                        cell.Value = dc.ColumnName.Substring(dc.ColumnName.IndexOf('_') + 1, 1);
                    }

                    colIndex++;
                }
            }
        }

        public static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt, int timezone, int selectedYear)
        {
            int colIndex = 0;
            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                foreach (DataColumn dc in dt.Columns)
                {
                    //Condition Removes join month-year and releave month-year columns from display in sheet
                    if (!dc.ColumnName.ToUpper().Contains("JOIN") && !dc.ColumnName.ToUpper().Contains("RELEAVE"))
                    {
                        var cell = ws.Cells[rowIndex, colIndex];
                        cell.Value = dr[dc.ColumnName];

                        ////Convert Cell Value to Number if its Number Type Column P,A,L,O,Total and PL
                        if (!dc.ColumnName.ToUpper().Equals("EMPNAME"))
                        {
                            if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                            {
                                cell.Value = float.Parse(dr[dc.ColumnName].ToString());
                            }
                        }

                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        //Set Right border after H column of TOTAL and Right border of PL
                        if (dc.ColumnName.ToUpper().Contains("TOTAL_H") || dc.ColumnName.ToUpper().Contains("PL"))
                        {
                            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        //Set Border to bottom of all cells (+3 because of three rows of heading added to number of rows)
                        if (rowIndex == dt.Rows.Count + 3)
                        {
                            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }

                        //Formatting of Data Cell (P,L,O,A,H) for each Month
                        if (dc.ColumnName.Contains("_") && !dc.ColumnName.Contains("Total"))
                        {
                            var border = cell.Style.Border;
                            var font = cell.Style.Font;
                            var fill = cell.Style.Fill;

                            #region "Set Font Color For P A L O H columns"
                            switch (dc.ColumnName.Substring(dc.ColumnName.IndexOf('_') + 1, 1))
                            {
                                case "P":
                                    font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#8CC152"));
                                    break;

                                case "A":
                                    font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#DA4453"));
                                    break;

                                case "L":
                                    font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#E9573F"));
                                    break;

                                case "O":
                                    font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#37BC9B"));
                                    break;

                                case "H":
                                    font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#4A89DC"));
                                    border.Right.Style = ExcelBorderStyle.Thin;
                                    break;

                                default:
                                    break;
                            }
                            #endregion

                            #region "Change Background Color of Cell If Emp is not joined with company"

                            string monthName = dc.ColumnName.Substring(0, 3);
                            int currentMonth = DateTime.ParseExact(monthName, "MMM", CultureInfo.CurrentCulture).Month;

                            int JOIN_MONTH = Convert.ToInt32(dr[dt.Columns["JOINMONTH"]].ToString());
                            int JOIN_YEAR = Convert.ToInt32(dr[dt.Columns["JOINYEAR"]].ToString());
                            int REL_MONTH = string.IsNullOrEmpty(dr[dt.Columns["RELEAVEMONTH"]].ToString()) ? 0 : Convert.ToInt32(dr[dt.Columns["RELEAVEMONTH"]].ToString());
                            int REL_YEAR = string.IsNullOrEmpty(dr[dt.Columns["RELEAVEYEAR"]].ToString()) ? 0 : Convert.ToInt32(dr[dt.Columns["RELEAVEYEAR"]].ToString());

                            if (REL_MONTH != 0 || REL_YEAR != 0)
                            {
                                if ((JOIN_MONTH > currentMonth && JOIN_YEAR >= selectedYear) || (REL_MONTH < currentMonth && REL_YEAR <= selectedYear))
                                {
                                    FormatDataCell(fill, font);
                                }
                            }
                            else
                            {
                                if (JOIN_MONTH > currentMonth && JOIN_YEAR >= selectedYear)
                                {
                                    FormatDataCell(fill, font);
                                }
                            }
                            #endregion

                        }

                        colIndex++;
                    }
                }
            }
        }

        //Format Column's Header Cell and Set Title
        static void FormatHeaderCell(ExcelRange wsCell, string title)
        {
            wsCell.Value = title;
            wsCell.Merge = true;
            wsCell.Style.Font.Bold = true;
            wsCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Setting the background color of header cells to Gray(Except First 7 fixed Columns)
            var fill = wsCell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.Gray);

            // Set border
            var border = wsCell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
        }

        //Format Data Cell If Employee currently not joined with company
        static void FormatDataCell(ExcelFill fill, ExcelFont font)
        {
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#EBEBEB"));
            font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#EBEBEB"));
        }

        #endregion

    }
}