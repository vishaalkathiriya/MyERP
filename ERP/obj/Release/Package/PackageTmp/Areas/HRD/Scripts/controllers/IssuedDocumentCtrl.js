/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("IssuedDocumentCtrl", [
            "$scope", "$rootScope", "$timeout", "IssuedDocumentService", "$http", "$filter", "ngTableParams", "$q",
            issuedDocumentCtrl
        ]);


    //Main controller function
    function issuedDocumentCtrl($scope, $rootScope, $timeout, IssuedDocumentService, $http, $filter, ngTableParams, $q) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.fileName = "";

        //Time
        $scope.mytime = new Date();

        //var dat = '';
        //$scope.mytime = $filter

        $scope.hstep = 1;
        $scope.mstep = 5;

        $scope.options = {
            hstep: [1, 2, 3],
            mstep: [1, 5, 10, 15, 25, 30]
        };

        $scope.ismeridian = false;
        $scope.toggleMode = function () {
            $scope.ismeridian = !$scope.ismeridian;
        };
        $scope.update = function () {
            var d = new Date();
            d.setHours(14);
            d.setMinutes(0);
            $scope.mytime = d;
        };
        //END TIME

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" },
            DocumentTypeId: 0
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
            $scope.mytime = null;
        };
        $scope.minDate = $scope.minDate || new Date();
        $scope.minDateForIssuedOn = $scope.minDate || new Date();
        //$scope.maxDate = $scope.maxDate || new Date();

        $scope.calendarOpenFromDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenToDate = false;
            $scope.calOpenIssueDate = false;
            $scope.calOpenFromDate = true;
        };

        $scope.calendarOpenToDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenFromDate = false;
            $scope.calOpenToDate = true;
            $scope.calOpenIssueDate = false;
        };

        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenFromDate = false;
            $scope.calOpenToDate = false;
            $scope.calOpenIssueDate = true;
        };

        $scope.$watch('fromDate', function (newValue) {
            $scope.editData.FromDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = $scope.StringToDateString($scope.editData.FromDate);
            if ($scope.CheckDate()) {
                $scope.editData.ToDate = $scope.editData.FromDate;
                $scope.toDate = $scope.StringToDateString($scope.editData.FromDate);
            }
        });
        $scope.$watch('editData.FromDate', function (newValue) {
            $scope.editData.FromDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = $scope.StringToDateString($scope.editData.FromDate);
            if ($scope.CheckDate()) {
                $scope.editData.ToDate = $scope.editData.FromDate;
                $scope.toDate = $scope.StringToDateString($scope.editData.FromDate);
            }
        });

        $scope.$watch('toDate', function (newValue) {
            $scope.editData.ToDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.$watch('issuedOn', function (newValue) {
            $scope.editData.IssuedOn = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateFromDate = function (fromDate) {
            if (!fromDate) {
                $scope.docform.txtFromDate.$setValidity("invalidFromDate", true);
                return;
            } else if (fromDate.length == 10) {
                if ($scope.ValidateDate(fromDate)) {
                    $scope.fromDate = $scope.StringToDateString(fromDate);
                    $scope.docform.txtFromDate.$setValidity("invalidFromDate", true);
                    if ($scope.CheckDate()) {
                        $scope.editData.ToDate = fromDate;
                        $scope.toDate = $scope.StringToDateString(fromDate);
                    }
                } else {
                    $scope.docform.txtFromDate.$setValidity("invalidFromDate", false);
                }
            } else {
                $scope.docform.txtFromDate.$setValidity("invalidFromDate", false);
            }
        };

        $scope.ValidateToDate = function (toDate) {
            if (!toDate) {
                $scope.docform.txtToDate.$setValidity("invalidToDate", true);
                return;
            } else if (toDate.length == 10) {
                if ($scope.ValidateDate(toDate)) {
                    $scope.toDate = $scope.StringToDateString(toDate);
                    $scope.docform.txtToDate.$setValidity("invalidToDate", true);

                    if ($scope.CheckDate()) {
                        $scope.editData.ToDate = $scope.editData.FromDate;
                        $scope.toDate = $scope.StringToDateString($scope.editData.FromDate);
                    }
                } else {
                    $scope.docform.txtToDate.$setValidity("invalidToDate", false);
                }
            } else {
                $scope.docform.txtToDate.$setValidity("invalidToDate", false);
            }
        };

        $scope.ValidateIssuedDate = function (issueDate) {
            if (!issueDate) {
                $scope.docform.txtIssuedOn.$setValidity("invalidIssuedDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issuedOn = $scope.StringToDateString(issueDate);
                    $scope.docform.txtIssuedOn.$setValidity("invalidIssuedDate", true);
                } else {
                    $scope.docform.txtIssuedOn.$setValidity("invalidIssuedDate", false);
                }
            } else {
                $scope.docform.txtIssuedOn.$setValidity("invalidIssuedDate", false);
            }
        };

        $scope.CheckDate = function () {
            if ($scope.editData.FromDate && $scope.editData.ToDate) {
                //var fromDate = new Date($filter('date')($scope.editData.FromDate, 'MM-dd-yyyy'));
                //var toDate = new Date($filter('date')($scope.editData.ToDate, 'MM-dd-yyyy'));
                var fDT = $scope.editData.FromDate.split('-');
                var tDT = $scope.editData.ToDate.split('-');

                var fDate = new Date(parseInt(fDT[2]), parseInt(fDT[1]) - 1, parseInt(fDT[0]));
                var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));

                if (fDate > tDate) {
                    return true;
                }
            }
            return false;
        }

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };
        // END DATE PICKER

        /*validate drop down*/
        $scope.validateDropDocument = function () {
            if ($scope.editData.DocumentTypeId && $scope.editData.DocumentTypeId != 0) return false;
            return true;
        };

        /* getting list of documents */
        function loadDocumentDrop() {
            IssuedDocumentService.RetrieveDocument().then(function (result) {
                $scope.DocumentList = result.data.DataList;
                $timeout(function () {
                    $scope.editData.DocumentTypeId = 0;
                });
            });
        };
        loadDocumentDrop();

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                HRDIssuedDocId: 0,
                ECode: "",
                FullName: "",
                DocumentTypeId: 0,
                FromDate: "",
                ToDate: "",
                AttachmentName: "",
                DepartmentName: "",
                IntercomNo: "",
                IssuedBy: "",
                IssuedOn: "",
                Remarks: ""
            };
            $scope.fileName = "";
            $scope.mytime = new Date(); //reset mytime
            $scope.editData.IssuedOn = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issuedOn = $scope.StringToDateString($scope.editData.IssuedOn);

            $scope.docform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new document click event*/
        $scope.AddDocument = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.IssuedOn = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issuedOn = $scope.StringToDateString($scope.editData.IssuedOn);
        };

        $scope.FilterByIssueDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        /*save document*/
        $scope.CreateUpdateDocument = function (doc) {
            $rootScope.IsAjaxLoading = true;
            doc.AttachmentName = $scope.fileName;

            var iDate = $filter('date')(doc.IssuedOn, 'dd-MM-yyyy').split('-');
            var iHour = $filter('date')($scope.mytime, 'HH-mm-ss').split('-');
            var idt = new Date(parseInt(iDate[2]), parseInt(iDate[1]) - 1, parseInt(iDate[0]), parseInt(iHour[0]), parseInt(iHour[1]), parseInt(iHour[2]));

            var fDate = $filter('date')(doc.FromDate, 'dd-MM-yyyy').split('-');
            var fdt = new Date(parseInt(fDate[2]), parseInt(fDate[1]) - 1, parseInt(fDate[0]), 0, 0, 0);

            var tDate = $filter('date')(doc.ToDate, 'dd-MM-yyyy').split('-');
            var tdt = new Date(parseInt(tDate[2]), parseInt(iDate[1]) - 1, parseInt(tDate[0]), 0, 0, 0);

            doc.IssuedOn = $filter('date')(idt, 'MM-dd-yyyy HH:mm:ss');
            doc.FromDate = $filter('date')(fdt, 'MM-dd-yyyy HH:mm:ss');
            doc.ToDate = $filter('date')(tdt, 'MM-dd-yyyy HH:mm:ss');

            IssuedDocumentService.CreateUpdateDocument(doc, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        ResetForm();
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.docform.$setPristine();
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

        /*reset the form*/
        $scope.ResetDocument = function () {
            if ($scope.mode == "Edit") {
                $scope.editData = {
                    HRDIssuedDocId: $scope.storage.lastRecord.HRDIssuedDocId,
                    ECode: $scope.storage.lastRecord.ECode,
                    FullName: $scope.storage.lastRecord.FullName,
                    DocumentTypeId: $scope.storage.lastRecord.DocumentTypeId,
                    FromDate: $filter('date')($scope.storage.lastRecord.FromDate, 'dd-MM-yyyy'),
                    ToDate: $filter('date')($scope.storage.lastRecord.ToDate, 'dd-MM-yyyy'),
                    AttachmentName: $scope.storage.lastRecord.AttachmentName,
                    DepartmentName: $scope.storage.lastRecord.DepartmentName,
                    IntercomNo: $scope.storage.lastRecord.IntercomNo,
                    IssuedBy: $scope.storage.lastRecord.IssuedBy,
                    IssuedOn: $filter('date')($scope.storage.lastRecord.IssuedOn, 'dd-MM-yyyy'),
                    Remarks: $scope.storage.lastRecord.Remarks
                };

                $scope.fileName = $scope.storage.lastRecord.AttachmentName;
                $scope.SetMyTime($scope.storage.lastRecord.IssuedOn);
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseDocument = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit document*/
        $scope.UpdateDocument = function (_doc) {
            $scope.storage.lastRecord = _doc;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                HRDIssuedDocId: _doc.HRDIssuedDocId,
                ECode: _doc.ECode,
                FullName: _doc.FullName,
                DocumentTypeId: _doc.DocumentTypeId,
                FromDate: $filter('date')(_doc.FromDate, 'dd-MM-yyyy'),
                ToDate: $filter('date')(_doc.ToDate, 'dd-MM-yyyy'),
                AttachmentName: _doc.AttachmentName,
                DepartmentName: _doc.DepartmentName,
                IntercomNo: _doc.IntercomNo,
                IssuedBy: _doc.IssuedBy,
                IssuedOn: $filter('date')(_doc.IssuedOn, 'dd-MM-yyyy'),
                Remarks: _doc.Remarks
            };

            $scope.fileName = _doc.AttachmentName;
            $scope.SetMyTime(_doc.IssuedOn);

            $scope.docform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        $scope.SetMyTime = function (date) {
            var iDate = $filter('date')(date, 'dd-MM-yyyy-HH-mm-ss').split('-');
            var fdt = new Date(parseInt(iDate[2]), parseInt(iDate[1]) - 1, parseInt(iDate[0]), parseInt(iDate[3]), parseInt(iDate[4]), parseInt(iDate[5]));
            $scope.mytime = fdt;
        };
        /*delete document*/
        $scope.DeleteDocument = function (id) {
            $rootScope.IsAjaxLoading = true;
            IssuedDocumentService.DeleteDocument(id).then(function (result) {
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

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDIssuedDocument.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };

        /*datatable*/
        $scope.RetrieveDocument = function () {
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
                    IssuedDocumentService.GetDocumentList($scope.filterDate.DocumentTypeId, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter()).then(function (result) {
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

        $scope.DocumentsLst = function (column) {
            var data;
            var def = $q.defer(),
              arr = [],
              names = [];
            IssuedDocumentService.RetrieveDocument().then(function (result) {
                angular.forEach(result.data.DataList, function (item) {
                    if (inArray(item.Id, arr) === -1) {
                        arr.push({ 'id': item.Id, 'title': item.Label });
                    }
                });
            });
            def.resolve(arr);
            return def;
        };
        var inArray = Array.prototype.indexOf ?
           function (val, arr) {
               return arr.indexOf(val)
           } :
           function (val, arr) {
               var i = arr.length;
               while (i--) {
                   if (arr[i] === val) return i;
               }
               return -1;
           }
    };

})();

