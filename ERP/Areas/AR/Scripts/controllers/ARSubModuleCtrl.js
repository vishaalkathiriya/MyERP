/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ARSubModuleCtrl", [
            "$scope", "$rootScope", "$timeout", "ARSubModuleService", "$http", "$filter","$q","ngTableParams",
            subModuleCtrl
        ]);


    //Main controller function
    function subModuleCtrl($scope, $rootScope, $timeout, ARSubModuleService, $http, $filter, $q, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.PermissionList = [];

        /*validate drop down*/
        $scope.validateModuleName = function () {
            if ($scope.editData.ModuleId && $scope.editData.ModuleId != 0) return false;
            return true;
        };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                SubModuleId: 0,
                ModuleId: 0,
                SubModuleName: "",
                URL: "",
                SeqNo:1,
                IsActive: true
            };

            $scope.PermissionList = [];
            $scope.smform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*add new sub module*/
        $scope.AddSubModule = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*change sequence number from grid*/
        $scope.isEdit = [];
        $scope.oldSeqNo = [];
        $scope.ChangeSeqNo = function (seqNo, subModuleId, index) {
            ARSubModuleService.UpdateSequenceNo(seqNo, subModuleId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.isEdit[index] = false;
                        $scope.oldSeqNo[index] = 0;
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

        $scope.CancelSeqChange = function (index, $data) {
            $scope.isEdit[index] = false;
            $data[index].SeqNo = $scope.oldSeqNo[index];
            $scope.oldSeqNo[index] = 0;
        };

        /*getting list of module dropdown*/
        function loadModuleDrop() {
            ARSubModuleService.GetModuleList().then(function (result) {
                $scope.ModuleList = result.data.DataList;
                $timeout(function () {
                    $scope.editData.ModuleId = 0; //select default option
                });
            });
        };
        loadModuleDrop();

        /*bind access rights combo*/
        function RetrievePermission() {
            var isDashboard = false;
            var isEmployee = false;
            var isInvoice = false;
            $scope.isAllSelected = false;
            $scope.SelectText = "Select All";
            if ($scope.editData.ModuleId == 1 && ($scope.editData.SubModuleName == "Dashboard" || $scope.editData.SubModuleName == "dashboard")) {
                isDashboard = true;
            }
            if (angular.lowercase($scope.editData.SubModuleName) == "manage employee") {
                isEmployee = true;
            } else if (angular.lowercase($scope.editData.SubModuleName) == "conversation") {
                isInvoice = true;
            }
            return ARSubModuleService.GetAccessPermissionList(isDashboard, isEmployee, isInvoice);
        }
        $scope.loadAccessPermissionDrop = function () {
            RetrievePermission().then(function (result) {
                $scope.PermissionList = result.data.DataList;
            });
        };

        /*convert array of selected list to comma separated string*/
        function GetSelectedAccess(callback) {
            var prom = "";
            angular.forEach($scope.PermissionList, function (value, key) {
                if (value.IsSelected) {
                    prom += value.Id + ",";
                }
            });

            $q.all(prom).then(function () {
                callback(prom);
            });
        };

        /*select all checkbox*/
        $scope.selectAll = function (selected) {
            if (selected) {
                $scope.SelectText = "Select All";
                angular.forEach($scope.PermissionList, function (value, key) {
                    $scope.PermissionList[key].IsSelected = false;
                });
            } else {
                $scope.SelectText = "DeSelect All";
                angular.forEach($scope.PermissionList, function (value, key) {
                    $scope.PermissionList[key].IsSelected = true;
                });
            }
        };

        /*active/inactive sub module*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            ARSubModuleService.ChangeStatus(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
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

        /*delete sub module*/
        $scope.DeleteSubModule = function (id) {
            $rootScope.IsAjaxLoading = true;
            ARSubModuleService.DeleteSubModule(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
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

        /*reset the form*/
        $scope.ResetSubModule = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.SubModuleId = $scope.storage.lastRecord.SubModuleId;
                $scope.editData.ModuleId = $scope.storage.lastRecord.ModuleId;
                $scope.editData.SubModuleName = $scope.storage.lastRecord.SubModuleName;
                $scope.editData.URL = $scope.storage.lastRecord.URL;
                $scope.editData.SeqNo = $scope.storage.lastRecord.SeqNo;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;

                //bind and select permission list
                RetrievePermission().then(function (result) {
                    $scope.PermissionList = result.data.DataList;
                    angular.forEach($scope.storage.lastRecord.AllowedAccess, function (value, key) {
                        $scope.PermissionList[key].IsSelected = value.IsSelected;
                    });
                });
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseSubModule = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit sub module*/
        $scope.UpdateSubModule = function (_sub) {
            $scope.storage.lastRecord = _sub;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.SubModuleId = _sub.SubModuleId;
            $scope.editData.ModuleId = _sub.ModuleId;
            $scope.editData.SubModuleName = _sub.SubModuleName;
            $scope.editData.URL = _sub.URL;
            $scope.editData.SeqNo = _sub.SeqNo;
            $scope.editData.IsActive = _sub.IsActive;

            //bind and select permission list
            RetrievePermission().then(function (result) {
                $scope.PermissionList = result.data.DataList;
                angular.forEach(_sub.AllowedAccess, function (value, key) {
                    $scope.PermissionList[key].IsSelected = value.IsSelected;
                });
            });

            $scope.smform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*save sub module*/
        $scope.CreateUpdateSubModule = function (data) {
            $rootScope.IsAjaxLoading = true;
            
            GetSelectedAccess(function (lstIds) {//callback
                data.AllowedAccess = lstIds;

                console.log(data.AllowedAccess);
                ARSubModuleService.CreateUpdateSubModule(data, $scope.timeZone).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            ResetForm();
                            $scope.RefreshTable();
                            $scope.storage.lastRecord = {};
                            $scope.isFirstFocus = false;
                            $scope.smform.$setPristine();
                            if ($scope.mode === "Edit") {
                                $rootScope.isFormVisible = false;
                                $scope.saveText = "Save";
                            }
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            });
        };

        /*datatable*/
        $scope.RetrieveSubModules = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    SubModuleName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    ARSubModuleService.GetSubModuleList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter()).then(function (result) {
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

        $scope.ModuleLst = function (column) {
            var data;
            var def = $q.defer(),
              arr = [],
              names = [];

            ARSubModuleService.GetModuleList().then(function (result) {
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

