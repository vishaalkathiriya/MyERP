using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for TechnologyGroup
    /// </summary>
    public class TechnologyGroup : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var techList = db.tblTechnologiesGroups.Select(a => new { a.TechnologiesGroup, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.TechnologiesGroup).ToList();


            DataTable dt = ERPUtilities.ConvertToDataTable(techList);
            ERPUtilities.ExportExcel(context, timezone, dt, "TechnologyGroup List", "TechnologyGroup List", "TechnologyGroupList");
            context = null;
        }
        //    //set the workbook properties and add a default sheet in it
        //    SetWorkbookProperties(p);
        //    //Create a sheet
        //    ExcelWorksheet ws = CreateSheet(p, "Technology Group List");

        //    ERPContext db = new ERPContext();
        //    var techList = db.tblTechnologiesGroups.Select(a => new { a.TechnologiesGroup, a.ChgDate }).ToList().OrderBy(a => a.TechnologiesGroup);

        //    ws.Cells[1, 1].Value = "Technology Group List";
        //    ws.Cells[1, 1, 1, 2].Merge = true;
        //    ws.Cells[1, 1, 1, 2].Style.Font.Bold = true;
        //    ws.Cells[1, 1, 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //    int rowIndex = 2;

        //    CreateHeader(ws, ref rowIndex, techList);
        //    CreateData(ws, ref rowIndex, techList);

        //    ws.Cells[ws.Dimension.Address].AutoFitColumns();

        //    context.Response.BinaryWrite(p.GetAsByteArray());
        //    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    context.Response.AddHeader("content-disposition", "attachment;  filename=TechnologyGroupList.xlsx");
        //}


        ///// Sets the workbook properties and adds a default sheet.
        //private void SetWorkbookProperties(ExcelPackage p)
        //{
        //    //Here setting some document properties
        //    p.Workbook.Properties.Author = "DRC Infotech";
        //    p.Workbook.Properties.Title = "DRC ERP";
        //}

        //private ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        //{
        //    p.Workbook.Worksheets.Add(sheetName);
        //    ExcelWorksheet ws = p.Workbook.Worksheets[1];
        //    ws.Name = sheetName; //Setting Sheet's name
        //    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
        //    ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
        //    return ws;
        //}

        //private void CreateHeader(ExcelWorksheet ws, ref int rowIndex, dynamic Catlist)
        //{
        //    //int colIndex = 1;
        //    var cell = ws.Cells[rowIndex, 1];
        //    //Setting the background color of header cells to Gray
        //    var fill = cell.Style.Fill;
        //    fill.PatternType = ExcelFillStyle.Solid;
        //    fill.BackgroundColor.SetColor(Color.Gray);

        //    //Setting Top/left,right/bottom borders.
        //    var border = cell.Style.Border;
        //    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //    //Setting Value in cell
        //    cell.Value = "Technology Group";

        //    // For Second Column--------------------------------
        //    cell = ws.Cells[rowIndex, 2];
        //    //Setting the background color of header cells to Gray
        //    fill = cell.Style.Fill;
        //    fill.PatternType = ExcelFillStyle.Solid;
        //    fill.BackgroundColor.SetColor(Color.Gray);

        //    //Setting Top/left,right/bottom borders.
        //    border = cell.Style.Border;
        //    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //    //Setting Value in cell
        //    cell.Value = "Changed On";
        //}

        //private void CreateData(ExcelWorksheet ws, ref int rowIndex, dynamic techlist)
        //{
        //    foreach (var item in techlist) // Adding Data into rows
        //    {
        //        rowIndex++;

        //        var cell = ws.Cells[rowIndex, 1];
        //        //Setting Value in cell
        //        cell.Value = item.TechnologiesGroup;
        //        //Setting borders of cell
        //        var border = cell.Style.Border;
        //        border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
        //        // For Second Column
        //        cell = ws.Cells[rowIndex, 2];
        //        //Setting Value in cell
        //        cell.Value = (Convert.ToDateTime(item.ChgDate).AddMinutes(-1 * timezone)).ToString("dd-MMM-yyyy HH:mm");

        //        //Setting borders of cell
        //        border = cell.Style.Border;
        //        border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
        //    }
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
