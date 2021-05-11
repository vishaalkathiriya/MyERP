/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';

    //Define Controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpDocumentCtrl", [
            "$scope","$modal", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter",
            EmpDocumentCtrl
        ]);

    //Main controller function

    function EmpDocumentCtrl($scope, $modal, $rootScope, $timeout, EmployeeCreateService, $http, $filter) {

        $scope.documents = $scope.documents || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFirstFocus = false;
        $scope.storage = { lastRecord: "" };
        $scope.documents.IsActive = true;
        $scope.DocFileName = "";
        $scope.isMasterActive = true;
        $scope.OriginPath = window.Origin;
        $scope.tempPath = window.temp;

        $scope.$watch('DocFileName', function () {
            $scope.uploadFileType = $scope.DocFileName.split('.')[1] === "pdf" ? "pdf" : "image";
        });

        $scope.tempEdit = false;

        $scope.ImagePath = $scope.tempPath;
        $scope.isChanged = false;

        /*validate dropdown*/
        $scope.validateDropDDocument = function () {
            if ($scope.documents.DocumentId && $scope.documents.DocumentId != 0) return false;
            return true;
        };
        $scope.validateImage = function () {
            if ($scope.DocFileName || $scope.DocFileName == "") return false;
            return true;
        };
        /* getting list of Documents from Document Table */
        function loadDocumentDrop() {
            EmployeeCreateService.GetActiveDocuments().then(function (result) {
                $scope.DDocument = result.data;
                $timeout(function () {
                    $scope.documents.DocumentId = 0;
                });
            });
        };
        loadDocumentDrop();


        /*reset the form*/
        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        /*save document*/
        $scope.CreateUpdateEmpDocument = function (q) {
            q.EmployeeId = $scope.master.EmployeeId;
            q.FileName = $scope.DocFileName;
            EmployeeCreateService.CreateUpdateEmpDocument(q).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.storage.lastRecord = {};
                        $scope.clearControl();

                        $scope.RetrieveEmpDocuments();
                        $scope.isFirstFocus = false;
                        $scope.empDform.$setPristine();
                        if ($scope.mode === "Edit") {
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

        /*active/inactive document*/
        $scope.ChangeStatus = function (document) {
            EmployeeCreateService.ChangeDocStatus(document).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.clearControl();
                        $scope.RetrieveEmpDocuments();
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
        $scope.ResetEmpDocument = function () {
            $scope.documents = $scope.storage.lastRecord;
            if ($scope.documents.SrNo) {
                $scope.tempEdit = false;
                $scope.UpdateEmpDocument($scope.documents);
            } else {
                $scope.clearControl();
            }
        };

        /*Clear all controls*/
        $scope.clearControl = function () {
            $scope.documents = {
                SrNo: 0,
                DocumentId: 0,
                FileName: '',
                IsActive: true,
            };
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.DocFileName = "";
            $scope.tempEdit = false;
            $scope.storage.lastRecord = {};
            $scope.empDform.$setPristine();
            $scope.SetFocus();
        };
        $scope.UpdateEmpDocument = function (_empDoc) {
            $scope.storage.lastRecord = _empDoc;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.documents.SrNo = _empDoc.SrNo;
            $scope.documents.DocumentId = _empDoc.DocumentId;
            $scope.ImagePath = $scope.OriginPath;
            $scope.DocFileName = _empDoc.FileName;
            $scope.documents.IsActive = _empDoc.IsActive;

            $scope.empDform.$setPristine();
            $scope.tempEdit = true;
            $scope.SetFocus();
        };

        $scope.$watch('DocFileName', function (newValue, oldValue) {
            if ($scope.tempEdit == false) {
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
                        } else {
                            $scope.ImagePath = $scope.OriginPath;
                        }
                    }
                }
                else {
                    $scope.ImagePath = $scope.tempPath;
                }
            }
            else {
                $scope.ImagePath = $scope.OriginPath;
                $scope.tempEdit = false;
            }
        }, true);

        $scope.setThumbPath = function (thumbPath) {
            $scope.OriginPath = thumbPath;
        };

        /*delete document*/
        $scope.DeleteEmpDoc = function (id) {
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.DeleteEmpDoc(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1: Success 
                        toastr.success(result.data.Message, 'Success');
                        $scope.clearControl();
                        $scope.RetrieveEmpDocuments();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else    //0:Error
                    {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*getting list of employee document*/
        $scope.RetrieveEmpDocuments = function () {
            $scope.clearControl();
            $rootScope.IsAjaxLoading = true;
            $scope.isRecord = false;
            EmployeeCreateService.GetEmpDocumentList($scope.master.EmployeeId, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        $scope.docList = result.data.DataList;
                        $scope.imageList = [];
                        angular.forEach($scope.docList, function (value, key) {
                            $scope.docList[key].FileType = value.FileName.split('.')[1];
                            if ($scope.docList[key].FileType != 'pdf') {
                                $scope.imageList.push({ FileName: value.FileName, CaptionText: value.tblDocument.Documents });
                            }
                        });
                    } else {
                        toastr.error(result.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }


        /*MODEL FOR IMAGE SHOW*/
        $scope.ImageShow = function (iList, index) {
            var modalInstance = $modal.open({
                templateUrl: 'Slider.html',
                controller: ModalImageCtrl,
                scope: $scope,
                resolve: {
                    data: function () { return ({ list: iList, index: index }); } //return anything that you want to pass to model
                }
            });
        };
    };

    // BEGIN MODAL IMAGE CONTROLLER
    var ModalImageCtrl = function ($scope, $modalInstance, data) {
        $scope.slideImages = data.list;
        $scope.imgCurrentIndex = data.index;

        $scope.callback = function () {
            $modalInstance.close();
        };
    };
})();