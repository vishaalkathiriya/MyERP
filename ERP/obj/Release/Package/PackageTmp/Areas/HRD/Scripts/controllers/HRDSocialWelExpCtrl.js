/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SocialWelExpCtrl", [
             "$scope", "$rootScope", "$timeout", "$http", "$filter", "SocialWelExpService", "ngTableParams", "$modal",
            socialWelExpCtrl
        ]);

    function socialWelExpCtrl($scope, $rootScope, $timeout, $http, $filter, SocialWelExpService, ngTableParams, $modal) {
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

        $scope.calendarOpenSocialWelExpDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenSocialWelExpDate = true;
        };

        //$scope.$watch('editdata.SocialWelExpDate', function (newvalue) {

        //    $scope.socialWelExpDate = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 

        //    //  // var test = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 
        //    //  // $scope.issuedate = $scope.stringtodatestring(test);
        //});

        //$scope.$watch('socialWelExpDate', function (newvalue) {
        //    console.log(newvalue);


        //    $scope.editData.SocialWelExpDate = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 
        //});

        $scope.$watch('socialWelExpDate', function (newValue) {
            $scope.editData.SocialWelExpDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });


        $scope.ValidateSocialWelExpdDate = function (socialWelExpDate, SocialWelExpform) {
            if (!socialWelExpDate) {
                $scope.SocialWelExpform.txtSocialWelExpDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (socialWelExpDate.length == 10) {
                if ($scope.ValidateDate(socialWelExpDate)) {
                    //test
                    $scope.socialWelExpDate = $scope.StringToDateString(socialWelExpDate);
                    $scope.SocialWelExpform.txtSocialWelExpDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.SocialWelExpform.txtSocialWelExpDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.SocialWelExpform.txtSocialWelExpDate.$setValidity("invalidIssueDate", false);
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
                ProgrammeName: "", Venue: "", GuestName: "", ExpenseAmount: "", SocialWelExpDate: "", Time: "", Attachment: ""
            };

            $scope.editData.SocialWelExpDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.socialWelExpDate = $scope.StringToDateString($scope.editData.SocialWelExpDate);
            $scope.fileName = "";
           // $scope.socialWelExpDate = "";
            $scope.SocialWelExpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD SOCIAL WELFARE EXPENSE FORM BUTTON
        $scope.AddSocialWelExp = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.SocialWelExpDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.socialWelExpDate = $scope.StringToDateString($scope.editData.SocialWelExpDate);
        };
        //END ADD SOCIAL WELFARE EXPENSE FORM BUTTON

        $scope.FilterByDate = function () {
            $scope.RefreshTable();
        };

        //BEGIN CLOSE SOCIAL WELFARE EXPENSE FORM
        $scope.CloseSocialWelExp = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE SOCIAL WELFARE EXPENSE FORM


        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM
        $scope.ResetSocialWelExp = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    ProgrammeName: $scope.storage.lastRecord.ProgrammeName,
                    Venue: $scope.storage.lastRecord.Venue,
                    GuestName: $scope.storage.lastRecord.GuestName,
                    ExpenseAmount: $scope.storage.lastRecord.ExpenseAmount,
                    SocialWelExpDate: $filter('date')($scope.storage.lastRecord.Date, 'dd-MM-yyyy'),
                    Time: $scope.storage.lastRecord.Time,
                    Attachment:$scope.storage.lastRecord.Attachment
                };
                $scope.fileName = $scope.storage.lastRecord.Attachment;
                $scope.socialWelExpDate = $scope.storage.lastRecord.Date;

            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM

         //BEGIN SAVE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.CreateUpdateSocialWelExp = function (socialWelExp) {

                var IDate = $filter('date')(socialWelExp.SocialWelExpDate, 'dd-MM-yyyy').split('-');
                var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);

                socialWelExp.Date = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

                SocialWelExpService.CreateUpdateSocialWelExp(socialWelExp, $scope.timeZone).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.editData = { ProgramName: "", Venue: "", GuestName: "", ExpAmount: "", SocialWelExpDate: "", Time: "" };
                            $scope.fileName = "";
                            $scope.editData.SocialWelExpDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                            $scope.socialWelExpDate = $scope.StringToDateString($scope.editData.SocialWelExpDate)
                            $scope.RefreshTable();
                            $scope.storage.lastRecord = {};
                            $scope.isFirstFocus = false;
                            $scope.SocialWelExpform.$setPristine();
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
        
        //BEGIN RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   
        $scope.RetrieveSocialWelExp = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    ProgrammeName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SocialWelExpService.GetSocialWelExpList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().ProgrammeName, params.filter().Venue, params.filter().Date, params.filter().Time, params.filter().ExpenseAmount, params.filter().GuestName, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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
        $scope.UpdateSocialWelExp = function (_socialWelExp) {

            $scope.storage.lastRecord = _socialWelExp;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _socialWelExp.SrNo,
                ProgrammeName: _socialWelExp.ProgrammeName,
                Venue: _socialWelExp.Venue,
                GuestName: _socialWelExp.GuestName,
                ExpenseAmount: _socialWelExp.ExpenseAmount,
                SocialWelExpDate: $filter('date')(_socialWelExp.Date, 'dd-MM-yyyy'),
                Time: _socialWelExp.Time,
                Attachment : _socialWelExp.Attachment
            };
            //  $scope.editData.fileName = _socialWelExp.Attachment;
           // $scope.editData.Attachment = _socialWelExp.Attachment;

            $scope.socialWelExpDate = _socialWelExp.Date;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM
   

        //BEGIN DELETE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.DeleteSocialWelExp = function (id) {
            $rootScope.IsAjaxLoading = true;
            SocialWelExpService.DeleteSocialWelExp(id).then(function (result) {
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
            document.location.href = "../../Handler/HRDSocialWelExp.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



      

        ///Model PopUp
        $scope.ShowSocialWelExp = function (SocialWelExp) {
            
          
            var modalInstance = $modal.open({
                templateUrl: 'SocialWelExpPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    SocialWelExpDetails: function () { return SocialWelExp; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, SocialWelExpDetails) {
            $scope.items = SocialWelExpDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
      
    }

})();

