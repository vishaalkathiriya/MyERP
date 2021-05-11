/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DocumentCtrl", [
            "$scope", "$rootScope", "$timeout", "DocumentService", "$http", "$filter", "ngTableParams",
            documentCtrl
        ]);


    //Main controller function
    function documentCtrl($scope, $rootScope, $timeout, DocumentService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                Documents: "",
                DocumentTypeId: 0,
                IsActive: true
            };
            //$scope.isChecked = true;
            $scope.docform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        $scope.validateDropDocumentType = function () {
            if ($scope.editData.DocumentTypeId && $scope.editData.DocumentTypeId != 0) return false;
            return true;
        };

        $scope.GetDocumentTypeList = function () {
            DocumentService.GetDocumentTypeList().then(function (result) {
                $scope.DocumentType = result.data.DataList;
                $timeout(function () {
                    $scope.editData.DocumentTypeId = 0;
                });
            });
        };
        $scope.GetDocumentTypeList();

        /*add new document click event*/
        $scope.AddDocument = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save document*/
        $scope.CreateUpdateDocument = function (doc) {
            DocumentService.CreateUpdateDocument(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { Documents: '', DocumentTypeId: 0, IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.docform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    }else if (result.data.MessageType == 2) { // 2:Warning
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
            //$rootScope.IsAjaxLoading = false;
        };

        /*active/inactive document*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            DocumentService.ChangeStatus(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*reset the form*/
        $scope.ResetDocument = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Documents = $scope.storage.lastRecord.Documents;
                $scope.editData.DocumentTypeId = $scope.storage.lastRecord.DocumentTypeId;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
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

            $scope.editData.Id = _doc.Id;
            $scope.editData.Documents = _doc.Documents;
            $scope.editData.DocumentTypeId = _doc.DocumentTypeId;
            $scope.editData.IsActive = _doc.IsActive;
            $scope.docform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete document*/
        $scope.DeleteDocument = function (id) {
            $rootScope.IsAjaxLoading = true;
            DocumentService.DeleteDocument(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'This record is in use');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Document.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveDocument = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Documents: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    DocumentService.GetDocumentList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Documents).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().Documents;
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


        /*delete selected records - COMMENTED FOR FUTRE USE IF NEEDED IN OTHER FORMS*/
        /*$scope.deleteAll = function () {
            var myArray = [];
            jQuery.each($scope.checkboxes.items, function (i, val) {
                if (val == true) {
                    myArray.push(i);
                }
            });

            if (myArray.length > 0) {
                DocumentService.deleteAll(myArray).then(function (result) {
                    if (result.data == "true") {
                        toastr.success('Your record(s) is successfully deleted', 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error('An error occured while processing your request', 'Opps, Something went wrong');
                    }
                });
            } else {
                toastr.info('please select something for delete', 'Information');
            }
        };*/

        /*
        $scope.checkboxes = { 'checked': false, items: {} };

        // watch for data checkboxes
        $scope.$watch('checkboxes.items', function (values) {
            if (!$scope.documents) {
                return;
            }
            var checked = 0, unchecked = 0,
                total = $scope.documents.length;
            angular.forEach($scope.documents, function (item) {
                checked += ($scope.checkboxes.items[item.Id]) || 0;
                unchecked += (!$scope.checkboxes.items[item.Id]) || 0;
            });
            if ((unchecked == 0) || (checked == 0)) {
                $scope.checkboxes.checked = (checked == total);
            }
            // grayed checkbox
            angular.element(document.getElementById("select_all")).prop("indeterminate", (checked != 0 && unchecked != 0));
        }, true);*/
    };
   

})();

