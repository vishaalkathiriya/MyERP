using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Utilities;
using System.Collections.Specialized;
using System.Web;
using System.Web.Script.Serialization;

namespace ERP.WebApis
{
    public class ARSubModuleController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Sub Module";

        public ARSubModuleController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/ARSubModule
        /// retrieve module list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblARModules.OrderBy(z => z.ModuleName)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.ModuleId,
                            Label = z.ModuleName
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
        /// GET api/ARSubModule
        /// retrieve access permission list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetAccessPermissionList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    bool isDashboard = Convert.ToBoolean(nvc["isDashboard"].ToString());
                    bool isEmployee = Convert.ToBoolean(nvc["isEmployee"].ToString());
                    bool isInvoice = Convert.ToBoolean(nvc["isInvoice"].ToString());

                    List<SelectItemAccessRights> list = new List<SelectItemAccessRights>();
                    if (isDashboard)
                    { //return dashboard enum 
                        foreach (int value in Enum.GetValues(typeof(ERPUtilities.DashboardWidget)))
                        {
                            list.Add(new SelectItemAccessRights
                            {
                                Id = value,
                                Label = Enum.GetName(typeof(ERPUtilities.DashboardWidget), value),
                                IsSelected = false
                            });
                        }
                    }
                    else
                    { //return normal access permission enum
                        foreach (int value in Enum.GetValues(typeof(ERPUtilities.AccessPermission)))
                        {
                            list.Add(new SelectItemAccessRights
                            {
                                Id = value,
                                Label = Enum.GetName(typeof(ERPUtilities.AccessPermission), value),
                                IsSelected = false
                            });
                        }
                    }

                    if (isEmployee)
                    {
                        foreach (int value in Enum.GetValues(typeof(ERPUtilities.AccessPermissionEmployeeTab)))
                        {
                            list.Add(new SelectItemAccessRights
                            {
                                Id = value,
                                Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionEmployeeTab), value),
                                IsSelected = false
                            });
                        }
                    }

                    if (isInvoice)
                    {
                        foreach (int value in Enum.GetValues(typeof(ERPUtilities.AccessPermissionInvoiceTab)))
                        {
                            list.Add(new SelectItemAccessRights
                            {
                                Id = value,
                                Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionInvoiceTab), value),
                                IsSelected = false
                            });
                        }
                    }
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
        /// POST api/ARSubModule
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblARSubModule sub)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblARSubModules.Where(z => z.SubModuleId == sub.SubModuleId).FirstOrDefault();
                    if (line != null)
                    {
                        if (sub.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!sub.IsActive)
                        {
                            line.IsActive = true;
                        }
                        line.ChgDate = DateTime.Now.ToUniversalTime();

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
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
        /// POST api/ARSubModule
        /// delete sub module 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSubModule(tblARSubModule sub)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblARSubModules.Where(z => z.SubModuleId == sub.SubModuleId).FirstOrDefault();
                    if (line != null)
                    {
                        db.tblARSubModules.Remove(line);
                    }

                    db.SaveChanges();
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
        /// POST api/ARSubModule
        /// create and update sub module
        /// </summary>
        [HttpPost]
        public ApiResponse CreateUpdateSubModule(tblARSubModule smod)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);

                    if (smod.SubModuleId == 0)
                    {// Mode == Add
                        tblARSubModule sm = new tblARSubModule
                        {
                            ModuleId = smod.ModuleId,
                            SubModuleName = smod.SubModuleName,
                            URL = smod.URL,
                            AllowedAccess = smod.AllowedAccess,
                            //SeqNo = smod.SeqNo,
                            IsActive = smod.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblARSubModules.Add(sm);
                        db.SaveChanges();
                        int newSubModuleId = db.tblARSubModules.Max(z => z.SubModuleId);
                        UpdateSequence(smod.SeqNo, newSubModuleId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblARSubModules.Where(z => z.SubModuleId == smod.SubModuleId).SingleOrDefault();
                        if (line != null)
                        {
                            line.ModuleId = smod.ModuleId;
                            line.SubModuleName = smod.SubModuleName;
                            line.URL = smod.URL;
                            line.AllowedAccess = smod.AllowedAccess;
                            line.IsActive = smod.IsActive;
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                        }

                        UpdateSequence(smod.SeqNo, smod.SubModuleId);
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
        /// POST api/IssuedDocument
        /// return sub module list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetSubModuleList(tblARSubModule sm)
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

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    //List<tblARSubModule> list = null;
                    List<ARSubModuleViewModel> list = new List<ARSubModuleViewModel>();
                    try
                    {
                        var smList = db.tblARSubModules.AsEnumerable().ToList();

                        //filter on columns
                        if (sm.ModuleId != 0)
                        {
                            iDisplayStart = 0;
                            smList = smList.Where(z => z.ModuleId == sm.ModuleId).ToList();
                        }
                        if (!string.IsNullOrEmpty(sm.SubModuleName) && sm.SubModuleName != "undefined")
                        {
                            iDisplayStart = 0;
                            smList = smList.Where(z => z.SubModuleName.ToLower().Contains(sm.SubModuleName.ToLower())).ToList();
                        }


                        //get sub row for module id
                        foreach (var item in smList)
                        {
                            List<SelectItemAccessRights> aList = new List<SelectItemAccessRights>();
                            if (!string.IsNullOrEmpty(item.AllowedAccess))
                            {
                                foreach (string l in item.AllowedAccess.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(l))
                                    {
                                        if (item.ModuleId == 1 && item.SubModuleName.ToLower() == "dashboard")
                                        {
                                            aList.Add(new SelectItemAccessRights
                                            {
                                                Id = Convert.ToInt32(l),
                                                Label = Enum.GetName(typeof(ERPUtilities.DashboardWidget), Convert.ToInt32(l)),
                                                IsSelected = true
                                            });
                                        }
                                        else
                                        {
                                            if (Enum.GetName(typeof(ERPUtilities.AccessPermission), Convert.ToInt32(l)) != null)
                                            {
                                                aList.Add(new SelectItemAccessRights
                                                {
                                                    Id = Convert.ToInt32(l),
                                                    Label = Enum.GetName(typeof(ERPUtilities.AccessPermission), Convert.ToInt32(l)),
                                                    IsSelected = true
                                                });
                                            }
                                            else
                                            {
                                                if (item.SubModuleName.ToLower() == "manage employee")
                                                {
                                                    aList.Add(new SelectItemAccessRights
                                                    {
                                                        Id = Convert.ToInt32(l),
                                                        Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionEmployeeTab), Convert.ToInt32(l)),
                                                        IsSelected = true
                                                    });
                                                }
                                                else if (item.SubModuleName.ToLower() == "conversation")
                                                {
                                                    aList.Add(new SelectItemAccessRights
                                                    {
                                                        Id = Convert.ToInt32(l),
                                                        Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionInvoiceTab), Convert.ToInt32(l)),
                                                        IsSelected = true
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            list.Add(new ARSubModuleViewModel
                            {
                                SubModuleId = item.SubModuleId,
                                ModuleId = item.ModuleId,
                                ModuleName = db.tblARModules.Where(z => z.ModuleId == item.ModuleId).Select(z => z.ModuleName).SingleOrDefault(),
                                SubModuleName = item.SubModuleName,
                                URL = item.URL,
                                AllowedAccess = aList,
                                SeqNo = item.SeqNo,
                                IsActive = item.IsActive,
                                ChgDate = item.ChgDate

                            });
                        }

                        //convert returned datetime to local timezone
                        var lstSMod = list.Select(i =>
                        {
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        });

                        //do sorting on list
                        list = DoSorting(lstSMod, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
        public List<ARSubModuleViewModel> DoSorting(IEnumerable<ARSubModuleViewModel> list, string orderBy)
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
                if (orderBy == "SubModuleName")
                {
                    list = list.OrderBy(z => z.SubModuleName).ToList();
                }
                else if (orderBy == "-SubModuleName")
                {
                    list = list.OrderByDescending(z => z.SubModuleName).ToList();
                }
                if (orderBy == "URL")
                {
                    list = list.OrderBy(z => z.URL).ToList();
                }
                else if (orderBy == "-URL")
                {
                    list = list.OrderByDescending(z => z.URL).ToList();
                }
                if (orderBy == "SeqNo")
                {
                    list = list.OrderBy(z => z.SeqNo).ToList();
                }
                else if (orderBy == "-SeqNo")
                {
                    list = list.OrderByDescending(z => z.SeqNo).ToList();
                }
                if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<ARSubModuleViewModel>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GET api/ARSubModule
        /// update sub module seq number
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
                    int subModuleId = Convert.ToInt32(nvc["subModuleId"]);

                    try
                    {
                        UpdateSequence(seqNo, subModuleId);
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



        private void UpdateSequence(int seqNo, int subModuleId)
        {
            try
            {
                int oldSeqNo;
                var line = db.tblARSubModules.Where(z => z.SubModuleId == subModuleId).SingleOrDefault();
                if (line != null)
                {
                    if (line.SeqNo != 0)
                    {
                        oldSeqNo = line.SeqNo;

                        // BEGIN COMMENTED BY DRCVHK
                        ////match seq number in old values
                        //var matched = db.tblARSubModules.Where(z => z.SeqNo == seqNo && z.ModuleId == line.ModuleId).SingleOrDefault();
                        //if (matched != null)
                        //{//Exists
                        //    matched.SeqNo = line.SeqNo; // swap value
                        //    line.SeqNo = seqNo; // place passed value here
                        //}
                        //else
                        //{//Not Exists
                        //    line.SeqNo = db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId).Max(z => z.SeqNo);
                        //    var afterList = db.tblARSubModules.Where(z => z.SeqNo > oldSeqNo).ToList();
                        //    foreach (var a in afterList)
                        //    {
                        //        a.SeqNo = a.SeqNo - 1;
                        //    }
                        //}
                        // END COMMENTED BY DRCVHK

                        if (oldSeqNo > seqNo)
                        {
                            var list = db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId && z.SeqNo >= seqNo && z.SeqNo < oldSeqNo).ToList();
                            foreach (var item in list)
                            {
                                item.SeqNo = item.SeqNo + 1;
                            }
                        }
                        else if (oldSeqNo < seqNo)
                        {
                            var list = db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId && z.SeqNo > oldSeqNo && z.SeqNo <= seqNo).ToList();
                            foreach (var item in list)
                            {
                                item.SeqNo = item.SeqNo - 1;
                            }
                        }
                        if (seqNo >= db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId).Max(z => z.SeqNo))
                        {
                            line.SeqNo = db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId).Max(z => z.SeqNo);
                        }
                        else
                        {
                            line.SeqNo = seqNo;
                        }
                    }
                    else
                    {
                        var list = db.tblARSubModules.Where(z => z.ModuleId == line.ModuleId && z.SeqNo >= seqNo).ToList();
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
