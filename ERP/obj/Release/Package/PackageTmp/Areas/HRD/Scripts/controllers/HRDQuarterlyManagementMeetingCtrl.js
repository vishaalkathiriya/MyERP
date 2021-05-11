/// <reference path="../libs/angular/angular.js" />

'use strict';

angular.module("ERPApp.Controllers").controller("QuarterlyManagementMeetingCtrl", [
    "$scope",
    "$rootScope",
    "$timeout",
    "QuarterlyManagementMeetingService",
    "$http",
    "$filter",
    "ngTableParams",
    function ($scope, $rootScope, $timeout, QMMS, $http, $filter, ngTableParams) {

        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";   
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.isFirstFocus = false;
        $scope.storage = { lastRecord: "" };
        $scope.Attachment = "";
        $scope.ischanged = false;
        $rootScope.isFormVisible = false;  
     //   $scope.lastMeeting = $scope.lastMeeting || {};
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
        $scope.maxDate = $scope.maxDate || new Date();
        $scope.calendarOpenDateOfMeeting = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfMeeting = true;
        }
        $scope.$watch('dateOfMeeting', function (newValue) {
            $scope.editData.DateOfMeeting = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateDateOfMeeting = function (date, Meetingform) {
            if (!date) {
                $scope.Meetingform.txtDateOfMeeting.$setValidity("invalidDate", true);
                return;
            } else if (date.length == 10) {
                if ($scope.ValidateDate(date)) {

                    $scope.dateOfMeeting = $scope.StringToDateString(date);
                    $scope.Meetingform.txtDateOfMeeting.$setValidity("invalidDate", true);
                } else {
                    $scope.Meetingform.txtDateOfMeeting.$setValidity("invalidDate", false);
                }
            } else {
                $scope.Meetingform.txtDateOfMeeting.$setValidity("invalidDate", false);
            }
        };
        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };
      
        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };
        // END DATE PICKER



        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                SrNo:"",  Title: "", ListOfParticipants: "", DateOfMeeting: "", Attachment: "", AgendaOfTraining: "", DecisionTakenDuringMeeting: ""
            };
            $scope.editData.DateOfMeeting = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateOfMeeting = $scope.StringToDateString($scope.editData.DateOfMeeting);
            $scope.fileName = "";
            // $scope.socialWelExpDate = "";
            $scope.Meetingform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD SOCIAL WELFARE EXPENSE FORM BUTTON
        $scope.AddMeeting = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.DateOfMeeting = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateOfMeeting = $scope.StringToDateString($scope.editData.DateOfMeeting);
        };
        //END ADD SOCIAL WELFARE EXPENSE FORM BUTTON

        $scope.FilterByMeetingDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN CLOSE SOCIAL WELFARE EXPENSE FORM
        $scope.CloseMeeting = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE SOCIAL WELFARE EXPENSE FORM


        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM
        $scope.ResetMeeting = function () {
            if ($scope.mode == "Edit") {
                $scope.editData = {
                    Title: $scope.storage.lastRecord.Title,
                    ListOfParticipants: $scope.storage.lastRecord.ListOfParticipants,
                    DateOfMeeting: $filter('date')($scope.storage.lastRecord.DateOfMeeting, 'dd-MM-yyyy'),
                    Attachment: $scope.storage.lastRecord.Attachment,
                    AgendaOfTraining: $scope.storage.lastRecord.AgendaOfTraining,
                    DecisionTakenDuringMeeting: $scope.storage.lastRecord.DecisionTakenDuringMeeting
                };
                $scope.dateOfMeeting = $scope.storage.lastRecord.DateOfMeeting;
                $scope.fileName = $scope.storage.lastRecord.Attachment;
            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM



        //BEGIN SAVE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.CreateUpdateMeeting = function (meetingManagement) {
            var IDate = $filter('date')(meetingManagement.DateOfMeeting, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            meetingManagement.DateOfMeeting = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');
            QMMS.CreateUpdateMeeting(meetingManagement, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { Title: "", ListOfParticipants: "", DateOfMeeting: "", Attachment: "", AgendaOfTraining: "", DecisionTakenDuringMeeting: "" };
                        $scope.fileName = "";
                        $scope.editData.DateOfMeeting = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateOfMeeting = $scope.StringToDateString($scope.editData.DateOfMeeting);

                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.Meetingform.$setPristine();
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

        //baki

        //BEGIN RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   
        $scope.RetrieveMeetings = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Title: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    QMMS.GetMeetingList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Title, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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
        //END RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   








        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateMeeting = function (meetingManagement) {

            $scope.storage.lastRecord = meetingManagement;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: meetingManagement.SrNo,
                Title: meetingManagement.Title,
                ListOfParticipants: meetingManagement.ListOfParticipants,
                DateOfMeeting: $filter('date')(meetingManagement.DateOfMeeting, 'dd-MM-yyyy'),
                Attachment: meetingManagement.Attachment,
                AgendaOfTraining: meetingManagement.AgendaOfTraining,
                DecisionTakenDuringMeeting: meetingManagement.DecisionTakenDuringMeeting
            };
            $scope.fileName = meetingManagement.Attachment;
        //     $scope.editData.Attachment = _socialWelExp.Attachment;
            $scope.dateOfMeeting = meetingManagement.DateOfMeeting;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM

        //BEGIN DELETE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.DeleteMeeting = function (id) {
            $rootScope.IsAjaxLoading = true;
            QMMS.DeleteMeeting(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        //END DELETE SOCIAL WELFARE EXPENSE INFORMATION

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDQuarterlyManagementMeeting.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        $scope.ShowMeetingInfo = function (id) {
           // $rootScope.IsAjaxLoading = true;
                document.location.href = "../../Handler/HRDQuarterlyManagementMeetingPDFFile.ashx?SrNo=" + id 


            // // USE FOR PDF OPEN IN NEW TAB
            //QMMS.ShowMeetingInfo(id).then(function (result) {
            //   //     $scope.editData.temp_data = result.data;
            //    $rootScope.IsAjaxLoading = false;
            //});


        };


      

    }
]);




//BannerServ.GetBannerType().then(function (result) {
//    $scope.BannerTypes = result.data;
//    //$scope.editData.Banner_Type = 0;
//});