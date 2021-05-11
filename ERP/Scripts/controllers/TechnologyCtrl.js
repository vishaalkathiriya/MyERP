/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("TechnologyCtrl", [
            "$scope", "$rootScope", "$timeout", "TechnologyService", "$http", "$filter", "ngTableParams",
            technologyCtrl
        ]);


    //Main controller function
    function technologyCtrl($scope, $rootScope, $timeout, TechnologyService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.filterData = {};
        $scope.topFilter = 0;

        $rootScope.$on('onTechnologyGroupTabSelected', function () {
            $rootScope.isFormVisible= false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isFormVisible= false;
        });

        /*validate dropdown*/
        $scope.validateDrop = function () {
            if ($scope.editData.TechnologyGroupId && $scope.editData.TechnologyGroupId != 0) return false;
            return true;
        };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                Id: 0,
                TechnologyGroupId: 0,
                Technologies: "",
                IsActive: true
            };
            $scope.techform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of technology groups*/
        function loadTechnologyDrop() {
            TechnologyService.RetrieveTechGroup().then(function (result) {
                $scope.TechnologyGroup = result.data.DataList;
                $scope.filterData.TechnologyGroupId = 0; //select default value of top filter dropdown
            });
        };
        loadTechnologyDrop();

        /*filter data*/
        $scope.FilterByTechGroup = function (filter) {
            $scope.topFilter = filter.TechnologyGroupId;
            $scope.RefreshTable();
        };

        /*add new technology */
        $scope.AddTechnology = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save technology */
        $scope.CreateUpdateTechnology = function (tech) {
            $scope.editData.TechnologiesGroupId = $scope.editData.TechnologyGroupId;
            TechnologyService.CreateUpdateTechnology(tech).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { Id: 0, TechnologyGroupId: 0, Technologies: '', IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.techform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) {//2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else if (result.data.MessageType == 0) {//0:Error
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

        /*active/inactive technology*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            TechnologyService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetTechnology = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Id = $scope.storage.lastRecord.Id;
                $scope.editData.TechnologyGroupId = $scope.storage.lastRecord.TechnologiesGroupId;
                $scope.editData.Technologies = $scope.storage.lastRecord.Technologies;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseTechnology = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit technology*/
        $scope.UpdateTechnology = function (_tech) {
            $scope.editData.techGroup = $scope.TechnologyGroup[0];
            $scope.storage.lastRecord = _tech;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _tech.Id;
            $scope.editData.Technologies = _tech.Technologies;
            $scope.editData.TechnologyGroupId = _tech.TechnologiesGroupId;
            $scope.editData.IsActive = _tech.IsActive;
            $scope.techform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete tech group*/
        $scope.DeleteTechnology = function (id) {
            $rootScope.IsAjaxLoading = true;
            TechnologyService.DeleteTechnology(id).then(function (result) {
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
            document.location.href = "../../Handler/Technology.ashx?timezone=" + $scope.timeZone + "&TGroupId=" + $scope.filterData.TechnologyGroupId
        };

        /*datatable*/
        $scope.RetrieveTechnology = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Technologies: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    TechnologyService.GetTechnologyList($scope.topFilter, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Technologies, params.filter().TechnologiesGroup).then(function (result) {
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

