/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("TechnologyGroupCtrl", [
            "$scope", "$rootScope", "$timeout", "TechnologyGroupService", "$http", "$filter", "ngTableParams",
            technologyGroupCtrl
        ]);


    //Main controller function
    function technologyGroupCtrl($scope, $rootScope, $timeout, TechGroupService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onTechnologyTabSelected', function () {
            $rootScope.isTechnologyGroupFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isTechnologyGroupFormVisible = false;
        });


        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                TechnologiesGroup: "",
                IsActive: true
            };
            $scope.techform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new tech group click event*/
        $scope.AddTechGroup = function () {
            $rootScope.isTechnologyGroupFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save technology group*/
        $scope.CreateUpdateTechGroup = function (tech) {
            TechGroupService.CreateUpdateTechGroup(tech).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { TechnologiesGroup: '', IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.techform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isTechnologyGroupFormVisible = false;
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

        /*active/inactive tech group*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            TechGroupService.ChangeStatus(id, status).then(function (result) {

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
        $scope.ResetTechGroup = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.TechnologiesGroup = $scope.storage.lastRecord.TechnologiesGroup;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseTechGroup = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isTechnologyGroupFormVisible = false;
        };

        /*get record for edit tech group*/
        $scope.UpdateTechGroup = function (_tech) {
            $scope.storage.lastRecord = _tech;
            $rootScope.isTechnologyGroupFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _tech.Id;
            $scope.editData.TechnologiesGroup = _tech.TechnologiesGroup;
            $scope.editData.IsActive = _tech.IsActive;
            $scope.techform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete tech group*/
        $scope.DeleteTechGroup = function (id) {
            $rootScope.IsAjaxLoading = true;
            TechGroupService.DeleteTechGroup(id).then(function (result) {
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
            document.location.href = "../../Handler/TechnologyGroup.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveTechGroup = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    TechnologiesGroup: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    TechGroupService.GetTechGroupList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TechnologiesGroup).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().TechnologiesGroup;
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

