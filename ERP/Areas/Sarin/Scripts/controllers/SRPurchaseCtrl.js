/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRPurchaseCtrl", [
            "$scope", "$rootScope", "$timeout", "SRPurchaseService", "$http", "$filter", "ngTableParams", "$modal",
            SRPurchaseCtrl
        ]);


    //Main controller function
    function SRPurchaseCtrl($scope, $rootScope, $timeout, SRPurchaseService, $http, $filter, ngTableParams, $modal) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $('#txtRemarks').elastic();
        $scope.fileName = "";
        $scope.OriginPath = "../../" + window.originSarinPurchPath;
        $scope.tempPath = "../../" + window.tempSarinPurchPath;

        $scope.tempEdit = true;
        $scope.ImagePath = $scope.tempPath;
        $scope.isChanged = false;

        $scope.$watch('fileName', function (newValue, oldValue) {
            if ($scope.mode == "Edit") {
                if (oldValue) {
                    if (newValue != oldValue) {
                        $scope.ImagePath = $scope.tempPath;
                    }
                    else {
                        $scope.ImagePath = $scope.OriginPath;
                    }
                }
                else {
                    if ($scope.isChanged && newValue != null) {
                        $scope.ImagePath = $scope.tempPath;
                    }
                    else {
                        $scope.ImagePath = $scope.OriginPath;
                    }
                }
            }
            else {
                $scope.ImagePath = $scope.tempPath;
            }
        }, true);

        $scope.$watch('editData.PurchaseId', function (newValue, oldValue) {
            if ($scope.mode == "Edit") {
                if (oldValue) {
                    if (newValue != oldValue) {
                        $scope.ImagePath = $scope.OriginPath;
                    }
                }
            }
        })

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

        $scope.$watch('editdata.PurchaseDate', function (newvalue) {
            $scope.issueDate = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 
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
                $scope.purchaseform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.purchaseform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.purchaseform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.purchaseform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.PurchaseDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };
        // End DATE PICKER

        /*reset the form*/
        function ResetForm() {
            $scope.fileName = "";
            $scope.tempEdit = true;
            $scope.editData = {
                PurchaseId: 0,
                PartId: 0,
                PurchaseDate: "",
                Quantity: 0,
                ApprovedBy: "",
                Attachment: "",
                Remarks: ""
            };
            $scope.purchaseform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }


        /*validate Part dropdown*/
        $scope.validateDropPart = function () {
            if ($scope.editData.PartId && $scope.editData.PartId != 0) return false;
            return true;
        };


        /* getting list of types*/
        function loadParts() {
            SRPurchaseService.RetrieveSRParts().then(function (result) {
                $scope.SRParts = result.data.DataList;
            });
        };
        loadParts();

        /*add new SR-Purchase*/
        $scope.AddSRPurchase = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save SR-Purchase*/
        $scope.CreateUpdateSRPurchage = function (doc) {

            doc.Attachment = $scope.fileName;
            var IDate = $filter('date')(doc.PurchaseDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            doc.PurchaseDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            SRPurchaseService.CreateUpdateSRPurchage(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            PurchaseId: 0,
                            PartId: 0,
                            PurchaseDate: "",
                            Quantity: 0,
                            ApprovedBy: "",
                            Attachment: "",
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;

                        $scope.fileName = "";
                        $scope.tempEdit = true;


                        $scope.purchaseform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
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

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*cancel button click event*/
        $scope.CloseSRPurchase = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRPurchase = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.PurchaseId = $scope.storage.lastRecord.PurchaseId;
                $scope.editData.PartId = $scope.storage.lastRecord.PartId;
                $scope.editData.PurchaseDate = $filter('date')($scope.storage.lastRecord.PurchaseDate, 'dd-MM-yyyy')
                $scope.editData.Quantity = $scope.storage.lastRecord.Quantity;
                $scope.editData.ApprovedBy = $scope.storage.lastRecord.ApprovedBy;
                $scope.editData.Attachment = $scope.storage.lastRecord.Attachment;
                $scope.tempEdit = false;
                $scope.fileName = $scope.storage.lastRecord.Attachment;
                $scope.ImagePath = $scope.OriginPath;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit Purchase Entry*/
        $scope.UpdateSRPurchage = function (_pur) {

            $scope.storage.lastRecord = _pur;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.PartId = _pur.PartId;
            $scope.issueDate = _pur.PurchaseDate;
            $scope.editData.PurchaseDate = $filter('date')(_pur.PurchaseDate, 'dd-MM-yyyy');
            $scope.editData.Quantity = _pur.Quantity;
            $scope.editData.ApprovedBy = _pur.ApprovedBy;
            $scope.editData.Attachment = _pur.Attachment;
            $scope.editData.Remarks = _pur.Remarks;

            $scope.ImagePath = $scope.OriginPath;
            $scope.tempEdit = false;
            $scope.fileName = _pur.Attachment;
            $scope.isChanged = false;

            $scope.editData.PurchaseId = _pur.PurchaseId;

            $scope.purchaseform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-Purchase*/
        $scope.DeleteSRPurchase = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRPurchaseService.DeleteSRPurchase(id).then(function (result) {
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

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/SRPurchase.ashx?timezone=" + $scope.timeZone
        };

        ///Model PopUp
        $scope.ShowAttachment = function (Purchase) {
            var modalInstance = $modal.open({
                templateUrl: 'PurchasePopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    PurchaseDetails: function () { return Purchase; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, PurchaseDetails) {
            $scope.items = PurchaseDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };

        /*datatable*/
        $scope.RetrieveSRPurchase = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    PurchasDate: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRPurchaseService.GetSRPurchaseList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().PartName, params.filter().Quantity, params.filter().ApprovedBy).then(function (result) {
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
    };


})();

