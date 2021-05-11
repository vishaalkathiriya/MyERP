using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class ARModuleController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Module";


        public ARModuleController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/ARModule
        /// create and update module
        /// </summary>
        [HttpPost]
        public ApiResponse SaveModule(tblARModule mod)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (mod.ModuleId == 0)
                    {// Mode == Add
                        tblARModule d = new tblARModule
                        {
                            ModuleName = mod.ModuleName,
                            //SeqNo = mod.SeqNo,
                            IsActive = mod.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblARModules.Add(d);
                        db.SaveChanges();
                        int newModuleId = db.tblARModules.Max(z => z.ModuleId);
                        UpdateSequence(mod.SeqNo, newModuleId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblARModules.Where(z => z.ModuleId == mod.ModuleId).SingleOrDefault();
                        if (line != null)
                        {
                            line.ModuleName = mod.ModuleName;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        UpdateSequence(mod.SeqNo, mod.ModuleId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }

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
        /// POST api/ARModule
        /// delete module 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteModule([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.ModuleStatus(id))
                    {
                        var line = db.tblARModules.Where(z => z.ModuleId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblARModules.Remove(line);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
                    }
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
        /// GET api/ARModule
        /// active-inActive record 
        /// </summary>
        [HttpGet]
        public ApiResponse ChangeStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int moduleId = Convert.ToInt32(nvc["moduleId"]);
                    Boolean isActive = Convert.ToBoolean(nvc["isActive"]);

                    if (DependancyStatus.ModuleStatus(moduleId))
                    {
                        var line = db.tblARModules.Where(z => z.ModuleId == moduleId).SingleOrDefault();
                        if (line != null)
                        {
                            if (isActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!isActive)
                            {
                                line.IsActive = true;
                            }
                        }

                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                    }
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
        /// GET api/ARModule
        /// return document list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleList()
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
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblARModule> list = null;
                    try
                    {
                        list = db.tblARModules.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ModuleName.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var Modules = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = Modules
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
        public List<tblARModule> DoSorting(IEnumerable<tblARModule> list, string orderBy)
        {
            try
            {
                if (orderBy == "ModuleName")
                {
                    list = list.OrderBy(z => z.ModuleName).ToList();
                }
                else if (orderBy == "-ModuleName")
                {
                    list = list.OrderByDescending(z => z.ModuleName).ToList();
                }
                if (orderBy == "SeqNo")
                {
                    list = list.OrderBy(z => z.SeqNo).ToList();
                }
                else if (orderBy == "-SeqNo")
                {
                    list = list.OrderByDescending(z => z.SeqNo).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblARModule>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GET api/ARModule
        /// update module seq number
        /// </summary>
        [HttpGet]
        public ApiResponse UpdateSequenceNo()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int seqNo = Convert.ToInt32(nvc["seqNo"]);
                    int moduleId = Convert.ToInt32(nvc["moduleId"]);

                    try
                    {
                        UpdateSequence(seqNo, moduleId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, true);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        private void UpdateSequence(int seqNo, int moduleId)
        {
            try
            {
                int oldSeqNo;
                var line = db.tblARModules.Where(z => z.ModuleId == moduleId).SingleOrDefault();
                if (line != null)
                {
                    if (line.SeqNo != 0)
                    {
                        oldSeqNo = line.SeqNo;

                        // BEGIN COMMENTED BY DRCVHK
                        ////match seq number in old values
                        //var matched = db.tblARModules.Where(z => z.SeqNo == seqNo).SingleOrDefault();
                        //if (matched != null)
                        //{//Exists
                        //    matched.SeqNo = line.SeqNo; // swap value
                        //    line.SeqNo = seqNo; // place passed value here
                        //}
                        //else
                        //{//Not Exists
                        //    line.SeqNo = db.tblARModules.Max(z => z.SeqNo);
                        //    var afterList = db.tblARModules.Where(z => z.SeqNo > oldSeqNo).ToList();
                        //    foreach (var a in afterList)
                        //    {
                        //        a.SeqNo = a.SeqNo - 1;
                        //    }
                        //} 
                        // END COMMENTED BY DRCVHK

                        if (oldSeqNo > seqNo)
                        {
                            var list = db.tblARModules.Where(z => z.SeqNo >= seqNo && z.SeqNo < oldSeqNo).ToList();
                            foreach (var item in list)
                            {
                                item.SeqNo = item.SeqNo + 1;
                            }
                        }
                        else if (oldSeqNo < seqNo)
                        {
                            var list = db.tblARModules.Where(z => z.SeqNo > oldSeqNo && z.SeqNo <= seqNo).ToList();
                            foreach (var item in list)
                            {
                                item.SeqNo = item.SeqNo - 1;
                            }
                        }
                        if (seqNo >= db.tblARModules.Max(z => z.SeqNo))
                        {
                            line.SeqNo = db.tblARModules.Max(z => z.SeqNo);
                        }
                        else
                        {
                            line.SeqNo = seqNo;
                        }
                    }
                    else
                    {
                        var list = db.tblARModules.Where(z => z.SeqNo >= seqNo).ToList();
                        foreach (var item in list)
                        {
                            item.SeqNo = item.SeqNo + 1;
                        }
                        line.SeqNo = seqNo;
                    }
                    line.ChgDate = DateTime.Now.ToUniversalTime();
                    line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
