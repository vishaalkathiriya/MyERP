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
    /// Summary description for EmpAttendanceMonthFormat
    /// </summary>
    public class EmpAttendanceMonthFormatHandler : IHttpHandler
    {

        //Get all the dates of particular month
        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
            }
        }
        //Convert Value To String And Format it
        public string GetValue(string value)
        {
            return Convert.ToDouble(value) == 0 ? "" : value.TrimEnd('0').TrimEnd('.');
        }

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            int filterMonth = Convert.ToInt32(context.Request.QueryString["month"].ToString());
            int filterYear = Convert.ToInt32(context.Request.QueryString["year"].ToString());
            string groupName = context.Request.QueryString["groupName"].ToString() == "null" ? null : context.Request.QueryString["groupName"].ToString();

            //Get First and Last Date Of Month
            DateTime StartDt = new DateTime(filterYear, filterMonth, 1, 0, 0, 0);
            DateTime EndDt = new DateTime(filterYear, filterMonth, DateTime.DaysInMonth(filterYear, filterMonth), 0, 0, 0);

            //Get Payroll Information
            List<tblEmpPayRollInformation> lstEmpPayrollData = new List<tblEmpPayRollInformation>();
            if (String.IsNullOrEmpty(groupName))
            {
                lstEmpPayrollData = db.tblEmpPayRollInformations.Where(z => z.JoiningDate != null && z.JoiningDate <= EndDt.Date && (z.ReLeavingDate == null || z.ReLeavingDate >= StartDt)).ToList();
            }
            else
            {
                lstEmpPayrollData = db.tblEmpPayRollInformations.Where(z => z.JoiningDate != null && z.JoiningDate <= EndDt.Date && (z.ReLeavingDate == null || z.ReLeavingDate >= StartDt) && z.GName == groupName).ToList();
            }

            //Get Festivals List Of Month
            List<tblFestival> lstFestivals = db.tblFestivals.Where(z => z.tblFestivalType.IsWorkingDay == false && z.FestivalDate >= StartDt && z.FestivalDate <= EndDt).ToList();

            //Get Attendance Data Of Month
            List<tblEmpAttendance> lstAttendanceData = db.tblEmpAttendances.Where(z => z.PDate.Month == filterMonth && z.PDate.Year == filterYear).ToList();

            List<EmpAttendanceMonthFormat> lstEmps = new List<EmpAttendanceMonthFormat>();
            foreach (tblEmpPayRollInformation item in lstEmpPayrollData)
            {
                EmpAttendanceMonthFormat obj = new EmpAttendanceMonthFormat();

                #region "Employee Personal Information"
                obj.EmpName = (from o in db.tblEmpPersonalInformations.AsEnumerable() where o.EmployeeId == item.EmployeeId select new { EmpName = o.CandidateFirstName + (o.CandidateMiddleName.Length > 1 ? o.CandidateMiddleName.Substring(0, 1) : o.CandidateMiddleName) + (o.CandidateLastName.Length > 1 ? o.CandidateLastName.Substring(0, 1) : o.CandidateLastName) }).FirstOrDefault().EmpName;
                obj.Total_P = obj.Total_A = obj.Total_L = obj.Total_O = obj.Total_H = 0;
                #endregion

                #region "Employee Attendance Information"
                // Get Whole Month Attendance Data of Employee
                List<EmpAttendanceEmployeeMonthDetailViewModel> lstObjdetail = new List<EmpAttendanceEmployeeMonthDetailViewModel>();
                foreach (DateTime date in AllDatesInMonth(filterYear, filterMonth))
                {
                    EmpAttendanceEmployeeMonthDetailViewModel objD = new EmpAttendanceEmployeeMonthDetailViewModel();
                    objD.Edate = date;

                    //Check Whether Employee is joined or not on this date
                    tblEmpPayRollInformation objPayroll = lstEmpPayrollData.Where(z => z.EmployeeId == item.EmployeeId).SingleOrDefault();
                    if ((objPayroll.JoiningDate > date.Date) || (objPayroll.ReLeavingDate != null && objPayroll.ReLeavingDate < date))
                    {
                        objD.isJoined = false;
                    }
                    else
                    {
                        objD.isJoined = true;
                    }

                    //Check whether the day is Sunday or Festival Day
                    bool isHoliday = false;
                    var festival = lstFestivals.Where(z => z.FestivalDate.Date == date.Date).ToList();
                    if (festival.Count > 0)
                    {
                        isHoliday = true;
                        objD.P = "H";
                    }
                    objD.isHoliday = isHoliday;

                    // If Attendance Entry Exists
                    tblEmpAttendance objTblEmpAttendance = lstAttendanceData.Where(z => z.EmployeeId == item.EmployeeId && z.PDate.Date == date.Date).FirstOrDefault();
                    if (objTblEmpAttendance != null)
                    {
                        if (objTblEmpAttendance.Presence == 1)
                        {
                            objD.P = "P";
                        }
                        if (objTblEmpAttendance.Leave == 1)
                        {
                            objD.P = "L";
                        }
                        if ((objTblEmpAttendance.IsHoliday == false ? 0 : 1) == 1)
                        {
                            objD.P = "H";
                        }
                        if (objTblEmpAttendance.Absence == 1)
                        {
                            objD.P = "A";
                        }

                        if (objD.P == null)
                        {
                            objD.P = "0.5";
                        }

                        objD.A = objTblEmpAttendance.Absence == Convert.ToDecimal(0.5) ? "0.5" : "";
                        objD.L = objTblEmpAttendance.Leave == Convert.ToDecimal(0.5) ? "0.5" : "";
                        objD.O = GetValue(objTblEmpAttendance.OT.ToString());

                        if (!string.IsNullOrEmpty(objTblEmpAttendance.Remark) || !string.IsNullOrWhiteSpace(objTblEmpAttendance.Remark))
                        {
                            objD.Remark = "Remark : " + objTblEmpAttendance.Remark;
                        }

                        obj.Total_P += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Presence.ToString()) ? "0" : objTblEmpAttendance.Presence.ToString());
                        obj.Total_A += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Absence.ToString()) ? "0" : objTblEmpAttendance.Absence.ToString());
                        obj.Total_L += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Leave.ToString()) ? "0" : objTblEmpAttendance.Leave.ToString());
                        obj.Total_O += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.OT.ToString()) ? "0" : objTblEmpAttendance.OT.ToString());
                        obj.Total_H += Convert.ToDouble(objTblEmpAttendance.IsHoliday == false ? "0" : "1");

                        //Calculate total salaried days
                        if (filterYear >= 2017)
                        {
                            double avgLeavePerMonth = objPayroll.LeavesAllowedPerYear / 12;
                            obj.Total_S = obj.Total_P + avgLeavePerMonth + obj.Total_O + obj.Total_H;
                        }
                        else
                        {
                            obj.Total_S = obj.Total_P + obj.Total_O + obj.Total_H;
                        }
                    }

                    lstObjdetail.Add(objD);
                }
                obj.objDetail = lstObjdetail;
                #endregion

                lstEmps.Add(obj);

            }

            //Order by EmpName Ascending
            lstEmps = lstEmps.OrderBy(z => z.EmpName).ToList();

            var data = from o in lstEmps
                       select new
                       {
                           P = o.Total_P,
                           A = o.Total_A,
                           L = o.Total_L,
                           O = o.Total_O,
                           H = o.Total_H,
                           S = o.Total_S,
                           o.EmpName,
                           o.objDetail
                       };

            DataTable dt = ERPUtilities.ConvertToDataTable(data.ToList());

            ExportExcel(context, timezone, dt, "Employee Attendance Report", "Employee Attendance Report", "EmpAttendanceReportMonthFormat", filterYear, filterMonth);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
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
        /// <param name="Year">Currently Filtered Year</param>
        /// <param name="Month">Currently Filtered Month</param>
        public static void ExportExcel(HttpContext context, int timezone, DataTable dt, string HederTitle, string SheetName, string fileName, int Year, int Month)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                ERPUtilities.SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = ERPUtilities.CreateSheet(p, SheetName);

                //Count Total Days of Selected Month-Year
                int totalDays = DateTime.DaysInMonth(Year, Month);

                //Set Main Heading
                ws.Cells[1, 1, 1, 7].Merge = true;
                ws.Cells[1, 8, 1, dt.Columns.Count + totalDays - 1].Value = HederTitle;
                ws.Cells[1, 8, 1, dt.Columns.Count + totalDays - 1].Merge = true;
                ws.Cells[1, 8, 1, dt.Columns.Count + totalDays - 1].Style.Font.Bold = true;
                ws.Cells[1, 8, 1, dt.Columns.Count + totalDays - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Set 2nd Row Headers
                FormatHeaderCell(ws.Cells[2, 1, 2, 6], "TOTAL");
                FormatHeaderCell(ws.Cells[2, 7, 2, 7], "EMPNAME");
                FormatHeaderCell(ws.Cells[2, 8, 2, 7 + totalDays], "Month : " + new DateTime(Year, Month, 1).ToString("MMMM", CultureInfo.InvariantCulture) + "-" + Year);

                //Set 3rd Row Headers
                int colIndex = 8;
                for (int i = 1; i <= totalDays; i++)
                {
                    FormatHeaderCell(ws.Cells[3, colIndex, 3, colIndex], i.ToString());
                    colIndex += 1;
                }

                //Freeze Headers Rows and Left 7 Columns
                ws.View.FreezePanes(4, 8);

                int rowIndex = 3;
                CreateHeader(ws, ref rowIndex, dt);
                CreateData(ws, ref rowIndex, dt, timezone, Year, Month);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Set Width Of Columns
                ws.Column(1).Width = ws.Column(2).Width = ws.Column(3).Width = ws.Column(4).Width = ws.Column(5).Width = ws.Column(6).Width = 5.5;
                for (int i = 1; i <= totalDays; i++)
                {
                    ws.Column(7 + i).Width = 5.5;
                }

                context.Response.BinaryWrite(p.GetAsByteArray());
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("content-disposition", "attachment;  filename=" + fileName + ".xlsx");
            }
        }

        //Format Column's Header Cell and Set Title
        static void FormatHeaderCell(ExcelRange wsCell, string title)
        {
            //Set title value
            int t;
            bool isInt = int.TryParse(title, out t);
            if (isInt) { wsCell.Value = t; } else { wsCell.Value = title; }

            wsCell.Merge = true;
            wsCell.Style.Font.Bold = true;
            wsCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var fill = wsCell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.Gray);

            // Set border
            var border = wsCell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
        }

        //Create Columns Header
        public static void CreateHeader(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            int colIndex = 1;
            foreach (DataColumn dc in dt.Columns) //Creating Headings
            {
                if (colIndex < 8)
                {
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Setting Top/left,right/bottom borders.
                    var border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    //Set header's horizontal alignment to center
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Set background color
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.Gray);

                    //Setting Value in cell
                    if (!dc.ColumnName.ToUpper().Contains("EMPNAME"))
                    {
                        cell.Value = dc.ColumnName;
                    }

                    colIndex++;
                }
            }
        }

        //Create Columns Data
        public static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt, int timezone, int Year, int Month)
        {
            int TotalDays = DateTime.DaysInMonth(Year, Month);
            int colIndex = 0;
            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                //Set Data in First 6 Columns Total(P,A,L,O,H and EmpName)
                for (int i = 0; i < 7; i++)
                {
                    if (colIndex < 8)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];
                        cell.Value = dr[i];
                    }
                    colIndex++;
                }

                //Get Detail of Attendance of Employee
                List<EmpAttendanceEmployeeMonthDetailViewModel> lstObjDetail = (List<EmpAttendanceEmployeeMonthDetailViewModel>)dr["objDetail"];

                //Set Data for Each-Day(1,2,3,4,5,6,.....) in Month
                #region "Set Data and BackColor in Row-Present"
                foreach (var item in lstObjDetail)
                {
                    //Set Data
                    var cell = ws.Cells[rowIndex, colIndex];
                    cell.Value = item.P;
                    AddRemarkToCell(cell, item.Remark);

                    //Set BackColor
                    string color = "";
                    if (item.P == "A")
                    {
                        color = ColorClass.Absence;
                    }
                    else if (item.isHoliday) { color = ColorClass.Holiday; }
                    else
                    {
                        switch (item.P)
                        {
                            case "P":
                                color = ColorClass.FullPresent;
                                break;
                            case "H":
                                color = ColorClass.Holiday;
                                break;
                            case "L":
                                color = ColorClass.Leave;
                                break;
                            case "0.5":
                                cell.Value = float.Parse(item.P);
                                color = ColorClass.HalfPresent;
                                break;
                            default:
                                break;
                        }
                    }
                    if (item.isJoined == false && item.isHoliday == false) { color = ColorClass.NotJoined; cell.Value = ""; }
                    if (color != "")
                    {
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(color));
                    }

                    colIndex++;
                }
                #endregion

                #region "Set Data and BackColor in Row-Leave"
                rowIndex++; colIndex = 8;
                ws.Cells[rowIndex, 1, rowIndex, 6].Merge = true;
                ws.Cells[rowIndex, 7].Value = "Leave";
                foreach (var item in lstObjDetail)
                {
                    //Set Data
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Set BackColor
                    string color = "";
                    if (item.Edate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        color = ColorClass.Sunday;
                    }
                    else if (Convert.ToDouble(string.IsNullOrEmpty(item.L) ? "0" : item.L) == 0.5)
                    {
                        cell.Value = float.Parse(item.L);
                        AddRemarkToCell(cell, item.Remark);
                        color = ColorClass.Leave;
                    }
                    if (color != "")
                    {
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(color));
                    }

                    colIndex++;
                }
                #endregion

                #region "Set Data and BackColor in Row-Absence"
                rowIndex++; colIndex = 8;
                ws.Cells[rowIndex, 1, rowIndex, 6].Merge = true;
                ws.Cells[rowIndex, 7].Value = "Absence";
                foreach (var item in lstObjDetail)
                {
                    //Set Data
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Set BackColor
                    string color = "";
                    if (item.Edate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        color = ColorClass.Sunday;
                    }
                    else if (Convert.ToDouble(string.IsNullOrEmpty(item.A) ? "0" : item.A) == 0.5)
                    {
                        cell.Value = float.Parse(item.A);
                        AddRemarkToCell(cell, item.Remark);
                        color = ColorClass.Absence;
                    }
                    if (color != "")
                    {
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(color));
                    }

                    colIndex++;
                }
                #endregion

                #region "Set Data and BackColor in Row-OvertTime"
                rowIndex++; colIndex = 8;
                ws.Cells[rowIndex, 1, rowIndex, 6].Merge = true;
                ws.Cells[rowIndex, 7].Value = "OT";
                ws.Cells[rowIndex, 1, rowIndex, 76 + TotalDays].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                foreach (var item in lstObjDetail)
                {
                    //Set Data
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Set BackColor
                    string color = "";
                    if (Convert.ToDouble(string.IsNullOrEmpty(item.O) ? "0" : item.O) == 1 || Convert.ToDouble(string.IsNullOrEmpty(item.O) ? "0" : item.O) == 0.5)
                    {
                        cell.Value = float.Parse(item.O);
                        AddRemarkToCell(cell, item.Remark);
                        color = ColorClass.OT;
                    }
                    else if (item.Edate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        color = ColorClass.Sunday;
                    }
                    if (color != "")
                    {
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(color));
                    }

                    colIndex++;
                }
                #endregion

            }

            #region "Set Borders and Alignments in Worksheet"
            int rowCount = ws.Dimension.End.Row;
            int colCount = ws.Dimension.End.Column;
            for (int i = 1; i <= rowCount; i++)
            {
                //Set Center Alignment for all Cells in worksheet
                for (int j = 1; j <= colCount; j++)
                {
                    ws.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //Set Right-Most Border After Each Row
                ws.Cells[i, colCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //Set Left Border Of EmpName Column
                ws.Cells[i, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            }
            #endregion
        }

        public static void AddRemarkToCell(ExcelRange cell, string Remark)
        {
            if (!string.IsNullOrEmpty(Remark) && !string.IsNullOrWhiteSpace(Remark))
            {
                cell.AddComment(Remark, "Remark");
            }
        }

        #endregion

    }

    public class ColorClass
    {
        public static string FullPresent = "#8DB4E3";
        public static string HalfPresent = "#FFFF00";
        public static string Holiday = "#FFC000";
        public static string Absence = "#FF0000";
        public static string Leave = "#00B050";
        public static string OT = "#87CEEB";
        public static string Sunday = "#E6B9B8";
        public static string NotJoined = "#EBEBEB";
    }
}