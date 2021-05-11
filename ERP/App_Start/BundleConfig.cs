using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ERP
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/Content/app")
                .Include("~/Content/js/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css")
                .Include("~/Content/css/font-icons/entypo/css/animation.css")
                .Include("~/Content/css/ng-table.min.css")
                .Include("~/Content/css/ng-tags-input.css")
                .Include("~/Content/css/ng-tags-input.bootstrap.css")
                .Include("~/Content/css/toastr.css")
                .Include("~/Content/css/colorpicker.css")
                .Include("~/Content/css/uploadify.css")
                .Include("~/Content/js/daterangepicker/daterangepicker-bs3.css")
                .Include("~/Content/css/ui.daterangepicker.css")
                .Include("~/Content/css/animate.css")
                .Include("~/Content/css/neon.css")
                .Include("~/Content/css/drc-fc.css")
                .Include("~/Content/css/timeline.css")
                .Include("~/Content/css/custom.css")
                .Include("~/Content/css/BootstrapXL.css")
                .Include("~/Content/css/ng-ckeditor.cs")
                );

            bundles.Add(new StyleBundle("~/Content/PrintInvoice")
                .Include("~/Content/css/printStyle.css")
            );

            bundles.Add(new ScriptBundle("~/Scripts/app")
                .Include("~/Content/js/jquery-1.10.2.min.js")
                .Include("~/Content/js/jquery-ui/js/jquery-ui.js")
                .Include("~/Content/js/gsap/main-gsap.js")
                .Include("~/Content/js/jquery.transit.min.js")
                .Include("~/Content/js/jquery.inputmask.bundle.min.js")
                .Include("~/Content/js/bootstrap.min.js")
                .Include("~/Content/js/resizeable.js")
                .Include("~/Content/js/toastr.js")
                .Include("~/Content/js/bootbox/bootbox.min.js")
                .Include("~/Content/js/daterangepicker/moment.min.js")
                .Include("~/Content/js/daterangepicker/daterangepicker.js")
                .Include("~/Content/js/jquery.nicescroll.min.js")
                .Include("~/Content/js/neon-api.js")
                .Include("~/Content/js/neon-chat.js")
                .Include("~/Content/js/neon-custom.js")
                .Include("~/Content/js/neon-demo.js")
                .Include("~/Content/js/jquery.elastic.source.js")
                .Include("~/Scripts/ckeditor/ckeditor.js")
            );


            bundles.Add(new ScriptBundle("~/Scripts/angularfiles")
                .Include("~/Scripts/libs/underscore/underscore.js")
                .Include("~/Scripts/libs/angular/angular.js")
                .Include("~/Scripts/libs/angular/angular-route.js")
                .Include("~/Scripts/libs/angular/angular-cookie.js")
                .Include("~/Scripts/libs/angular/angular-sanitize.js")
                .Include("~/Scripts/libs/angular/angular-scroll.js")
                .Include("~/Scripts/modules/ng-table.src.js")
                .Include("~/Scripts/modules/ng-tags-input.js")
                .Include("~/Scripts/modules/ui-bootstrap.js")
                .Include("~/Scripts/modules/colorpicker.js")
                .Include("~/Scripts/modules/calendar.js")
                .Include("~/Scripts/modules/daterangepicker.js")
                .Include("~/Scripts/modules/date-parser.js")
                .Include("~/Scripts/modules/datepicker.js")
                .Include("~/Scripts/libs/angular/ng-file-upload-shim.js")
                .Include("~/Scripts/libs/angular/ng-file-upload.js")
                .Include("~/Scripts/modules/app.js")
                .Include("~/Scripts/directives/drcTooltip.js")
                .Include("~/Scripts/modules/textAngular-sanitize.js")
                .Include("~/Scripts/modules/textAngularSetup.js")
                .Include("~/Scripts/modules/textAngular.js")
                .Include("~/Scripts/filters/TimeAgoFilter.js")
                .Include("~/Scripts/filters/DecimalToTime.js")
                .Include("~/Scripts/filters/RemoveHtmlTags.js")
                .Include("~/Scripts/directives/drcScrollTop.js")
                .Include("~/Scripts/modules/ng-ckeditor.js")
                );


            ////ABGROUP
            //bundles.Add(new ScriptBundle("~/Scripts/abgroup")
            //    .Include("~/Scripts/controllers/GroupCtrl.js")
            //    .Include("~/Scripts/services/GroupService.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    );

            ////ABCONTACT
            //bundles.Add(new ScriptBundle("~/Scripts/abcontact")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/controllers/ABContactCtrl.js")
            //    .Include("~/Scripts/services/ABContactService.js")
            //    .Include("~/Scripts/directives/checkbox.js")
            //    .Include("~/Scripts/directives/drcMultiSelect.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    );

            ////ABPRINTADDRESS
            //bundles.Add(new ScriptBundle("~/Scripts/abprintaddress")
            //    .Include("~/Scripts/controllers/ABPrintAddressCtrl.js")
            //    .Include("~/Scripts/services/ABPrintService.js")
            //    .Include("~/Scripts/directives/abTemplateBuilder.js")
            //    );

            ////ABTEMPLATES
            //bundles.Add(new ScriptBundle("~/Scripts/abtemplates")
            //    .Include("~/Scripts/controllers/ABTemplateCtrl.js")
            //    .Include("~/Scripts/services/ABTemplateService.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    );

            //ABGROUP
            bundles.Add(new ScriptBundle("~/Area/AddressBook/Scripts/abgroup")
                .Include("~/Areas/AB/Scripts/controllers/ABGroupCtrl.js")
                .Include("~/Areas/AB/Scripts/services/ABGroupService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/confirmbox.js")
                );

            //ABCONTACT
            bundles.Add(new ScriptBundle("~/Area/AddressBook/Scripts/abcontact")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Areas/AB/Scripts/controllers/ABContactCtrl.js")
                .Include("~/Areas/AB/Scripts/services/ABContactService.js")
                .Include("~/Scripts/directives/checkbox.js")
                .Include("~/Scripts/directives/drcMultiSelect.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/uploadFile.js")
                );

            //ABPRINTADDRESS
            bundles.Add(new ScriptBundle("~/Area/AddressBook/Scripts/abprintaddress")
                .Include("~/Areas/AB/Scripts/controllers/ABPrintAddressCtrl.js")
                .Include("~/Areas/AB/Scripts/services/ABPrintService.js")
                //.Include("~/Scripts/directives/abTemplateBuilder.js")
                );

            //ABTEMPLATES
            bundles.Add(new ScriptBundle("~/Area/AddressBook/Scripts/abtemplates")
                .Include("~/Areas/AB/Scripts/controllers/ABTemplateCtrl.js")
                .Include("~/Areas/AB/Scripts/services/ABTemplateService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/confirmbox.js")
                );


            //APPLYLEAVE
            bundles.Add(new ScriptBundle("~/Scripts/applyleave")
                .Include("~/Content/js/fullcalendar.js")
                .Include("~/Scripts/controllers/ApplyLeaveCtrl.js")
                .Include("~/Scripts/services/ApplyLeaveService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/drcPopover.js")
                );

            ////ARMODULES
            //bundles.Add(new ScriptBundle("~/Scripts/armodules")
            //    .Include("~/Scripts/controllers/ARModuleCtrl.js")
            //    .Include("~/Scripts/services/ARModuleService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    );

            ////ARSUBMODULES
            //bundles.Add(new ScriptBundle("~/Scripts/arsubmodules")
            //    .Include("~/Scripts/controllers/ARSubModuleCtrl.js")
            //    .Include("~/Scripts/services/ARSubModuleService.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    );



            //ARMODULES
            bundles.Add(new ScriptBundle("~/Area/AR/Scripts/armodules")
                .Include("~/Areas/AR/Scripts/controllers/ARModuleCtrl.js")
                .Include("~/Areas/AR/Scripts/services/ARModuleService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                );

            //ARSUBMODULES
            bundles.Add(new ScriptBundle("~/Area/AR/Scripts/arsubmodules")
                .Include("~/Areas/AR/Scripts/controllers/ARSubModuleCtrl.js")
                .Include("~/Areas/AR/Scripts/services/ARSubModuleService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/confirmbox.js")
                );




            ////BRANDS
            //bundles.Add(new ScriptBundle("~/Scripts/brands")
            //    .Include("~/Scripts/controllers/BrandsCtrl.js")
            //    .Include("~/Scripts/services/BrandsService.js")
            //    .Include("~/Scripts/directives/checkbox.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    );

            //BRANDS AREAS IN INVENTORY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/brands")
                 .Include("~/Areas/Inventory/Scripts/controllers/BrandsCtrl.js")
                .Include("~/Areas/Inventory/Scripts/services/BrandsService.js")
                .Include("~/Scripts/directives/checkbox.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                );

            ////CAREGORY
            //bundles.Add(new ScriptBundle("~/Scripts/categorys")
            //      .Include("~/Scripts/controllers/CategoriesCtrl.js")
            //      .Include("~/Scripts/services/CategoryService.js")
            //      .Include("~/Scripts/directives/confirmbox.js")
            //      .Include("~/Scripts/directives/focus.js")
            //    );

            //CAREGORY AREAS IN INVENTORY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/categorys")
                  .Include("~/Areas/Inventory/Scripts/controllers/CategoriesCtrl.js")
                  .Include("~/Areas/Inventory/Scripts/services/CategoryService.js")
                  .Include("~/Scripts/directives/confirmbox.js")
                  .Include("~/Scripts/directives/focus.js")
                );

            ////DESHBOARD
            // bundles.Add(new ScriptBundle("~/Scripts/deshboard")
            //         .Include("~/Scripts/directives/angular-timer.js")
            //         .Include("~/Content/js/fullcalendar.js")
            //         .Include("~/Scripts/controllers/DashboardCtrl.js")
            //         .Include("~/Scripts/services/DashboardService.js")
            //         .Include("~/Scripts/services/ApplyLeaveService.js")
            //         .Include("~/Scripts/services/EmpDailyInOutService.js")
            //         .Include("~/Scripts/controllers/EmpDailyInOutCtrl.js")
            //     );




            //DESHBOARD chnage  service in area
            bundles.Add(new ScriptBundle("~/Scripts/deshboard")
                    .Include("~/Scripts/directives/angular-timer.js")
                    .Include("~/Content/js/fullcalendar.js")
                //.Include("~/Scripts/controllers/DashboardCtrl.js")
                    .Include("~/Scripts/services/DashboardService.js")
                    .Include("~/Areas/HR/Scripts/services/ApplyLeaveService.js")
                    .Include("~/Scripts/services/EmpDailyInOutService.js")
                    .Include("~/Scripts/controllers/EmpDailyInOutCtrl.js")
                    .Include("~/Areas/HR/Scripts/services/EmpAttendanceService.js")
                );



            //DESIGNATION MASTER
            bundles.Add(new ScriptBundle("~/Scripts/designationsMaster")
            .Include("~/Scripts/controllers/DesignationMasterCtrl.js")
                .Include("~/Scripts/controllers/DesignationCtrl.js")
                .Include("~/Scripts/services/DesignationService.js")
                .Include("~/Scripts/controllers/DesignationGroupCtrl.js")
                .Include("~/Scripts/services/DesignationGroupService.js")
                .Include("~/Scripts/controllers/DesignationParentCtrl.js")
                 .Include("~/Scripts/services/DesignationParentService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                );




            //DOCUMENTS
            bundles.Add(new ScriptBundle("~/Scripts/documents")
                .Include("~/Scripts/controllers/DocumentCtrl.js")
                .Include("~/Scripts/services/DocumentService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                );



            //CURRENCY
            bundles.Add(new ScriptBundle("~/Scripts/Currency")
                .Include("~/Scripts/controllers/CurrencyCtrl.js")
                .Include("~/Scripts/services/CurrencyService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                );


            //DAILY IN OUT
            bundles.Add(new ScriptBundle("~/Scripts/dailyInOut")
                 .Include("~/Scripts/controllers/EmpDailyInOutCtrl.js")
                 .Include("~/Scripts/services/EmpDailyInOutService.js")
                 .Include("~/Scripts/directives/focus.js")
                );

            ////EMPLOYEE CREATE
            //bundles.Add(new ScriptBundle("~/Scripts/employeeCreate")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/controllers/EmpMasterCtrl.js")
            //    .Include("~/Scripts/controllers/EmployeeCreateCtrl.js")
            //    .Include("~/Scripts/controllers/EmpQualificationCtrl.js")
            //    .Include("~/Scripts/controllers/EmpDocumentCtrl.js")
            //    .Include("~/Scripts/controllers/EmpWorkExperienceCtrl.js")
            //    .Include("~/Scripts/controllers/EmpCompanyInfoCtrl.js")
            //    .Include("~/Scripts/controllers/EmpRelativeInfoCtrl.js")
            //    .Include("~/Scripts/controllers/EmpPayRollCtrl.js")
            //    .Include("~/Scripts/controllers/EmpCredentialCtrl.js")
            //    .Include("~/Scripts/controllers/EmpLoginInfoCtrl.js")
            //    .Include("~/Scripts/services/EmployeeCreateService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    .Include("~/Scripts/directives/drcInputMask.js")
            //    .Include("~/Scripts/directives/slider.js")
            //    );

            ////EMPLOYEE LIST
            //bundles.Add(new ScriptBundle("~/Scripts/employeeList")
            //    .Include("~/Scripts/controllers/EmployeeListCtrl.js")
            //    .Include("~/Scripts/services/EmployeeCreateService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    );

            ////EMPLOYEE ORGANIZATION CHART
            //bundles.Add(new ScriptBundle("~/Scripts/employeeOrgChart")
            //    .Include("~/Content/js/jit/jit.js")
            //    .Include("~/Scripts/controllers/EmpOrgChartCtrl.js")
            //    );

            ////EMPLOYEE  VIEW PROFILE
            //bundles.Add(new ScriptBundle("~/Scripts/employeeViewProfile")
            //    .Include("~/Scripts/controllers/EmpViewProfileCtrl.js")
            //    .Include("~/Scripts/services/EmployeeCreateService.js")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    );


            //EMPLOYEE CREATE AREA IN HR
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/employeeCreate")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpMasterCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmployeeCreateCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpQualificationCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpDocumentCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpWorkExperienceCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpCompanyInfoCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpRelativeInfoCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpPayRollCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpCredentialCtrl.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpLoginInfoCtrl.js")
                .Include("~/Areas/HR/Scripts/services/EmployeeCreateService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/uploadFile.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Scripts/directives/slider.js")
                );

            //EMPLOYEE LIST AREA IN HR --
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/employeeList")
                .Include("~/Areas/HR/Scripts/controllers/EmployeeListCtrl.js")
                .Include("~/Areas/HR/Scripts/services/EmployeeCreateService.js")
                .Include("~/Scripts/directives/confirmbox.js")
                );

            //EMPLOYEE ORGANIZATION CHART AREA IN HR
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/employeeOrgChart")
                .Include("~/Content/js/jit/jit.js")
                .Include("~/Areas/HR/Scripts/controllers/EmpOrgChartCtrl.js")
                 );

            //EMPLOYEE  VIEW PROFILE AREA IN HR
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/employeeViewProfile")
                .Include("~/Areas/HR/Scripts/controllers/EmpViewProfileCtrl.js")
                .Include("~/Areas/HR/Scripts/services/EmployeeCreateService.js")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Scripts/directives/uploadFile.js")
                );


            //FESTIVAL MASTER
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/festivalMaster")
            .Include("~/Areas/HR/Scripts/controllers/FestivalMasterCtrl.js")
               .Include("~/Areas/HR/Scripts/controllers/FestivalCtrl.js")
               .Include("~/Areas/HR/Scripts/services/FestivalService.js")
               .Include("~/Areas/HR/Scripts/controllers/FestivalTypeCtrl.js")
                 .Include("~/Areas/HR/Scripts/services/FestivalTypeService.js")
               .Include("~/Scripts/directives/confirmbox.js")
               .Include("~/Scripts/directives/focus.js")
               );

            //HR: DAILY IN-OUT REPORT
            bundles.Add(new ScriptBundle("~/Area/HR/Scripts/empAttendanceReport")
               .Include("~/Areas/HR/Scripts/controllers/EmpAttendanceCtrl.js")
               .Include("~/Areas/HR/Scripts/services/EmpAttendanceService.js")
                );


            //HR: DAILY IN-OUT REPORT
            bundles.Add(new ScriptBundle("~/Areas/HR/Scripts/dailyInOutReport")
                //.Include("~/Areas/HR/Scripts/controllers/DailyInOutReportCtrl.js")
                //.Include("~/Areas/HR/Scripts/services/DailyInOutReportService.js")
               .Include("~/Scripts/directives/drcInputMask.js")
               .Include("~/Scripts/directives/confirmbox.js")
            );


            // //HRDDD IN PRESS MEDIA
            //bundles.Add(new ScriptBundle("~/Scripts/hrdDDInPressMedia")
            //   .Include("~/Scripts/controllers/HRDDDInPressMediaCtrl.js")
            //   .Include("~/Scripts/services/HRDDDInPressMediaService.js")
            //   .Include("~/Scripts/date/date.js")
            //   .Include("~/Scripts/directives/confirmbox.js")
            //   .Include("~/Scripts/directives/focus.js")
            //   .Include("~/Content/js/jquery.uploadify.js")
            //   .Include("~/Scripts/directives/uploadFile.js")
            //   .Include("~/Scripts/directives/drcInputMask.js")
            //   .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //   .Include("~/Scripts/directives/drcFilterDatePicker.js")

            //    );


            //HRD FINANCIAL DEATH EMPLOYEES
            // bundles.Add(new ScriptBundle("~/Scripts/hrdFinancialDeathEmp")
            //    .Include("~/Scripts/date/date.js")
            //    .Include("~/Scripts/controllers/HRDFinancialDeathEmpCtrl.js")
            //    .Include("~/Scripts/services/HRDFinancialDeathEmpService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/drcInputMask.js")
            //    .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //    .Include("~/Scripts/directives/drcFilterDatePicker.js")

            //     );

            ////HRDMEDIAL HELP
            // bundles.Add(new ScriptBundle("~/Scripts/hrdMedicalHelp")
            //    .Include("~/Scripts/date/date.js")
            //    .Include("~/Scripts/controllers/HRDMedicalHelpCtrl.js")
            //    .Include("~/Scripts/services/HRDMedicalHelpService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    .Include("~/Scripts/directives/drcInputMask.js")
            //    .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //    .Include("~/Scripts/directives/drcFilterDatePicker.js")


            //     );


            ////HRDPRESS MEDIA
            // bundles.Add(new ScriptBundle("~/Scripts/hrdPressMedia")
            //     .Include("~/Scripts/date/date.js")
            //    .Include("~/Scripts/controllers/HRDPressMediaExpCtrl.js")
            //    .Include("~/Scripts/services/HRDPressMediaExpService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    .Include("~/Scripts/directives/drcInputMask.js")
            //    .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //    .Include("~/Scripts/directives/drcFilterDatePicker.js")

            //     );

            ////HRD QUARTERLY MEETING MANAGEMENT
            // bundles.Add(new ScriptBundle("~/Scripts/hrdQuarterlyMeetingManagement")
            //        .Include("~/Scripts/date/date.js")
            //        .Include("~/Scripts/controllers/HRDQuarterlyManagementMeetingCtrl.js")
            //        .Include("~/Scripts/services/HRDQuarterlyManagementMeetingService.js")
            //        .Include("~/Scripts/directives/checkbox.js")
            //        .Include("~/Scripts/directives/confirmbox.js")
            //        .Include("~/Scripts/directives/focus.js")
            //        .Include("~/Content/js/jquery.uploadify.js")
            //        .Include("~/Scripts/directives/uploadFile.js")
            //        .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //        .Include("~/Scripts/directives/drcFilterDatePicker.js")


            //     );

            ////HRD SOCIAL WELFARE EXPENSE
            // bundles.Add(new ScriptBundle("~/Scripts/hrdSocialWelfareExpense")
            //        .Include("~/Scripts/controllers/HRDSocialWelExpCtrl.js")
            //        .Include("~/Scripts/services/HRDSocialWelExpService.js")
            //        .Include("~/Scripts/date/date.js")
            //        .Include("~/Scripts/directives/confirmbox.js")
            //        .Include("~/Scripts/directives/focus.js")
            //        .Include("~/Content/js/jquery.uploadify.js")
            //        .Include("~/Scripts/directives/uploadFile.js")
            //        .Include("~/Scripts/directives/drcInputMask.js")
            //        .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //        .Include("~/Scripts/directives/drcFilterDatePicker.js")

            //        );


            ////HRD TRAINING AND MEETING
            // bundles.Add(new ScriptBundle("~/Scripts/hrdTrainingAndMeeting")
            //        .Include("~/Scripts/controllers/HRDTrainingsAndMeetingCtrl.js")
            //        .Include("~/Scripts/services/HRDTrainingAndMeetingService.js")
            //        .Include("~/Scripts/date/date.js")
            //        .Include("~/Scripts/directives/confirmbox.js")
            //        .Include("~/Scripts/directives/focus.js")
            //        .Include("~/Content/js/jquery.uploadify.js")
            //        .Include("~/Scripts/directives/uploadFile.js")
            //        .Include("~/Scripts/directives/drcInputMask.js")
            //        .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //        .Include("~/Scripts/directives/drcFilterDatePicker.js")

            //        );

            ////INVENTORY CREATE 
            //bundles.Add(new ScriptBundle("~/Scripts/inventoryCreate")
            //        .Include("~/Scripts/controllers/InventoriesCreateCtrl.js")
            //       .Include("~/Scripts/services/InventoriesService.js")
            //       .Include("~/Scripts/directives/checkbox.js")
            //       .Include("~/Scripts/directives/confirmbox.js")
            //       .Include("~/Scripts/directives/focus.js")
            //       .Include("~/Scripts/directives/drcInputMask.js")
            //       );

            ////INVENTORY LIST
            //bundles.Add(new ScriptBundle("~/Scripts/inventoryList")
            //    .Include("~/Scripts/controllers/InventoriesListCtrl.js")
            //    .Include("~/Scripts/services/InventoriesService.js")
            //    .Include("~/Scripts/directives/checkbox.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    );






            //INVENTORY CREATE  AREA IN  INVENTORY 
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/inventoryCreate")
                   .Include("~/Areas/Inventory/Scripts/controllers/InventoriesCreateCtrl.js")
                   .Include("~/Areas/Inventory/Scripts/services/InventoriesService.js")
                   .Include("~/Scripts/directives/checkbox.js")
                   .Include("~/Scripts/directives/confirmbox.js")
                   .Include("~/Scripts/directives/focus.js")
                   .Include("~/Scripts/directives/drcInputMask.js")
                   );

            //INVENTORY LIST AREA IN INVENTORY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/inventoryList")
                .Include("~/Areas/Inventory/Scripts/controllers/InventoriesListCtrl.js")
                .Include("~/Areas/Inventory/Scripts/services/InventoriesService.js")
                .Include("~/Scripts/directives/checkbox.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Scripts/directives/uploadFile.js")
                );

            //// ISSUED HRD DOCUMENTS
            // bundles.Add(new ScriptBundle("~/Scripts/issuedHRDDocuments")
            //    .Include("~/Content/js/jquery.uploadify.js")
            //    .Include("~/Scripts/date/date.js")
            //    .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
            //    .Include("~/Scripts/controllers/IssuedDocumentCtrl.js")
            //    .Include("~/Scripts/services/IssuedDocumentService.js")
            //    .Include("~/Scripts/directives/confirmbox.js")
            //    .Include("~/Scripts/directives/focus.js")
            //    .Include("~/Scripts/directives/uploadFile.js")
            //    .Include("~/Scripts/directives/drcFilterDatePicker.js")
            //    .Include("~/Scripts/directives/drcInputMask.js")
            //     );

            // ISSUED HRD DOCUMENTS BASED ON AREA
            bundles.Add(new ScriptBundle("~/Scripts/issuedHRDDocuments")
               .Include("~/Content/js/jquery.uploadify.js")
               .Include("~/Scripts/date/date.js")
               .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
               .Include("~/Areas/HRD/Scripts/controllers/IssuedDocumentCtrl.js")
               .Include("~/Areas/HRD/Scripts/services/IssuedDocumentService.js")
               .Include("~/Scripts/directives/confirmbox.js")
               .Include("~/Scripts/directives/focus.js")
               .Include("~/Scripts/directives/uploadFile.js")
               .Include("~/Scripts/directives/drcFilterDatePicker.js")
               .Include("~/Scripts/directives/drcInputMask.js")
                );


            ////LOCATION
            //bundles.Add(new ScriptBundle("~/Scripts/location")
            //   .Include("~/Scripts/controllers/LocationCtrl.js")
            //   .Include("~/Scripts/services/LocationService.js")
            //   .Include("~/Scripts/directives/confirmbox.js")
            //   .Include("~/Scripts/directives/focus.js")
            //    );

            //LOCATION AREA IN INVENROTY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/location")
               .Include("~/Areas/Inventory/Scripts/controllers/LocationCtrl.js")
               .Include("~/Areas/Inventory/Scripts/services/LocationService.js")
               .Include("~/Scripts/directives/confirmbox.js")
               .Include("~/Scripts/directives/focus.js")
                );

            //NOTIFICATION
            bundles.Add(new ScriptBundle("~/Scripts/notification")
                    .Include("~/Scripts/libs/signalr/jquery.signalR-2.0.3.min.js")
                    .Include("~/Scripts/notification/manage-notification.js")
                );



            // PMS  MODULE
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/PMSModule")
                .Include("~/Areas/PMS/Scripts/controllers/PMSModuleCtrl.js")
               .Include("~/Areas/PMS/Scripts/services/PMSModuleService.js")
               .Include("~/Scripts/directives/confirmbox.js")
               .Include("~/Scripts/directives/focus.js")
               .Include("~/Scripts/directives/drcModuleHover.js")
               .Include("~/Scripts/directives/drcDragToSort.js")
               .Include("~/Scripts/directives/drcDragToSortForTodo.js")
               .Include("~/Scripts/directives/drcPopoverAssignTodo.js")
               .Include("~/Scripts/directives/focusNext.js")
                );

            //PMS MODULE TODO
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/PMSModuleTodo")
               .Include("~/Areas/PMS/Scripts/controllers/PMSModuleCtrl.js")
               .Include("~/Areas/PMS/Scripts/services/PMSModuleService.js")
               .Include("~/Scripts/directives/confirmbox.js")
               .Include("~/Scripts/directives/focus.js")
               .Include("~/Scripts/directives/drcModuleHover.js")
               .Include("~/Scripts/directives/drcPopoverAssignTodo.js")
               .Include("~/Scripts/directives/focusNext.js")
                );

            //PMS PROJECT 
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/pmsProject")
               .Include("~/Areas/PMS/Scripts/controllers/PMSProjectCtrl.js")
               .Include("~/Areas/PMS/Scripts/services/PMSProjectService.js")
               .Include("~/Scripts/directives/focus.js")
               .Include("~/Scripts/directives/drcMultiSelect.js")
               .Include("~/Scripts/directives/drcInputMask.js")
                );


            //PMS PROJECT PROGRESS
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/pmsProjectProgress")
                       .Include("~/Areas/PMS/Scripts/controllers/PMSModuleCtrl.js")
                       .Include("~/Areas/PMS/Scripts/services/PMSModuleService.js")
                );

            //PMS REPORT
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/pmsProjectReport")
                   .Include("~/Scripts/date/date.js")
                   .Include("~/Areas/PMS/Scripts/controllers/PMSReportCtrl.js")
                   .Include("~/Areas/PMS/Scripts/services/PMSReportService.js")
                   .Include("~/Scripts/directives/drcInputMask.js")
                   .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
                   .Include("~/Scripts/directives/drcFilterDatePicker.js")
                    );

            //PMS TODO COMMENTS
            bundles.Add(new ScriptBundle("~/Areas/PMS/Scripts/pmsProjectTodoComment")
                   .Include("~/Content/js/jquery.uploadify.js")
                   .Include("~/Areas/PMS/Scripts/controllers/PMSTodoCommentCtrl.js")
                   .Include("~/Areas/PMS/Scripts/services/PMSTodoCommentService.js")
                   .Include("~/Scripts/directives/uploadMultiFile.js")
                   .Include("~/Scripts/directives/slider.js")
                   .Include("~/Scripts/directives/confirmbox.js")
                   );


            //ROLE
            bundles.Add(new ScriptBundle("~/Scripts/roles")
                   .Include("~/Scripts/controllers/RoleCtrl.js")
                   .Include("~/Scripts/services/RoleService.js")
                   .Include("~/Scripts/directives/confirmbox.js")
                   .Include("~/Scripts/directives/focus.js")
                );

            ////SUB CATEGORY
            //bundles.Add(new ScriptBundle("~/Scripts/subCategories")
            //        .Include("~/Scripts/controllers/SubCategoriesCtrl.js")
            //       .Include("~/Scripts/services/SubCategoriesService.js")
            //       .Include("~/Scripts/directives/checkbox.js")
            //       .Include("~/Scripts/directives/confirmbox.js")
            //       .Include("~/Scripts/directives/focus.js")
            //    );

            //SUB CATEGORY AREAS IN INVENTORY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/subCategories")
                 .Include("~/Areas/Inventory/Scripts/controllers/SubCategoriesCtrl.js")
                 .Include("~/Areas/Inventory/Scripts/services/SubCategoriesService.js")
                 .Include("~/Scripts/directives/checkbox.js")
                 .Include("~/Scripts/directives/confirmbox.js")
                 .Include("~/Scripts/directives/focus.js")
             );




            //TECHNOLOGY MASTER
            bundles.Add(new ScriptBundle("~/Scripts/technologyMaster")
            .Include("~/Scripts/controllers/TechnologyMasterCtrl.js")
                   .Include("~/Scripts/controllers/TechnologyGroupCtrl.js")
                   .Include("~/Scripts/services/TechnologyGroupService.js")
                   .Include("~/Scripts/controllers/TechnologyCtrl.js")
                   .Include("~/Scripts/services/TechnologyService.js")
                   .Include("~/Scripts/directives/confirmbox.js")
                   .Include("~/Scripts/directives/focus.js")

                );


            ////VENDORS
            //bundles.Add(new ScriptBundle("~/Scripts/vendors")
            //    .Include("~/Scripts/controllers/VendorsCtrl.js")
            //   .Include("~/Scripts/services/VendorsService.js")
            //   .Include("~/Scripts/directives/checkbox.js")
            //   .Include("~/Scripts/directives/confirmbox.js")
            //   .Include("~/Scripts/directives/focus.js")
            //    );

            //VENDORS AREAS IN INVENTORY
            bundles.Add(new ScriptBundle("~/Area/Inventory/Scripts/vendors")
                   .Include("~/Areas/Inventory/Scripts/controllers/VendorsCtrl.js")
                   .Include("~/Areas/Inventory/Scripts/services/VendorsService.js")
                   .Include("~/Scripts/directives/checkbox.js")
                   .Include("~/Scripts/directives/confirmbox.js")
                   .Include("~/Scripts/directives/focus.js")
                );

            //INVOICE CLIENT CREATE
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/invoiceClientMaster")
                .Include("~/Areas/Invoice/Scripts/controllers/ClientCreateMasterCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/ClientCreatePartialCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/ClientCreateFullCtrl.js")
                .Include("~/Areas/Invoice/Scripts/services/ClientCreateService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/drcOmitSubFormValidation.js")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Scripts/directives/uploadFile.js")
               );

            //INVOICE CLIENT OVERVIEW
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/invoiceClientOverview")
                .Include("~/Areas/Invoice/Scripts/controllers/ClientOverViewCtrl.js")
                .Include("~/Areas/Invoice/Scripts/services/ClientCreateService.js")
                );

            //INVOICE CLIENT CONVERSATION MASTER
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/Conversation")
                .Include("~/Areas/Invoice/Scripts/controllers/ConversationMasterCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/ConversationCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/InquiryConversationCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/ProjectConversationCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/MilestoneCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/InvoiceCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/PaymentReceivedCtrl.js")


                .Include("~/Areas/Invoice/Scripts/services/ConversationService.js")
                .Include("~/Areas/Invoice/Scripts/services/InquiryService.js")
                .Include("~/Areas/Invoice/Scripts/services/ProposalService.js")
                .Include("~/Areas/Invoice/Scripts/services/ProjectService.js")
                .Include("~/Areas/Invoice/Scripts/services/MilestoneService.js")
                .Include("~/Areas/Invoice/Scripts/services/InvoiceService.js")
                .Include("~/Areas/Invoice/Scripts/services/PaymentReceivedService.js")

                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Scripts/directives/uploadFile.js")
                .Include("~/Scripts/directives/uploadMultiFile.js")
                );

            //INVOICE PRINT
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/PrintInvoice")
                .Include("~/Areas/Invoice/Scripts/controllers/PrintInvoiceCtrl.js")
                .Include("~/Areas/Invoice/Scripts/services/InvoiceService.js")
                );
            //INVOICE CLIENT CONVERSATION
            //bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/Conversation")
            //.Include("~/Areas/Invoice/Scripts/controllers/ConversationCtrl.js")
            //.Include("~/Areas/Invoice/Scripts/services/ConversationService.js")
            //.Include("~/Scripts/directives/focus.js")
            //.Include("~/Scripts/directives/drcInputMask.js")
            //.Include("~/Scripts/directives/confirmbox.js")
            //.Include("~/Content/js/jquery.uploadify.js")
            //.Include("~/Scripts/directives/uploadFile.js")
            //.Include("~/Scripts/directives/uploadMultiFile.js")
            //);

            //KYC CLIENT CREATE
            bundles.Add(new ScriptBundle("~/Scripts/kycClientMaster")
                .Include("~/Scripts/controllers/KYCClientCreateCtrl.js")
                .Include("~/Scripts/services/KYCClientCreateService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Scripts/directives/drcOmitSubFormValidation.js")
                .Include("~/Content/js/jquery.uploadify.js")
                .Include("~/Scripts/directives/uploadFile.js")
               );

            //INQUIRY
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/Inquiry")
                .Include("~/Areas/Invoice/Scripts/controllers/InquiryCtrl.js")
                .Include("~/Areas/Invoice/Scripts/services/InquiryService.js")
                .Include("~/Areas/Invoice/Scripts/services/ProposalService.js")
                .Include("~/Scripts/directives/focus.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Scripts/directives/confirmbox.js")
                .Include("~/Content/js/jquery.uploadify.js")
                //.Include("~/Scripts/directives/uploadFile.js")
                .Include("~/Scripts/directives/uploadMultiFile.js")
                );

            //INVOICE REPORT
            bundles.Add(new ScriptBundle("~/Area/Invoice/Scripts/Report")
                //.Include("~/Areas/Invoice/Scripts/controllers/ReportMasterCtrl.js")
                .Include("~/Areas/Invoice/Scripts/controllers/ReportInvoiceCtrl.js")

                //.Include("~/Areas/Invoice/Scripts/services/ReportMasterService.js")
                .Include("~/Areas/Invoice/Scripts/services/ReportInvoiceService.js")
                .Include("~/Scripts/date/date.js")
                .Include("~/Scripts/directives/drcInputMask.js")
                .Include("~/Content/js/daterangepicker/daterangepicker.jQuery.js")
                .Include("~/Scripts/directives/drcFilterDatePicker.js")

                );
        }
    }
}