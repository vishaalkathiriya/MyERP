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
using System.Configuration;
using System.IO;


namespace ERP.WebApis
{
    public class HRDMedicalHelpController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "MedicalHelp";

        public HRDMedicalHelpController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }


        //BEGIN CREATE AND UPDATE MEDICAL INFORMATION WITH WEB API        
        [HttpPost]
        public ApiResponse SaveMedicalHelp(tblHRDMedicalHelp medHelp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (medHelp.IsPatelSocialGroup==true)
                    {
                        if (medHelp.SrNo == 0)
                        {// Mode == Add
                            tblHRDMedicalHelp d = new tblHRDMedicalHelp
                            {
                                ECode = medHelp.ECode,
                                EmployeeName = medHelp.EmployeeName,
                                PatientName = medHelp.PatientName,
                                Relation = medHelp.Relation,
                                HospitalName = medHelp.HospitalName,
                                ChequeNumber = medHelp.ChequeNumber,
                                ChequeIssueDate = medHelp.ChequeIssueDate,
                                ReceiverName = medHelp.ReceiverName,
                                MobileNumber = medHelp.MobileNumber,
                                Amount = medHelp.Amount,
                                QuotationAmount =(System.Decimal) medHelp.QuotationAmount,
                                ApprovedBy = medHelp.ApprovedBy,
                                Attachment = medHelp.Attachment,
                                IsPatelSocialGroup=medHelp.IsPatelSocialGroup,
                                CreDate=DateTime.Now.ToUniversalTime(),
                                ChgDate=DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };
                            db.tblHRDMedicalHelps.Add(d);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        else
                        {// Mode == Edit
                            var line = db.tblHRDMedicalHelps.Where(z => z.SrNo == medHelp.SrNo).SingleOrDefault();
                            if (line != null)
                            {
                                line.ECode = medHelp.ECode;
                                line.EmployeeName = medHelp.EmployeeName;
                                line.PatientName = medHelp.PatientName;
                                line.ReceiverName = medHelp.ReceiverName;
                                line.HospitalName = medHelp.HospitalName;
                                line.ChequeNumber = medHelp.ChequeNumber;
                                line.ChequeIssueDate = medHelp.ChequeIssueDate;
                                line.ReceiverName = medHelp.ReceiverName;
                                line.MobileNumber = medHelp.MobileNumber;
                                line.Amount = medHelp.Amount;
                                line.Attachment = medHelp.Attachment;
                                line.QuotationAmount=medHelp.QuotationAmount;
                                line.ApprovedBy=medHelp.ApprovedBy;
                                line.IsPatelSocialGroup=medHelp.IsPatelSocialGroup;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            //    UpdateSequence(medHelp.SrNo);
                            //   UpdateSequence(mod.SeqNo, mod.ModuleId);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                           
                        }
                    }else{

                         if (medHelp.SrNo == 0)
                    {// Mode == Add
                        tblHRDMedicalHelp d= new tblHRDMedicalHelp
                        {
                            ECode=medHelp.ECode,
                            EmployeeName=medHelp.EmployeeName,
                            PatientName=medHelp.PatientName,
                            Relation=medHelp.Relation,
                            HospitalName=medHelp.HospitalName,
                            ChequeNumber=medHelp.ChequeNumber,
                            ChequeIssueDate = medHelp.ChequeIssueDate,
                            ReceiverName=medHelp.ReceiverName,
                            MobileNumber=medHelp.MobileNumber,
                            Amount=medHelp.Amount,
                            QuotationAmount=0,
                            ApprovedBy = string.Empty,
                            IsPatelSocialGroup = false,
                            Attachment=medHelp.Attachment,
                            CreDate=DateTime.Now.ToUniversalTime(),
                            ChgDate=DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())

                        };
                        db.tblHRDMedicalHelps.Add(d);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDMedicalHelps.Where(z => z.SrNo == medHelp.SrNo).SingleOrDefault();
                        if (line != null)
                        {
                            line.ECode = medHelp.ECode;
                            line.EmployeeName = medHelp.EmployeeName;
                            line.PatientName = medHelp.PatientName;
                            line.ReceiverName = medHelp.ReceiverName;
                            line.HospitalName = medHelp.HospitalName;
                            line.ChequeNumber = medHelp.ChequeNumber;
                            line.ChequeIssueDate = medHelp.ChequeIssueDate;
                            line.ReceiverName = medHelp.ReceiverName;
                            line.MobileNumber = medHelp.MobileNumber;
                            line.Amount = medHelp.Amount;
                            line.Attachment = medHelp.Attachment;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            /*
                            line.QuotationAmount = medHelp.QuotationAmount;
                            line.ApprovedBy = medHelp.ApprovedBy;
                            line.IsPatelSocialGroup = medHelp.IsPatelSocialGroup;
                             */
                            line.QuotationAmount = 0;
                            line.ApprovedBy = string.Empty;
                            line.IsPatelSocialGroup = false;
                        }
                       //    UpdateSequence(medHelp.SrNo);
                     //   UpdateSequence(mod.SeqNo, mod.ModuleId);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }


                    }
                    MoveFile(medHelp.Attachment);
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
        //END CREATE AND UPDATE MEDICAL INFORMATION WITH WEB API


        //BEGIN DELETE MEDICAL INFORMATION BASE SRNO
        [HttpPost]
        public ApiResponse DeleteMedicalHelp([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
                try
                {
                        var line = db.tblHRDMedicalHelps.Where(z => z.SrNo == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblHRDMedicalHelps.Remove(line);
                            db.SaveChanges();

                            if (!string.IsNullOrEmpty(line.Attachment)) {
                                DeleteProfilePicture(line.Attachment);
                            }
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }

                       
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            return apiResponse;
        }
        //END DELETE MEDICAL INFORMATION BASE SRNO
        
        //BEGIN A LIST OF MEDICAL INFORMATION WITH SORTING AND FILTERING
        [HttpGet]
        public ApiResponse GetMedicalHelpList()
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
                    string ECode = nvc["ECode"]; //for col Employee ECode 0
                    string EmployeeName = nvc["EmployeeName"]; //for col EmployeeName 0
                    string PatientName = nvc["PatientName"]; //for col PatientName 0
                    string Relation = nvc["Relation"]; //for col Relation 0
                    string HospitalName = nvc["HospitalName"]; //for col HospitalName
                    string ChequeIssueDate = nvc["ChequeIssueDate"]; //for col ChequeIssueDate
                    string ChequeNumber = nvc["ChequeNumber"]; //for col ChequeNumber
                    string ReceiverName = nvc["ReceiverName"]; //for col ReceiverName
                    string MobileNumber = nvc["MobileNumber"]; //for col MobileNumber 0
                    string Amount = nvc["Amount"]; //for col Amount 0
                    string IsPatelSocialGroup = nvc["IsPatelSocialGroup"]; //for col Amount 0

                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDMedicalHelp> list = null;
                    try
                    {
                        list = db.tblHRDMedicalHelps.ToList();


                        //top filter
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.ChequeIssueDate.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.ChequeIssueDate.Date >= fromDate.Date && z.ChequeIssueDate.Date <= toDate.Date).ToList();
                            }
                        }



                        if (IsPatelSocialGroup == "Y")
                        {
                            
                             list = list.Where(z => z.IsPatelSocialGroup.Equals(true)).ToList();
                        }

                        if (IsPatelSocialGroup == "N")
                        {
                            
                            list = list.Where(z => z.IsPatelSocialGroup.Equals(false)).ToList();
                        }


                        //1. filter data
                        if (!string.IsNullOrEmpty(ECode) && ECode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ECode.ToLower().Contains(ECode.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(EmployeeName) && EmployeeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EmployeeName.ToLower().Contains(EmployeeName.ToLower())).ToList();
                        }


                        if (!string.IsNullOrEmpty(PatientName) && PatientName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.PatientName.ToLower().Contains(PatientName.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(Relation) && Relation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Relation.ToLower().Contains(Relation.ToLower())).ToList();
                        }

                        if (!string.IsNullOrEmpty(HospitalName) && HospitalName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.HospitalName.ToLower().Contains(HospitalName.ToLower())).ToList();
                        }

                        //if (!string.IsNullOrEmpty(ChequeIssueDate) && ChequeIssueDate != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.ChequeIssueDate.Contains(ChequeIssueDate.ToLower())).ToList();
                        //}

                        if (!string.IsNullOrEmpty(ChequeNumber) && ChequeNumber != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ChequeNumber.Contains(ChequeNumber.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(ReceiverName) && ReceiverName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToList();
                        }
                        //if (!string.IsNullOrEmpty(MobileNumber) && MobileNumber != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.MobileNumber.Contains(MobileNumber.ToLower())).ToList();
                        //}
                        //if (!string.IsNullOrEmpty(Amount) && Amount != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.Amount.Contains(Amount)).ToList();
                        //}

                        //if (!string.IsNullOrEmpty(IsPatelSocialGroup) && IsPatelSocialGroup != "undefined")
                        //{
                        //    iDisplayStart = 0;
                        //    list = list.Where(z => z.IsPatelSocialGroup).ToList();
                        //}
                        
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var Modules = list.Select(i =>
                        {                    

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
        //END A LIST OF MEDICAL INFORMATION WITH SORTING AND FILTERING

        //BEGIN A LIST OF MEDICAL INFORMATION BASE COLUMN WISE SORTING
        public List<tblHRDMedicalHelp> DoSorting(IEnumerable<tblHRDMedicalHelp> list, string orderBy)
        {
            try
            {
                if (orderBy == "ECode")
                {
                    list = list.OrderBy(z => z.ECode).ToList();
                }
                else if (orderBy == "-ECode")
                {
                    list = list.OrderByDescending(z => z.ECode).ToList();
                }       
                if (orderBy == "EmployeeName")
                {
                    list = list.OrderBy(z => z.EmployeeName).ToList();
                }
                else if (orderBy == "-EmployeeName")
                {
                    list = list.OrderByDescending(z => z.EmployeeName).ToList();
                }

                else if (orderBy == "PatientName")
                {
                    list = list.OrderBy(z => z.PatientName).ToList();
                }
                else if (orderBy == "-PatientName")
                {
                    list = list.OrderByDescending(z => z.PatientName).ToList();
                }
                else if (orderBy == "Relation")
                {
                    list = list.OrderBy(z => z.Relation).ToList();
                }
                else if (orderBy == "-Relation")
                {
                    list = list.OrderByDescending(z => z.Relation).ToList();
                }
                else if (orderBy == "HospitalName")
                {
                    list = list.OrderBy(z => z.HospitalName).ToList();
                }
                else if (orderBy == "-HospitalName")
                {
                    list = list.OrderByDescending(z => z.HospitalName).ToList();
                }
                else if (orderBy == "ChequeIssueDate")
                {
                    list = list.OrderBy(z => z.ChequeIssueDate).ToList();
                }
                else if (orderBy == "-ChequeIssueDate")
                {
                    list = list.OrderByDescending(z => z.ChequeIssueDate).ToList();
                }
                else if (orderBy == "ChequeNumber")
                {
                    list = list.OrderBy(z => z.ChequeNumber).ToList();
                }
                else if (orderBy == "-ChequeNumber")
                {
                    list = list.OrderByDescending(z => z.ChequeNumber).ToList();
                }
                else if (orderBy == "ReceiverName")
                {
                    list = list.OrderBy(z => z.ReceiverName).ToList();
                }
                else if (orderBy == "-ReceiverName")
                {
                    list = list.OrderByDescending(z => z.ReceiverName).ToList();
                }
                else if (orderBy == "MobileNumber")
                {
                    list = list.OrderBy(z => z.MobileNumber).ToList();
                }
                else if (orderBy == "-MobileNumber")
                {
                    list = list.OrderByDescending(z => z.MobileNumber).ToList();
                }
                else if (orderBy == "Amount")
                {
                    list = list.OrderBy(z => z.Amount).ToList();
                }
                else if (orderBy == "-Amount")
                {
                    list = list.OrderByDescending(z => z.Amount).ToList();
                }
                else if (orderBy == "IsPatelSocialGroup")
                {
                    list = list.OrderBy(z => z.IsPatelSocialGroup).ToList();
                }
                else if (orderBy == "-IsPatelSocialGroup")
                {
                    list = list.OrderByDescending(z => z.IsPatelSocialGroup).ToList();
                }




                
                return list.ToList<tblHRDMedicalHelp>();
            }
            catch
            {
                return null;
            }
        }
        //END A LIST OF MEDICAL INFORMATION BASE COLUMN WISE SORTING



        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadMedicalHelp"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);
                // System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadMedicalHelp"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
        #endregion
    }
}