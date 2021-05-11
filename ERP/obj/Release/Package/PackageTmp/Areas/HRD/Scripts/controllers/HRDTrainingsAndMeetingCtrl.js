/// <reference path="../libs/angular/angular.js" />

'use strict';


angular.module("ERPApp.Controllers").controller("TrainingAndMeeting", [
    "$scope",
    "$rootScope",
    "$timeout",
    "TrainingAndMeetingService",
    "$http",
    "$filter",
    "ngTableParams",
    "$modal",
    function ($scope, $rootScope, $timeout, TrainingAndMeetingService, $http, $filter, ngTableParams, $modal) {

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

        $scope.calendarOpenDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDate = true;
        };

        $scope.$watch('date', function (newValue) {
            $scope.editData.Date = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateTrainingAndMeetingDate = function (trainingDate, TrainingMeetingform) {

            if (!trainingDate) {
                $scope.TrainingMeetingform.txtDate.$setValidity("invalidDate", true);
                return;
            } else if (trainingDate.length == 10) {
                if ($scope.ValidateDate(trainingDate)) {
                    //test
                    $scope.date = $scope.StringToDateString(trainingDate);
                    $scope.TrainingMeetingform.txtDate.$setValidity("invalidDate", true);
                } else {
                    $scope.TrainingMeetingform.txtDate.$setValidity("invalidDate", false);
                }
            } else {
                $scope.TrainingMeetingform.txtDate.$setValidity("invalidDate", false);
            }
        };

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

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };

        $scope.maxDate = $scope.maxDate || new Date();
        // End DATE PICKER


        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                Department: "", Manager: "", Subject: "", NoOfParticipant: "", Intercom: "", Date: "", Attachment: ""
            };

            $scope.editData.Date = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.date = $scope.StringToDateString($scope.editData.Date);
            $scope.fileName = "";
            // $scope.socialWelExpDate = "";
            $scope.TrainingMeetingform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM


        //BEGIN ADD SOCIAL WELFARE EXPENSE FORM BUTTON
        $scope.AddTrainingAndMeeting = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.Date = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.date = $scope.StringToDateString($scope.editData.Date);
        };
        //END ADD SOCIAL WELFARE EXPENSE FORM BUTTON



        $scope.FilterByDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };


        $scope.CloseTrainingMeeting = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };


        $scope.ResetDDPressMedia = function () {

                if ($scope.mode == "Edit") {
                    $scope.editData = {
                        Department: $scope.storage.lastRecord.Department,
                        Manager: $scope.storage.lastRecord.Manager,
                        Subject: $scope.storage.lastRecord.Subject,
                        NoOfParticipant: $scope.storage.lastRecord.NoOfParticipant,
                        Intercom: $scope.storage.lastRecord.Intercom,
                        Date: $filter('date')($scope.storage.lastRecord.Date, 'dd-MM-yyyy'),
                        Attachment: $scope.storage.lastRecord.Attachment
                    };
                    $scope.fileName = $scope.storage.lastRecord.Attachment;
                    $scope.date = $scope.storage.lastRecord.Date;
                } else { //mode == add
                    ResetForm();
                }
            };

        $scope.CreateUpdateTrainingAndMeeting = function (trainingMeeting) {

            var IDate = $filter('date')(trainingMeeting.Date, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            trainingMeeting.Date = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

            TrainingAndMeetingService.CreateUpdateTrainingAndMeeting(trainingMeeting, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { Department: "", Manager: "", Subject: "", NoOfParticipant: "", Intercom: "", Date: "", Attachment: "" };
                        $scope.fileName = "";
                        $scope.editData.Date = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.date = $scope.StringToDateString($scope.editData.Date);
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.TrainingMeetingform.$setPristine();
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


        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        $scope.RetrieveTrainingMeeting = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Department: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    TrainingAndMeetingService.GetTrainingMeetingList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Department, params.filter().Manager, params.filter().Subject, params.filter().NoOfParticipant, params.filter().Intercom, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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


        $scope.UpdateTrainingMeeting = function (trainingMeeting) {

            $scope.storage.lastRecord = trainingMeeting;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.editData = {
                SrNo: trainingMeeting.SrNo,
                Department: trainingMeeting.Department,
                Manager: trainingMeeting.Manager,
                Subject: trainingMeeting.Subject,
                NoOfParticipant: trainingMeeting.NoOfParticipant,
                Intercom: trainingMeeting.Intercom,
                Date: $filter('date')(trainingMeeting.Date, 'dd-MM-yyyy'),
                Attachment: trainingMeeting.Attachment
            };
            $scope.date = trainingMeeting.Date;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };


        $scope.DeleteTrainingMeeting = function (id) {
            $rootScope.IsAjaxLoading = true;
            TrainingAndMeetingService.DeleteTrainingMeeting(id).then(function (result) {
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


        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDTrainingsAndMeeting.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };




        ///Model PopUp
        $scope.ShowTrainingMeeting = function (TrainingMeeting) {

            
            var modalInstance = $modal.open({
                templateUrl: 'TrainingMeetingPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    TrainingMeetingDetails: function () { return TrainingMeeting; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, TrainingMeetingDetails) {
            $scope.items = TrainingMeetingDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };




    }
]);