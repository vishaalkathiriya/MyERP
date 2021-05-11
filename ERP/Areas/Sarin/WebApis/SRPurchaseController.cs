using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace ERP.WebApis
{
    public class SRPurchaseController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Purchase Entry";

        public SRPurchaseController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRPurchase
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRPurchase(tblSRPurchase srPurchase)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (srPurchase.PurchaseId == 0) // Mode == Add
                    {
                        tblSRPurchase p = new tblSRPurchase
                        {
                            PartId = srPurchase.PartId,
                            PurchaseDate = srPurchase.PurchaseDate,
                            Quantity = srPurchase.Quantity,
                            ApprovedBy = srPurchase.ApprovedBy,
                            Attachment = srPurchase.Attachment,
                            CreBy = srPurchase.CreBy,
                            ChgBy = srPurchase.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = srPurchase.Remarks
                        };
                        db.tblSRPurchases.Add(p);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRPurchases.Where(z => z.PurchaseId == srPurchase.PurchaseId).SingleOrDefault();
                        if (line != null)
                        {
                            line.PartId = srPurchase.PartId;
                            line.PurchaseDate = srPurchase.PurchaseDate;
                            line.Quantity = srPurchase.Quantity;
                            line.ApprovedBy = srPurchase.ApprovedBy;
                            line.Attachment = srPurchase.Attachment;
                            line.ChgBy = srPurchase.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = srPurchase.Remarks;
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    db.SaveChanges();
                    MoveFile(srPurchase.Attachment);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/SRPurchase
        /// retrieve SR Part list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRPartList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRParts.OrderBy(z => z.PartName)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.PartId,
                            Label = z.PartName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/SRPurchase
        /// delete SR Part
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRPurchase([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRPurchases.Where(z => z.PurchaseId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRPurchases.Remove(line);
                    }
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.Attachment))
                    {
                        DeletePurchasePicture(line.Attachment);
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/SRPurchage
        /// return SR-Purchase list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRPurchaseList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string srPartNameFilter = nvc["filter1"];
                    string srQtyFilter = nvc["filter2"];
                    string srApprovedByFilter = nvc["filter3"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRPurchase> list = null;
                    List<SRPurchaseViewModel> lstPurchase = new List<SRPurchaseViewModel>();
                    try
                    {
                        list = db.tblSRPurchases.ToList();

                        //1. filter SR Quantity column if exists
                        if (!string.IsNullOrEmpty(srQtyFilter) && srQtyFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Quantity.ToString().ToLower().Contains(srQtyFilter.ToLower())).ToList();
                        }
                        //2. filter SR Approved column if exists
                        if (!string.IsNullOrEmpty(srApprovedByFilter) && srApprovedByFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ApprovedBy.ToLower().Contains(srApprovedByFilter.ToLower())).ToList();
                        }

                        //3. convert returned datetime to local timezone & insert PartName col from different table
                        foreach (var l in list)
                        {
                            SRPurchaseViewModel p = new SRPurchaseViewModel
                            {
                                PurchaseId = l.PurchaseId,
                                PartId = l.PartId,
                                PartName = db.tblSRParts.Where(z => z.PartId == l.PartId).Select(z => z.PartName).SingleOrDefault(),
                                PurchaseDate = l.PurchaseDate,
                                Quantity = l.Quantity,
                                ApprovedBy = l.ApprovedBy,
                                Attachment = l.Attachment,
                                FullAttachmentPath = "/" + ConfigurationManager.AppSettings["UploadSarinPurchase"].ToString() + @"\" + l.Attachment,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstPurchase.Add(p);
                        }

                        //4. filter on SR Part Name col if exists
                        if (!string.IsNullOrEmpty(srPartNameFilter) && srPartNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPurchase = lstPurchase.Where(z => z.PartName.ToLower().Contains(srPartNameFilter.ToLower())).ToList();
                        }

                        //5. do sorting on list
                        lstPurchase = DoSorting(lstPurchase, orderBy.Trim());

                        //6. take total count to return for ng-table
                        var Count = lstPurchase.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstPurchase.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
                        lstPurchase = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<SRPurchaseViewModel> DoSorting(List<SRPurchaseViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "PartName")
                {
                    list = list.OrderBy(z => z.PartName).ToList();
                }
                else if (orderBy == "-PartName")
                {
                    list = list.OrderByDescending(z => z.PartName).ToList();
                }
                else if (orderBy == "PurchaseDate")
                {
                    list = list.OrderBy(z => z.PurchaseDate).ToList();
                }
                else if (orderBy == "-PurchaseDate")
                {
                    list = list.OrderByDescending(z => z.PurchaseDate).ToList();
                }
                else if (orderBy == "Quantity")
                {
                    list = list.OrderBy(z => z.Quantity).ToList();
                }
                else if (orderBy == "-Quantity")
                {
                    list = list.OrderByDescending(z => z.Quantity).ToList();
                }
                else if (orderBy == "ApprovedBy")
                {
                    list = list.OrderBy(z => z.ApprovedBy).ToList();
                }
                else if (orderBy == "-ApprovedBy")
                {
                    list = list.OrderByDescending(z => z.ApprovedBy).ToList();
                }
                else if (orderBy == "Remarks")
                {
                    list = list.OrderBy(z => z.Remarks).ToList();
                }
                else if (orderBy == "-Remarks")
                {
                    list = list.OrderByDescending(z => z.Remarks).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRPurchaseViewModel>();
            }
            catch
            {
                return null;
            }
        }

        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving purchase picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadSarinPurchase"].ToString()) + "/" + fileName;
                System.IO.File.Move(sourceFile, destinationFile);
            }
        }

        /// <summary>
        /// //delete purchase picture
        /// </summary>
        private void DeletePurchasePicture(string attachment)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadSarinPurchase"].ToString());

            if (File.Exists(Path.Combine(mainPath, attachment)))
            {
                File.Delete(Path.Combine(mainPath, attachment));
            }
        }
        #endregion
    }
}
