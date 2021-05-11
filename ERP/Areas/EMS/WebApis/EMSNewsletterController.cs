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
using Newtonsoft.Json;

namespace ERP.Areas.EMS.WebApis
{
    public class EMSNewsletterPrepareDataBindingModel
    {
        public int newsletterId { get; set; }
        public string datetime { get; set; }
    }
    public class EMSNewsletterController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Newsletter";

        public EMSNewsletterController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse PostNewsletter(tblEMSNewsletter newsletter)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (newsletter.NewsletterID == 0)
                    {
                        db.tblEMSNewsletters.Add(newsletter);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, newsletter);
                    }
                    else
                    {// Mode == Edit
                        var oldNewsletter = db.tblEMSNewsletters.Where(g => g.NewsletterID == newsletter.NewsletterID).SingleOrDefault();
                        if (oldNewsletter != null)
                        {
                            oldNewsletter.Subject = newsletter.Subject;
                            oldNewsletter.HeaderAndFooter = newsletter.Subject;
                            oldNewsletter.HTML = newsletter.HTML;
                            oldNewsletter.IsDeleted = newsletter.IsDeleted;
                            oldNewsletter.NrOpened = newsletter.NrOpened;
                            oldNewsletter.Too = newsletter.Too;
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, newsletter);
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

        [HttpPost]
        public ApiResponse PostNewsletterPrepare(EMSNewsletterPrepareDataBindingModel data)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    using (db)
                    {
                        //DD-MM-YYYY HH:mm
                        var scheduleDate = data.datetime.Split(' ')[0].Split('-').Select(Int32.Parse).ToList();
                        var scheduleTime = data.datetime.Split(' ')[1].Split(':').Select(Int32.Parse).ToList();
                        var scheduleDateTime = new DateTime(scheduleDate[2], scheduleDate[1], scheduleDate[0], scheduleTime[0], scheduleTime[1], 0);

                        var newsletter = db.tblEMSNewsletters.Where(nl => nl.NewsletterID == data.newsletterId).SingleOrDefault();
                        if (newsletter != null)
                        {
                            var groupIds = newsletter.Too.Split(',').ToList();
                            foreach (var groupId in groupIds)
                            {
                                db.tblEMSMailSendPrepares.Add(new tblEMSMailSendPrepare
                                {
                                    ClientGroupID = Convert.ToInt32(groupId),
                                    ClientID = 0,
                                    DatePickup = scheduleDateTime,
                                    NewsletterID = data.newsletterId,
                                    IsDeleted = false
                                });
                                // var allClients = new List<tblEMSClient>();
                                //var group = db.tblEMSGroups.Find(Convert.ToInt32(groupId));
                                //if (group != null)
                                //{
                                //    var groupClients = db.tblEMSGroupClients.Where(gc => gc.ClientGroupID == group.ClientGroupID).ToList();
                                //    foreach (var groupClient in groupClients)
                                //    {
                                //        var client = db.tblEMSClients.Find(groupClient.ClientID);
                                //        if (client != null) allClients.Add(client);
                                //    }
                                //}
                            }
                            db.SaveChanges();
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, "");
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

        [HttpGet]
        public ApiResponse GetAllNewsletters()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    var newsletters = new List<tblEMSNewsletter>();
                    try
                    {
                        newsletters = db.tblEMSNewsletters.Where(g => g.IsDeleted == false).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", newsletters);
                    }
                    catch (Exception)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        newsletters = null;
                    }

                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse GetClientGroups()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string searchString = nvc["searchString"].ToString();
                    var groups = new List<tblEMSGroup>();
                    try
                    {
                        groups = db.tblEMSGroups.Where(g => g.Name.Contains(searchString)).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", groups);
                    }
                    catch (Exception)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        groups = null;
                    }
                    
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        public ApiResponse GetclientGroupsByIds()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string groupIds = nvc["groupIds"].ToString();
                    if (!String.IsNullOrWhiteSpace(groupIds))
                    {
                        List<int> lstGroupIds = groupIds.Split(',').Select(Int32.Parse).ToList();
                        var groups = new List<tblEMSGroup>();
                        try
                        {
                            groups = db.tblEMSGroups.Where(g => lstGroupIds.Contains(g.ClientGroupID)).ToList();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", groups);
                        }
                        catch (Exception)
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                        }
                        finally
                        {
                            groups = null;
                        }
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", "");
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse GetNewsletterById(int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    var newsletter = new tblEMSNewsletter();
                    try
                    {
                        newsletter = db.tblEMSNewsletters.Find(id);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", newsletter);
                    }
                    catch (Exception)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        newsletter = null;
                    }

                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
    }
}