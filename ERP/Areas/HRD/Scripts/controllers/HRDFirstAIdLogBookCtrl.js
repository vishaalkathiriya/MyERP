/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDFirstAIdLogBookCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "FirstAIdLogBookService", "ngTableParams", "$modal", "Upload",
           HRDFirstAIdLogBookCtrl
       ]);

    function HRDFirstAIdLogBookCtrl($scope, $rootScope, $timeout, $http, $filter, FirstAIdLogBookService, ngTableParams, $modal, Upload) {
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




        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenIssueDate = true;
            $scope.editData.calOpenDateOfExpiry = false;
        };

        $scope.calendarOpenDateOfExpiry = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfExpiry = true;
            $scope.editData.calOpenIssueDate = false;
        };

        $scope.$watch('dateofissue', function (newValue) {
            $scope.editData.DateOfIssue = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.$watch('expirydate', function (newValue) {
            $scope.editData.ExpiryDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });



        $scope.ValidateIssuedDate = function (dateofissue, HRDFirstAIdLogBookCtrlform) {
            if (!dateofissue) {
                $scope.HRDFirstAIdLogBookCtrlform.txtDateOfIssue.$setValidity("InvaliddateOfIssue", true);
                return;
            } else if (dateofissue.length == 10) {
                if ($scope.ValidateDate(dateofissue)) {
                    //test
                    $scope.dateofissue = $scope.StringToDateString(dateofissue);
                    $scope.HRDFirstAIdLogBookCtrlform.txtDateOfIssue.$setValidity("InvaliddateOfIssue", true);
                } else {
                    $scope.HRDFirstAIdLogBookCtrlform.txtDateOfIssue.$setValidity("InvaliddateOfIssue", false);
                }
            } else {
                $scope.HRDFirstAIdLogBookCtrlform.txtDateOfIssue.$setValidity("InvaliddateOfIssue", false);
            }
        };

        


        $scope.ValidateDateOfExpiry = function (expirydate, HRDFirstAIdLogBookCtrlform) {
            if (!expirydate) {
                $scope.HRDFirstAIdLogBookCtrlform.ExpiryDate.$setValidity("invalidExpiryDate", true);
                return;
            } else if (expirydate.length == 10) {
                if ($scope.ValidateDate(expirydate)) {
                    //test
                    $scope.expirydate = $scope.StringToDateString(expirydate);
                    $scope.HRDFirstAIdLogBookCtrlform.ExpiryDate.$setValidity("invalidExpiryDate", true);
                } else {
                    $scope.HRDFirstAIdLogBookCtrlform.ExpiryDate.$setValidity("invalidExpiryDate", false);
                }
            } else {
                $scope.HRDFirstAIdLogBookCtrlform.ExpiryDate.$setValidity("invalidIssueDate", false);
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
            })
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
                NameOfIssuer: "", NameOfReceiver: "", NameOfFirstAIdItems: "", Quanity: "", Size: "", ManagerName: "", LocationOfFirstAIdBox: "", Price: "", Remarks: "", Attachment: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfIssue = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofissue = $scope.StringToDateString($scope.editData.DateOfIssue);

            $scope.editData.ExpiryDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.expirydate = $scope.StringToDateString($scope.editData.ExpiryDate);


            $scope.FirstAIdLogBookform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM




        //BEGIN ADD FIRST AID LOG BOOK BUTTON
        $scope.AddFirstAIdLogBookform = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form

            $scope.editData.DateOfIssue = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofissue = $scope.StringToDateString($scope.editData.DateOfIssue);

            $scope.editData.ExpiryDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.expirydate = $scope.StringToDateString($scope.editData.ExpiryDate);
        };
        //END ADD FIRST AID LOG BOOK FORM BUTTON


        $scope.FilterByExpiryDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };



        //BEGIN CloseFirstAIdLogBook FORM
        $scope.CloseFirstAIdLogBookform = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CloseFirstAIdLogBook FORM


        //BEGIN RESET First aid log book
        $scope.ResetFirstAIdLogBookform = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {


                    NameOfIssuer: $scope.storage.lastRecord.NameOfIssuer,
                    NameOfReceiver: $scope.storage.lastRecord.NameOfReceiver,
                    NameOfFirstAIdItems: $scope.storage.lastRecord.NameOfFirstAIdItems,
                    DateOfIssue: $filter('date')($scope.storage.lastRecord.DateOfIssue, 'dd-MM-yyyy'),
                    Quanity: $scope.storage.lastRecord.Quanity,
                    Size: $scope.storage.lastRecord.Size,
                    ManagerName: $scope.storage.lastRecord.ManagerName,
                    LocationOfFirstAIdBox: $scope.storage.lastRecord.LocationOfFirstAIdBox,
                    Price: $scope.storage.lastRecord.Price,
                    ExpiryDate: $filter('date')($scope.storage.lastRecord.ExpiryDate, 'dd-MM-yyyy'),
                    Remarks: $scope.storage.lastRecord.Remarks,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.file = $scope.storage.lastRecord.Attachment;
                $scope.rejFiles = [];

            } else {
                ResetForm();
            }
        };
        //BEGIN END FIRST AID LOG BOOK FORM


        //BEGIN SAVE FIRST AID LOG BOOK INFORMATION
        $scope.CreateUpdateFirstAIdLogBook = function (FirstAIdLogBook) {

            var IDate = $filter('date')(FirstAIdLogBook.DateOfIssue, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);

            FirstAIdLogBook.DateOfIssue = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

            var ExpiryDate = $filter('date')(FirstAIdLogBook.ExpiryDate, 'dd-MM-yyyy').split('-');
            var temp_Expiry_Date = new Date(parseInt(ExpiryDate[2]), parseInt(ExpiryDate[1]) - 1, parseInt(ExpiryDate[0]), 0, 0, 0);

            FirstAIdLogBook.ExpiryDate = $filter('date')(temp_Expiry_Date, 'MM-dd-yyyy HH:mm:ss');


            FirstAIdLogBookService.CreateUpdateFirstAIdLogBook(FirstAIdLogBook, $scope.timeZone).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { NameOfIssuer: "", NameOfReceiver: "", NameOfFirstAIdItems: "", DateOfIssue: "", Quanity: "", Size: "", ManagerName: "", LocationOfFirstAIdBox: "", Price: "", ExpiryDate: "", Remarks: "", Attachment: "" };
                        


                        $scope.editData.DateOfIssue = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateofissue = $scope.StringToDateString($scope.editData.DateOfIssue);

                        $scope.editData.ExpiryDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.expirydate = $scope.StringToDateString($scope.editData.ExpiryDate);
                        $scope.fileName = "";

                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.FirstAIdLogBookform.$setPristine();
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
        //END SAVE First Aid INFORMATION

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE FIRST ASSIST LOG BOOK INFORMATION (DATATABLE)   
        $scope.RetrieveFirstAIdLogBook = function () {
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

                    FirstAIdLogBookService.GetFirstAIdLogBook($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().NameOfIssuer, params.filter().NameOfReceiver, params.filter().NameOfFirstAIdItems, params.filter().DateOfIssue, params.filter().Quanity, params.filter().Size, params.filter().ManagerName, params.filter().LocationOfFirstAIdBox, params.filter().Price, params.filter().ExpiryDate, params.filter().Remarks, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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
        //END RETRIVE FIRST AID LOG RECORD (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateFirstAIdLogBook = function (_FirstAIdLogBook) {

            $scope.storage.lastRecord = _FirstAIdLogBook;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _FirstAIdLogBook.SrNo,
                NameOfIssuer: _FirstAIdLogBook.NameOfIssuer,
                NameOfReceiver: _FirstAIdLogBook.NameOfReceiver,
                NameOfFirstAIdItems: _FirstAIdLogBook.NameOfFirstAIdItems,
                DateOfIssue: $filter('date')(_FirstAIdLogBook.DateOfIssue, 'dd-MM-yyyy'),
                Quanity: _FirstAIdLogBook.Quanity,
                Size: _FirstAIdLogBook.Size,
                ManagerName: _FirstAIdLogBook.ManagerName,
                LocationOfFirstAIdBox: _FirstAIdLogBook.LocationOfFirstAIdBox,
                Price: _FirstAIdLogBook.Price,
                ExpiryDate: $filter('date')(_FirstAIdLogBook.ExpiryDate, 'dd-MM-yyyy'),
                Remarks: _FirstAIdLogBook.Remarks,
                Attachment: _FirstAIdLogBook.Attachment,
                files: _FirstAIdLogBook.Attachment
            };

            $scope.dateofissue = _FirstAIdLogBook.DateOfIssue;
            $scope.expirydate = _FirstAIdLogBook.ExpiryDate;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE FIRST AID LOGBOOK   INFORMATION
        $scope.DeleteFirstAIdLogBook = function (id) {
            $rootScope.IsAjaxLoading = true;

            FirstAIdLogBookService.DeleteFirstAIdLogBook(id).then(function (result) {
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
        //END DELETE FIRST AID LOGBOOK  INFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDFirstAIdLogBook.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowFirstAIdLogBook = function (FirstAIdLogBook) {
            var modalInstance = $modal.open({
                templateUrl: 'FirstAIdLogBook.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    FirstAIdLogBookDetails: function () { return FirstAIdLogBook; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, FirstAIdLogBookDetails) {
            $scope.items = FirstAIdLogBookDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();


