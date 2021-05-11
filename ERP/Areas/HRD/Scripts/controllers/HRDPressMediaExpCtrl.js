/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';


    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("PressMediaExpCtrl", [
             "$scope", "$rootScope", "$timeout", "$http", "$filter", "PressMediaExpService", "ngTableParams","$modal",
            pressMediaExpCtrl
        ]);

    function pressMediaExpCtrl($scope, $rootScope, $timeout, $http, $filter, PressMediaExpService, ngTableParams, $modal) {

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
        };
       
        $scope.ValidateIssuedDate = function (issueDate, PressMediaExpform) {
            if (!issueDate) {
                $scope.PressMediaExpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    //test
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.PressMediaExpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.PressMediaExpform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.PressMediaExpform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.IssueDate = $filter('date')(newValue, 'dd-MM-yyyy');
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
                NameOfPressMedia: "", RepresentativeName: "", Occasion: "", ApprovedBy: "", Amount: "", IssueDate: "",MobileNumber:"" ,Attachment: ""
            };
            $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
            $scope.fileName = "";
            $scope.PressMediaExpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD PRESS MEDIA EXPENSE FORM BUTTON
        $scope.AddPressMediaExp = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
        };
        //END ADD PRESS MEDIA EXPENSE FORM BUTTON

        $scope.FilterByIssueDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN CLOSE SOCIAL WELFARE EXPENSE FORM
        $scope.ClosePressMediaExp = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE SOCIAL WELFARE EXPENSE FORM


        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM
        $scope.ResetPressMediaExp = function () {
            if ($scope.mode == "Edit") {
                $scope.editData = {
                    NameOfPressMedia: $scope.storage.lastRecord.NameOfPressMedia,
                    RepresentativeName: $scope.storage.lastRecord.RepresentativeName,
                    MobileNumber: $scope.storage.lastRecord.MobileNumber,
                    Amount: $scope.storage.lastRecord.Amount,
                    ApprovedBy: $scope.storage.lastRecord.ApprovedBy,
                    Occasion: $scope.storage.lastRecord.Occasion,
                    IssueDate: $filter('date')($scope.storage.lastRecord.Date, 'dd-MM-yyyy'),
                    Attachment: $scope.storage.lastRecord.Attachment

                };
                $scope.fileName = $scope.storage.lastRecord.Attachment;
            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM


        //BEGIN SAVE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.CreateUpdatePressMediaExp = function (pressMediaExp) {

            var IDate = $filter('date')(pressMediaExp.IssueDate, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            pressMediaExp.Date = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');
       

            PressMediaExpService.CreateUpdatePressMediaExp(pressMediaExp, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { NameOfPressMedia: "", RepresentativeName: "", Occasion: "", ApprovedBy: "", Amount: "", IssueDate: "", MobileNumber: "", Attachment: "" };
                        $scope.fileName = "";
                        $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate)
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.PressMediaExpform.$setPristine();
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

        //service name : PressMediaExpService

        //BEGIN RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   
        $scope.RetrievePressMediaExp = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    NameOfPressMedia: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    PressMediaExpService.GetPressMediaExpList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().NameOfPressMedia, params.filter().RepresentativeName, params.filter().Date1, params.filter().MobileNumber, params.filter().Amount, params.filter().ApprovedBy, params.filter().Occasion, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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

        //SocialWelExpDate = IssueDate
        //socialWelExp = issueDate


        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdatePressMediaExp = function (_pressMediaExp) {

            $scope.storage.lastRecord = _pressMediaExp;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
  
            $scope.editData = {
                SrNo: _pressMediaExp.SrNo,
                NameOfPressMedia: _pressMediaExp.NameOfPressMedia,
                RepresentativeName: _pressMediaExp.RepresentativeName,
                MobileNumber: _pressMediaExp.MobileNumber,
                Amount: _pressMediaExp.Amount,
                ApprovedBy: _pressMediaExp.ApprovedBy,
                Occasion: _pressMediaExp.Occasion,
                IssueDate: $filter('date')(_pressMediaExp.Date, 'dd-MM-yyyy'),
                Attachment: _pressMediaExp.Attachment
                
            };
            $scope.fileName = _pressMediaExp.Attachment;
            //$scope.socialWelExpDate = _socialWelExp.Date;
            $scope.issueDate = _pressMediaExp.Date;
            $scope.PressMediaExpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM

        //BEGIN DELETE PRESS MEDIA EXPENSE INFORMATION
        $scope.DeletePressMediaExp = function (id) {
            $rootScope.IsAjaxLoading = true;
            PressMediaExpService.DeletePressMediaExp(id).then(function (result) {
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
        //END DELETE PRESS MEDIA WELFARE EXPENSE INFORMATION

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDPressMediaExp.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //END EXPORT TO EXCEL

        

           


        $scope.ShowPressMediaExpens = function (PressMediaExpens) {

            

            var modalInstance = $modal.open({
                templateUrl: 'PressMediaExpensPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    PressMediaExpensDetails: function () { return PressMediaExpens; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, PressMediaExpensDetails) {
            $scope.items = PressMediaExpensDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };



    }

})();
