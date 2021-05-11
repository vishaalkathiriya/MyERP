/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("MedicalHelpCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "MedicalHelpService", "ngTableParams", "$q", "$modal",
            medicalHelpCtrl
        ]);

    //Main controller function
    function medicalHelpCtrl($scope, $rootScope, $timeout, $http, $filter, MedicalHelpService, ngTableParams, $q, $modal) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.Attachment = "";
        $scope.ischanged = false;

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };

        // BEGIN DATE PICKER
        $scope.dateOptions = { 'year-format': "'yy'", 'starting-day': 1 };
        $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.today = function () {
            $scope.currentDate = new Date();
        };
        $scope.today();
        $scope.showWeeks = true;
        $scope.toggleWeeks = function () {
            $scope.showWeeks = !$scope.showWeeks;
        };
        $scope.clear = function () {
            $scope.currentDate = null;

        };
        //   $scope.maxDate = $scope.maxDate || new Date();
        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenIssueDate = true;
        };

        $scope.$watch('editdata.IssueDate', function (newvalue) {

           $scope.issueDate = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 

      //  // var test = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 
      //  // $scope.issuedate = $scope.stringtodatestring(test);
        });

          $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };


        $scope.ValidateIssuedDate = function (issueDate) {
            if (!issueDate) {
                $scope.MedicalHelpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.IssueDate = $scope.StringToDateString(issueDate);
                    $scope.MedicalHelpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.MedicalHelpform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.MedicalHelpform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.IssueDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };
        // End DATE PICKER

        //BEGIN RESET FORM
        function ResetForm() {


            $scope.editData = {
                ECode: "", EmployeeName: "", PatientName: "", HospitalName: "", MobileNumber: "", Relation: "", ChequeNumber: "", IssueDate: "", ReceiverName: "", Amount: "", Attachment: ""
            };

            $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
           // $scope.issueDate = "";
            $scope.fileName = "";
            $scope.MedicalHelpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD MEDICAL FORM BUTTON
        $scope.AddMedicalHelp = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };
        //END  ADD MEDICAL FORM BUTTON

        $scope.FilterByIssueDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN CLOSE MEDICAL FORM
        $scope.CloseMedicalHelp = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE MEDICAL FORM

        //BEGIN RESET MEDICAL FORM
        $scope.ResetMedicalHelp = function () {
            if ($scope.mode == "Edit") {

                $scope.editData = {
                    ECode: $scope.storage.lastRecord.ECode,
                    EmployeeName: $scope.storage.lastRecord.EmployeeName,
                    PatientName: $scope.storage.lastRecord.PatientName,
                    HospitalName: $scope.storage.lastRecord.HospitalName,
                    MobileNumber: $scope.storage.lastRecord.MobileNumber,
                    Relation: $scope.storage.lastRecord.Relation,
                    ChequeNumber: $scope.storage.lastRecord.ChequeNumber,
                    //IssueDate : $scope.storage.lastRecord.ChequeIssueDate,
                    IssueDate: $filter('date')($scope.storage.lastRecord.ChequeIssueDate, 'dd-MM-yyyy'),
                    ReceiverName: $scope.storage.lastRecord.ReceiverName,
                    Amount: $scope.storage.lastRecord.Amount,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
              //  $scope.issueDate = $scope.storage.lastRecord.ChequeIssueDate;
                $scope.fileName = $scope.storage.lastRecord.Attachment;

                if ($scope.storage.lastRecord.IsPatelSocialGroup == true) {
                    $scope.editData.IsPatelSocialGroup = $scope.storage.lastRecord.IsPatelSocialGroup;
                    $scope.editData.QuotationAmount = $scope.storage.lastRecord.QuotationAmount;
                    $scope.editData.ApprovedBy = $scope.storage.lastRecord.ApprovedBy;
                }
            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET MEDICAL FORM

        //BEGIN SAVE MEDICAL INFORMATION
        $scope.CreateUpdateMedicalHelp = function (medHelp) {

            var IDate = $filter('date')(medHelp.IssueDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            medHelp.ChequeIssueDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            MedicalHelpService.CreateUpdateMedicalHelp(medHelp, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { ECode: "", EmployeeName: "", PatientName: "", HospitalName: "", MobileNumber: "", Relation: "", ChequeNumber: "", IssueDate: "", ReceiverName: "", Amount: "", Attachment: "" };
                        // $scope.issueDate = "";
                        $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
                        $scope.fileName = "";

                        //$scope.ResetForm();
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.MedicalHelpform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }
        //END SAVE MEDICAL INFORMATION


        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        
        //BEGIN RETRIVE MEDICAL INFORMATION (DATATABLE)
        $scope.RetrieveMedicalHelp = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    ECode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    MedicalHelpService.GetMedicalHelpList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().ECode, params.filter().EmployeeName, params.filter().PatientName, params.filter().Relation, params.filter().HospitalName, params.filter().ChequeIssueDate, params.filter().ChequeNumber, params.filter().ReceiverName, params.filter().MobileNumber, params.filter().Amount, params.filter().IsPatelSocialGroup, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                            } else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                }
            });
        }
        //END RETRIVE MEDICAL INFORMATION

        
        //BEGIN DELETE MEDICAL INFORMATION
        $scope.DeleteMedicalHelp = function (id) {
            $rootScope.IsAjaxLoading = true;
            MedicalHelpService.DeleteMedicalHelp(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {  // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        //END DELETE MEDICAL INFORMATION


        /*get record for edit medical Help*/
        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateMedicalHelp = function (_medHelp) {
            $scope.storage.lastRecord = _medHelp;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            if (_medHelp.IsPatelSocialGroup == true) {
          
                $scope.editData = {
                    SrNo: _medHelp.SrNo,
                    ECode: _medHelp.ECode,
                    EmployeeName: _medHelp.EmployeeName,
                    PatientName: _medHelp.PatientName,
                    Relation: _medHelp.Relation,
                    HospitalName: _medHelp.HospitalName,
                    IssueDate: $filter('date')(_medHelp.ChequeIssueDate, 'dd-MM-yyyy'),
                    ChequeNumber: _medHelp.ChequeNumber,
                    ReceiverName: _medHelp.ReceiverName,
                    MobileNumber: _medHelp.MobileNumber,
                    Amount: _medHelp.Amount,
                    QuotationAmount: _medHelp.QuotationAmount,
                    ApprovedBy: _medHelp.ApprovedBy,
                    Attachment: _medHelp.Attachment,
                    IsPatelSocialGroup: true
                }
            } else {

                $scope.editData = {
                    SrNo: _medHelp.SrNo,
                    ECode: _medHelp.ECode,
                    EmployeeName: _medHelp.EmployeeName,
                    PatientName: _medHelp.PatientName,
                    Relation: _medHelp.Relation,
                    HospitalName: _medHelp.HospitalName,
                    IssueDate: $filter('date')(_medHelp.ChequeIssueDate, 'dd-MM-yyyy'),
                    ChequeNumber: _medHelp.ChequeNumber,
                    ReceiverName: _medHelp.ReceiverName,
                    MobileNumber: _medHelp.MobileNumber,
                    Amount: _medHelp.Amount,
                    Attachment: _medHelp.Attachment
                }
            }
            //  $scope.socialWelExpDate = _socialWelExp.Date;
            $scope.issueDate = _medHelp.ChequeIssueDate;
            $scope.fileName = _medHelp.Attachment;
            $scope.MedicalHelpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDMedicalHelp.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        }
        //END EXPORT TO EXCEL

        //ShowMedicalHelp

        ///Model PopUp
        $scope.ShowMedicalHelp = function (MedicalHelp) {
            var modalInstance = $modal.open({
                templateUrl: 'MedicalHelpPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    MedicalHelpDetails: function () { return MedicalHelp; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, MedicalHelpDetails) {
            $scope.items = MedicalHelpDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };




        $scope.FilterPatelSocialGroup = function (column) {

            var def = $q.defer(),
            arr = [
                { id: "Y", title: "Yes" },
                { id: "N", title: "No" }
            ];
            def.resolve(arr);
            return def;
        };
    }

})();
