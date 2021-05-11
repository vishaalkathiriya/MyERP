using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf.draw;
using System.IO;
using iTextSharp.text.xml;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Configuration;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for EmployeeViewProfilePDF
    /// </summary>
    public class EmployeeViewProfilePDF : IHttpHandler
    {
        ERPContext db = new ERPContext();

        //EmployeeId
        public void ProcessRequest(HttpContext context)
        {
            try
            {

                int id = Convert.ToInt32(context.Request.QueryString["EmployeeId"].ToString());
                string ProfileImage = ConfigurationManager.AppSettings["UploadPath"].ToString();
                string filename = context.Server.MapPath("~//" + ProfileImage + "//");

                var employee_data = (from empPersonalInfo in db.tblEmpPersonalInformations
                                     join payRollInfo in db.tblEmpPayRollInformations on empPersonalInfo.EmployeeId equals payRollInfo.EmployeeId into emp
                                     from payRoll in emp.DefaultIfEmpty()
                                     where empPersonalInfo.EmployeeId == id
                                     select new EmployeeViewProfileViewModel
                                     {
                                         EmployeeId = empPersonalInfo.EmployeeId,
                                         EmployeeRegisterCode = empPersonalInfo.EmployeeRegisterCode,
                                         CandidateFirstName = empPersonalInfo.CandidateFirstName,
                                         CandidateMiddleName = (empPersonalInfo.CandidateMiddleName.Length > 1 ? empPersonalInfo.CandidateMiddleName.Substring(0, 1) : empPersonalInfo.CandidateMiddleName),
                                         CandidateLastName = empPersonalInfo.CandidateLastName,
                                         GuardianFirstName = empPersonalInfo.GuardianFirstName,
                                         GuardianMiddleName = (empPersonalInfo.GuardianMiddleName.Length > 1 ? empPersonalInfo.GuardianMiddleName.Substring(0, 1) : empPersonalInfo.GuardianMiddleName),
                                         GuardianLastName = empPersonalInfo.GuardianLastName,
                                         ProfilePhoto = empPersonalInfo.ProfilePhoto,
                                         Present_HouseNo = empPersonalInfo.Present_HouseNo,
                                         Present_Location = empPersonalInfo.Present_Location,
                                         Present_Area = empPersonalInfo.Present_Area,
                                         Present_Country = db.tblCountries.Where(z => z.CountryId == empPersonalInfo.Present_Country).Select(z => z.CountryName).FirstOrDefault(),
                                         Present_State = db.tblStates.Where(z => z.StateId == empPersonalInfo.Present_State).Select(z => z.StateName).FirstOrDefault(),
                                         Present_City = empPersonalInfo.Present_City,
                                         Present_PostalCode = empPersonalInfo.Present_PostalCode,
                                         Permanent_HouseNo = empPersonalInfo.Permanent_HouseNo,
                                         Permanent_Location = empPersonalInfo.Permanent_Location,
                                         Permanent_Area = empPersonalInfo.Permanent_Area,
                                         Permanent_Country = db.tblCountries.Where(z => z.CountryId == empPersonalInfo.Permanent_Country).Select(z => z.CountryName).FirstOrDefault(),
                                         Permanent_State = db.tblStates.Where(z => z.StateId == empPersonalInfo.Permanent_State).Select(z => z.StateName).FirstOrDefault(),
                                         Permanent_City = empPersonalInfo.Permanent_City,
                                         Permanent_PostalCode = empPersonalInfo.Permanent_PostalCode,
                                         MaritalStatus = empPersonalInfo.MaritalStatus,
                                         MarriageAnniversaryDate = empPersonalInfo.MarriageAnniversaryDate,
                                         BirthDate = empPersonalInfo.BirthDate,
                                         Gender = empPersonalInfo.Gender,
                                         DrivingLicenceNumber = empPersonalInfo.DrivingLicenceNumber,
                                         PassportNumber = empPersonalInfo.PassportNumber,
                                         PassportExpiryDate = empPersonalInfo.PassportExpiryDate,
                                         AdharNumber = empPersonalInfo.AdharNumber,
                                         PANCardNumber = empPersonalInfo.PANCardNumber,
                                         PersonalEmailId = empPersonalInfo.PersonalEmailId,
                                         PersonalMobile = empPersonalInfo.PersonalMobile,
                                         NomineeMobile = empPersonalInfo.NomineeMobile,
                                         CompanyEmailId = empPersonalInfo.CompanyEmailId,
                                         CompanyMobile = empPersonalInfo.CompanyMobile,
                                         BloodGroup = empPersonalInfo.BloodGroup,
                                         CompanyBankAccount = payRoll.CompanyBankAccount,
                                         CompanyBankName = payRoll.CompanyBankName,
                                         JoiningDate = payRoll.JoiningDate

                                     }).SingleOrDefault();

                employee_data.BloodGroup = employee_data.BloodGroup == "0" ? "" : employee_data.BloodGroup;
                //var bloodGroupId = short.Parse(employee_data.BloodGroup);
                //var bloodGroupName = db.tblBloodGroups.Where(bg => bg.BloodGroupId == bloodGroupId).Select(bg => bg.BloodGroupName).SingleOrDefault();
                //employee_data.BloodGroup = bloodGroupName;

                DateTime temp_Birthdate = (DateTime)employee_data.BirthDate;
                var gender = "";
                if (employee_data.Gender.ToLower() == "m") { gender = "Male"; }
                if (employee_data.Gender.ToLower() == "f") { gender = "Female"; }

                String fileName_temp;
                if (!string.IsNullOrEmpty(employee_data.ProfilePhoto)) { fileName_temp = filename + employee_data.ProfilePhoto; } else { fileName_temp = filename + "default_image.jpg"; }
                if (File.Exists(fileName_temp))
                {
                    fileName_temp = filename + employee_data.ProfilePhoto;
                }
                else
                {
                    fileName_temp = filename + "default_image.jpg";
                }

                String GuardianFirstName, GuardianMiddleName, GuardianLastName;
                if (employee_data.GuardianFirstName != null && employee_data.GuardianFirstName != "") { GuardianFirstName = employee_data.GuardianFirstName; } else { GuardianFirstName = "-"; }
                if (employee_data.GuardianMiddleName != null && employee_data.GuardianMiddleName != "") { GuardianMiddleName = employee_data.GuardianMiddleName; } else { GuardianMiddleName = "-"; }
                if (employee_data.GuardianLastName != null && employee_data.GuardianLastName != "") { GuardianLastName = employee_data.GuardianLastName; } else { GuardianLastName = "-"; }

                string CompanyEmailId, CompanyMobile, PersonalEmailId, PersonalMobile, MaritalStatus, PANCardNumber, DrivingLicenceNumber, PassportNumber, AdharNumber, BloodGroup, NomineeMobile, bankAccount;
                if (employee_data.CompanyEmailId != null && employee_data.CompanyEmailId != "") { CompanyEmailId = employee_data.CompanyEmailId; } else { CompanyEmailId = "Not Available"; }
                if (employee_data.CompanyMobile != null && employee_data.CompanyMobile != "") { CompanyMobile = employee_data.CompanyMobile; } else { CompanyMobile = "Not Available"; }
                if (employee_data.PersonalEmailId != null && employee_data.PersonalEmailId != "") { PersonalEmailId = employee_data.PersonalEmailId; } else { PersonalEmailId = "Not Available"; }
                if (employee_data.PersonalMobile != null && employee_data.PersonalMobile != "") { PersonalMobile = employee_data.PersonalMobile; } else { PersonalMobile = "Not Available"; }
                if (employee_data.MaritalStatus != null && employee_data.MaritalStatus != "") { MaritalStatus = employee_data.MaritalStatus; } else { MaritalStatus = "Not Available"; }
                if (employee_data.PANCardNumber != null && employee_data.PANCardNumber != "") { PANCardNumber = employee_data.PANCardNumber; } else { PANCardNumber = "Not Available"; }
                if (employee_data.DrivingLicenceNumber != null && employee_data.DrivingLicenceNumber != "") { DrivingLicenceNumber = employee_data.DrivingLicenceNumber; } else { DrivingLicenceNumber = "Not Available"; }
                if (employee_data.PassportNumber != null && employee_data.PassportNumber != "") { PassportNumber = employee_data.PassportNumber; } else { PassportNumber = "Not Available"; }
                if (employee_data.AdharNumber != null && employee_data.AdharNumber != "") { AdharNumber = employee_data.AdharNumber; } else { AdharNumber = "Not Available"; }
                if (employee_data.BloodGroup != null && employee_data.BloodGroup != "") { BloodGroup = employee_data.BloodGroup; } else { BloodGroup = "Not Available"; }
                if (employee_data.NomineeMobile != null && employee_data.NomineeMobile != "") { NomineeMobile = employee_data.NomineeMobile; } else { NomineeMobile = "Not Available"; }
                if (employee_data.CompanyBankAccount != null && employee_data.CompanyBankAccount != "") { bankAccount = employee_data.CompanyBankAccount; } else { bankAccount = "Not Available"; }


                //String table = "<html>";
                //table += "<body style='margin-left: 155px; margin-right: 155px;'>";
                //table += "<div style='text-align: center;'>";
                //table += "<b><h1><div style='color: #0B0B61;font-size: 18px;'> Employee Information  </div></h1></b> </div><br/>";
                //table += "<br/><table><tr><td><div  style=' font-size: 9px; padding-left: 15px;'><img src='" + fileName_temp + "'  width='120' align='bottom' >";
                //table += "</div></td><td><b style=' color: darkgray;font-size: 11px; text-align:Left'  align='top' > Personal Information :</b><br/><div  style=' font-size: 9px; padding-left: 15px;'><strong>Register Code:</strong> " + employee_data.EmployeeRegisterCode + "<br/> <strong>Emploee Name:</strong> " + employee_data.CandidateFirstName + " " + employee_data.CandidateMiddleName + " " + employee_data.CandidateLastName + "<br/> <strong> Guardian Name: </strong> " + GuardianFirstName + " " + GuardianMiddleName + " " + GuardianLastName + "<br/> <strong> Date of Birth:</strong> " + String.Format("{0:dd-MMM-yyy}", temp_Birthdate) + "<br/> <strong>Gender:</strong> " + gender;
                //table += "</div></td></tr></table>";
                //table += "<br/><table><tr><td><b style=' color: darkgray;font-size: 11px; text-align:Left'>Present Address:</b><br/><div  style=' font-size: 9px; padding-left: 15px;'>" + employee_data.Present_HouseNo + "," + employee_data.Present_Location + "<br/>" + employee_data.Present_Area + ",<br/>" + employee_data.Present_City + "-" + employee_data.Present_PostalCode + ",<br/>" + employee_data.Present_State + "," + employee_data.Present_Country + ".";
                //table += "</div></td><td><b style=' color: darkgray;font-size: 11px; text-align:Left'> Permanent Address:</b><br/><div  style=' font-size: 9px; padding-left: 15px;'>" + employee_data.Permanent_HouseNo + "," + employee_data.Permanent_Location + "<br/>" + employee_data.Permanent_Area + ",<br/>" + employee_data.Permanent_City + "-" + employee_data.Permanent_PostalCode + ",<br/>" + employee_data.Permanent_State + "," + employee_data.Permanent_Country + ".";
                //table += "</div></td></tr></table>";
                //table += "<br/><h2 style='text-align:Left ;'><b style=' color: darkgray;font-size:  11px;'>Other Information:</b></h2>";
                //table += "<table style='width: 100%;' cellpadding='0' cellspacing='0'>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Company Email:</strong>" + CompanyEmailId + "</td><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Company Mobile:</strong> " + CompanyMobile + "</td></tr>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Personal Email:</strong> " + PersonalEmailId + "</td><td style='font-size: 9px; vertical-align: top; padding-left: 15px;'><strong>Personal Mobile: </strong>" + PersonalMobile + "</td></tr>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Marital Status:</strong> " + MaritalStatus + "@" + employee_data.MarriageAnniversaryDate + "</td><td style='font-size: 9px; vertical-align: top; padding-left: 15px;'><strong>PAN Number:</strong> " + PANCardNumber + "</td></tr>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Driving Licence Number:</strong>" + DrivingLicenceNumber + "</td><td style='font-size: 9px; vertical-align: top; padding-left: 15px;'><strong>Adhar Card Number:</strong> " + AdharNumber + "</td></tr>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Passport Number:</strong>" + PassportNumber + "</td><td style='font-size: 9px; vertical-align: top; padding-left: 15px;'><strong>Blood Group:</strong> " + BloodGroup + "</td></tr>";
                //table += "<tr style='border-spacing:1em;'><td style='font-size: 9px; vertical-align: top;padding-left: 15px;'><strong>Bank Account :</strong>" + bankAccount + "</td><td style='font-size: 9px; vertical-align: top; padding-left: 15px;'><strong>Nominee Mobile Number: </strong>" + NomineeMobile + "</td></tr>";
                //table += "</table>";
                //table += "</body>";
                //table += "</html>";


                string html = string.Empty;

                using (var sr = new StreamReader(context.Server.MapPath(ConfigurationManager.AppSettings["ViewEmployeeProfile"].ToString())))
                {
                    var temp = sr;
                    html = sr.ReadToEnd();
                    html = html.Replace("##fileName_temp##", fileName_temp);
                    html = html.Replace("##registerCopde##", employee_data.EmployeeRegisterCode);
                    html = html.Replace("##candidateName##", employee_data.CandidateFirstName + " " + employee_data.CandidateMiddleName + " " + employee_data.CandidateLastName );
                    html = html.Replace("##GuardianName##", GuardianFirstName + " " + GuardianMiddleName + " " + GuardianLastName);
                    html = html.Replace("##bod##", String.Format("{0:dd-MMM-yyy}", temp_Birthdate));
                    html = html.Replace("##gender##", gender);

                    html = html.Replace("##Present_HouseNo##", employee_data.Present_HouseNo);
                    html = html.Replace("##Present_Location##", employee_data.Present_Location);
                    html = html.Replace("##Present_Area##", employee_data.Present_Area);
                    html = html.Replace("##Present_City##", employee_data.Present_City + "-" + employee_data.Present_PostalCode);
                    html = html.Replace("##Present_State##", employee_data.Present_State + "," + employee_data.Present_Country);

                    html = html.Replace("##Permanent_HouseNo##", employee_data.Permanent_HouseNo);
                    html = html.Replace("##Permanent_Location##", employee_data.Permanent_Location);
                    html = html.Replace("##Permanent_Area##", employee_data.Permanent_Area);
                    html = html.Replace("##Permanent_City##", employee_data.Permanent_City + "-" + employee_data.Permanent_PostalCode);
                    html = html.Replace("##Permanent_State##", employee_data.Permanent_State + "," + employee_data.Permanent_Country);

                    html = html.Replace("##CompanyEmailId##", CompanyEmailId);
                    html = html.Replace("##CompanyMobile##", CompanyMobile);
                    html = html.Replace("##PersonalEmailId##", PersonalEmailId);
                    html = html.Replace("##PersonalMobile##", PersonalMobile);
                    html = html.Replace("##MaritalStatus##", MaritalStatus + "@" + employee_data.MarriageAnniversaryDate);
                    html = html.Replace("##PANCardNumber##", PANCardNumber);

                    html = html.Replace("##DrivingLicenceNumber##", DrivingLicenceNumber);
                    html = html.Replace("##AdharNumber##", AdharNumber);
                    html = html.Replace("##PassportNumber##", PassportNumber);
                    html = html.Replace("##BloodGroup##", BloodGroup);

                    html = html.Replace("##bankAccount##", bankAccount);
                    html = html.Replace("##NomineeMobile##", NomineeMobile);
                }

                String fileName = string.Format("{0}{1}{2}", "Employee_Profile", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), ".pdf");
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 30f, 30f, 30f, 30);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);

                pdfDoc.Open();

                PdfContentByte cb = writer.DirectContent;
                cb.SetLineWidth(2.0f); // Make a bit thicker than 1.0 default
                cb.MoveTo(20, pdfDoc.Top - 35f);
                cb.LineTo(580, pdfDoc.Top - 35f);
                cb.Stroke();

                iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(pdfDoc);
                hw.Parse(new StringReader(html));
                pdfDoc.Close();
            }
            catch (Exception ex)
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