/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDFireExtinguiserCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "FireExtinguiserService", "ngTableParams", "$modal", "Upload",
           HRDFireExtinguiserCtrl
       ]);

    function HRDFireExtinguiserCtrl($scope, $rootScope, $timeout, $http, $filter, FireExtinguiserService, ngTableParams, $modal, Upload) {
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

        $scope.calendarOpenDateOfInspection = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfInspection = true;
            $scope.editData.calOpenDateOfRefilling = false;
            $scope.editData.calOpenDueDateForNextRefilling = false;
        };

        $scope.calenderOpenDateOfRefilling = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfRefilling = true;
            $scope.editData.calOpenDateOfInspection = false;
            $scope.editData.calOpenDueDateForNextRefilling = false;
        };


        $scope.calendarOpenDueDateForNextRefilling = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDueDateForNextRefilling = true;
            $scope.editData.calOpenDateOfRefilling = false;
            $scope.editData.calOpenDateOfInspection= false;
        };

        $scope.$watch('dateofinspaction', function (newValue) {
            $scope.editData.DateOfInspection = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.$watch('dateofrefilling', function (newValue) {
            $scope.editData.DateOfRefilling = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.$watch('duedatereffilling', function (newValue) {
            $scope.editData.DueDateForNextRefilling = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateDateOfInspection = function (dateofinspaction, FireExtinguiserform) {
            if (!dateofinspaction) {
                $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", true);
                return;
            } else if (dateofinspaction.length == 10) {
                if ($scope.ValidateDate(dateofinspaction)) {
                    //test
                    $scope.dateofinspaction = $scope.StringToDateString(dateofinspaction);
                    $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", true);
                } else {
                    $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", false);
                }
            } else {
                $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", false);
            }
        };

        $scope.ValidateDateOfRefilling = function (dateofrefilling, FireExtinguiserform) {
            if (!dateofrefilling) {
                $scope.FireExtinguiserform.txtDateOfRefilling.$setValidity("invalidDateOfRefilling", true);
                return;
            } else if (dateofrefilling.length == 10) {
                if ($scope.ValidateDate(dateofrefilling)) {
                    //test
                    $scope.dateofrefilling = $scope.StringToDateString(dateofrefilling);
                    $scope.FireExtinguiserform.txtDateOfRefilling.$setValidity("invalidDateOfRefilling", true);
                } else {
                    $scope.FireExtinguiserform.txtDateOfRefilling.$setValidity("invalidDateOfRefilling", false);
                }
            } else {
                $scope.FireExtinguiserform.txtDateOfRefilling.$setValidity("invalidDateOfRefilling", false);
            }
        };

        $scope.ValidateDueDateForNextRefilling = function (duedatereffilling, FireExtinguiserform) {
            if (!duedatereffilling) {
                $scope.FireExtinguiserform.txtDueDateForNextRefilling.$setValidity("invalidDueDateForNextRefilling", true);
                return;
            } else if (duedatereffilling.length == 10) {
                if ($scope.ValidateDate(duedatereffilling)) {
                    //test
                    $scope.duedatereffilling = $scope.StringToDateString(duedatereffilling);
                    $scope.FireExtinguiserform.txtDueDateForNextRefilling.$setValidity("invalidDueDateForNextRefilling", true);
                } else {
                    $scope.FireExtinguiserform.txtDueDateForNextRefilling.$setValidity("invalidDueDateForNextRefilling", false);
                }
            } else {
                $scope.FireExtinguiserform.txtDueDateForNextRefilling.$setValidity("invalidDueDateForNextRefilling", false);
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


        //BEGIN FILE UPLODER
        $scope.upload = function (files) {
            Upload.upload({
                url: "../Handler/FileUpload.ashx",
                file: files
            }).progress(function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);

            }).success(function (data, status, headers, config) {
                $scope.editData.Attachment = data;
            });
        }
        $scope.$watch('rejFiles', function () {
            if (angular.isArray($scope.rejFiles)) {
                $scope.rejFiles.forEach(function (file) {
                    toastr.error('The selected file is not a valid file. Please select a valid *.xls, *.pdf or *.doc file and try again ', 'Invalid selected file');
                });
            }
        });
        //END FILE UPLODER

        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                TypeOfFireExtinguiser: "", Capacity: "", Location: "", UsedOfFireExtinguiser: "", Reason: "", Remark: "", Attachment: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

            $scope.editData.DateOfRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofrefilling = $scope.StringToDateString($scope.editData.DateOfRefilling);

            $scope.editData.DueDateForNextRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.duedatereffilling = $scope.StringToDateString($scope.editData.DueDateForNextRefilling);


            $scope.FireExtinguiserform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM


        //BEGIN ADD FIRE EXTINGUISER LOG BOOK BUTTON
        $scope.AddFireExtinguiserLogBook = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form

            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

            $scope.editData.DateOfRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofrefilling = $scope.StringToDateString($scope.editData.DateOfRefilling);

            $scope.editData.DueDateForNextRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.duedatereffilling = $scope.StringToDateString($scope.editData.DueDateForNextRefilling);

        };
        //END ADD  FIRE EXTINGUISER LOG BOOK FORM BUTTON

        $scope.FilterDateOfRefilling = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN Close FIRE EXTINGUISER LOG BOOK
        $scope.CloseFireExtinguiser = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END Close FIRE EXTINGUISER LOG BOOK

        //BEGIN RESET FIRE EXTINGUISER LOG BOOK
        $scope.ResetFireExtinguiser = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    TypeOfFireExtinguiser: $scope.storage.lastRecord.TypeOfFireExtinguiser,
                    Capacity: $scope.storage.lastRecord.Capacity,
                    Location: $scope.storage.lastRecord.Location,
                    DateOfInspection: $filter('date')($scope.storage.lastRecord.DateOfInspection, 'dd-MM-yyyy'),
                    UsedOfFireExtinguiser: $scope.storage.lastRecord.UsedOfFireExtinguiser,
                    DateOfRefilling: $filter('date')($scope.storage.lastRecord.DateOfRefilling, 'dd-MM-yyyy'),
                    DueDateForNextRefilling: $filter('date')($scope.storage.lastRecord.DueDateForNextRefilling, 'dd-MM-yyyy'),
                    Reason: $scope.storage.lastRecord.Reason,
                    Remark: $scope.storage.lastRecord.Remark,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.file = $scope.storage.lastRecord.Attachment;
                $scope.rejFiles = [];

            } else {
                ResetForm();
            }
        };
        //BEGIN END FIRE EXTINGUISER LOG BOOK


        //BEGIN SAVE FIRE EXTINGUISER LOG BOOK INFORMATION
        $scope.CreateFireExtinguiser= function (FireExtinguiser) {

            var IDate = $filter('date')(FireExtinguiser.DateOfInspection, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            FireExtinguiser.DateOfInspection = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

            var tempDate = $filter('date')(FireExtinguiser.DateOfRefilling, 'dd-MM-yyyy').split('-');
            var tempDateOfRefilling = new Date(parseInt(tempDate[2]), parseInt(tempDate[1]) - 1, parseInt(tempDate[0]), 0, 0, 0);
            FireExtinguiser.DateOfRefilling = $filter('date')(tempDateOfRefilling, 'MM-dd-yyyy HH:mm:ss');

            var DueDate = $filter('date')(FireExtinguiser.DueDateForNextRefilling, 'dd-MM-yyyy').split('-');
            var tempDueDate = new Date(parseInt(DueDate[2]), parseInt(DueDate[1]) - 1, parseInt(DueDate[0]), 0, 0, 0);
            FireExtinguiser.DueDateForNextRefilling = $filter('date')(tempDueDate, 'MM-dd-yyyy HH:mm:ss');

                        //CREATE FIRE EXTINGUISER LOG BOOK 

            FireExtinguiserService.CreateFireExtinguiserLogBook(FireExtinguiser, $scope.timeZone).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { TypeOfFireExtinguiser: "", Capacity: "", Location: "", UsedOfFireExtinguiser: "", Reason: "", Remark: "", Attachment: "" };

                        $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

                        $scope.editData.DateOfRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateofrefilling = $scope.StringToDateString($scope.editData.DateOfRefilling);

                        $scope.editData.DueDateForNextRefilling = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.duedatereffilling = $scope.StringToDateString($scope.editData.DueDateForNextRefilling);


                        $scope.fileName = "";

                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.FireExtinguiserform.$setPristine();
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
        //END SAVE FIRE EXTINGUISER LOG BOOK 

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVEFIRE EXTINGUISER LOG BOOK  INFORMATION (DATATABLE)   
        $scope.RetrieveFireExtinguiser = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    NameOfIssuer: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    FireExtinguiserService.RetFireExtinguiser($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TypeOfFireExtinguiser, params.filter().Capacity, params.filter().Location, params.filter().DateOfInspection, params.filter().UsedOfFireExtinguiser, params.filter().DateOfRefilling, params.filter().DueDateForNextRefilling, params.filter().Reason, params.filter().Remark, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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
                            }
                            else {
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
        //END RETRIVE FIRE EXTINGUISER LOG BOOK  (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateFireExtinguiser = function (_FireExtinguiser) {

            console.log(_FireExtinguiser)

            $scope.storage.lastRecord = _FireExtinguiser;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _FireExtinguiser.SrNo,
                TypeOfFireExtinguiser: _FireExtinguiser.TypeOfFireExtinguiser,
                Capacity: _FireExtinguiser.Capacity,
                Location: _FireExtinguiser.Location,
                DateOfInspection: $filter('date')(_FireExtinguiser.DateOfInspection, 'dd-MM-yyyy'),
                UsedOfFireExtinguiser: _FireExtinguiser.UsedOfFireExtinguiser,
                DateOfRefilling: $filter('date')(_FireExtinguiser.DateOfRefilling, 'dd-MM-yyyy'),
                DueDateForNextRefilling: $filter('date')(_FireExtinguiser.DueDateForNextRefilling, 'dd-MM-yyyy'),
                Reason: _FireExtinguiser.Reason,
                Remark: _FireExtinguiser.Remark,
                Attachment: _FireExtinguiser.Attachment,
                files: _FireExtinguiser.Attachment
            };
            $scope.dateofinspaction = _FireExtinguiser.DateOfInspection;
            $scope.dateofrefilling = _FireExtinguiser.DateOfRefilling;
            $scope.duedatereffilling = _FireExtinguiser.DueDateForNextRefilling;
            $scope.isFirstFocus = false;
            $timeout(function () {
            $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM

        //change service name and button name
        //BEGIN DELETE FIRE EXTINGUISER LOG BOOK  INFORMATION
        $scope.DeleteFireExtinguiser = function (id) {
            $rootScope.IsAjaxLoading = true;

            FireExtinguiserService.DeleteFireExtinguiser(id).then(function (result) {
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
        //END DELETE FIRE EXTINGUISER LOG BOOK  INFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDFireExtinguiserLogBook.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowFireExtinguiser = function (FireExtinguiser) {
            var modalInstance = $modal.open({
                templateUrl: 'FireExtinguiser.html',
                controller: HRDFireExtinguiserCtrl,
                resolve: {
                    FireExtinguiserDetails: function () { return FireExtinguiser; }
                }
            });
        }
        var HRDFireExtinguiserCtrl = function ($scope, $modalInstance, FireExtinguiserDetails) {
            $scope.items = FireExtinguiserDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();


