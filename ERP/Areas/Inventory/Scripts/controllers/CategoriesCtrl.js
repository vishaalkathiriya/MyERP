/// <reference path="../libs/angular/angular.js" />
(function () {

    'use strict';

    angular.module("ERPApp.Controllers").controller("CategoriesCtrl", [
        "$scope",
        "$rootScope",
        "$timeout",
        "CategoryService",
        "$filter",
         "ngTableParams",
        function ($scope, $rootScope, $timeout, CS, $filter, ngTableParams) {
            $scope.editData = $scope.editData || {};
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
            $scope.lastCategory = $scope.lastCategory || {};
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

            //load Category
            $scope.loadAllCategories = function () {
                //CS.getCategoryList($scope.timeZone).then(function (result) {
                //    $scope.values = result.data;
                //});
                $scope.tableParams = new ngTableParams({
                    page: 1,
                    count: 10,  // Total No. of records per page
                    sorting: {
                        CategoryName: 'asc'
                    },
                    defaultSort: 'asc'
                }, {
                    total: 0,
                    filterDelay: 750,
                    getData: function ($defer, params) {
                        $rootScope.IsAjaxLoading = true;
                        CS.GetCategoryList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().CategoryName).success(function (result) {
                            ////order by
                            //var tableData = params.sorting() ?
                            //    $filter('orderBy')(result.data.result, params.orderBy()) :
                            //    result.data.result;
                            if (result.IsValidUser) {
                                //display no data message
                                if (result.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().CategoryName;
                                } else {
                                    $scope.noRecord = false;
                                }

                                params.total(result.DataList.total);
                                $defer.resolve(result.DataList = result.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            } else {
                                $rootScope.redirectToLogin();
                            }
                        });
                    }
                });
            }

            /*add new Category click event*/
            $scope.addCategory = function () {
                $scope.mode = "Add";
                $scope.saveText = "Save";
                $scope.editData = {};
                $scope.lastCategory = {};
                $rootScope.isFormVisible = true;
                $scope.editData.IsActive = true;
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
                $scope.catform.$setPristine();
                //$scope.catNameTarget.focus();

            };

            //Create and Update Category
            $scope.SaveCategory = function (category) {
                CS.SaveCategory(category).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            //if (result.data === "true") {
                            toastr.success(result.data.Message, 'Success');
                            $scope.editData = { CategoryName: '', IsActive: true };
                            $scope.refreshTable();
                            $scope.lastCategory = {};
                            $scope.isFirstFocus = false;
                            $scope.catform.$setPristine();
                            if ($scope.mode === "Edit") {
                                $rootScope.isFormVisible = false;
                                $scope.saveText = "Save";
                            }
                            else {
                                $scope.SetFocus();
                            }

                            //} else {
                            //    toastr.error('This entry is already exists or having some problem while entering the data', 'Opps, Something went wrong');
                            //}
                        } else if (result.data.MessageType == 2) { // 2:Warning
                            toastr.warning(result.data.Message, 'Record already exists');
                            $scope.SetFocus();

                        } else if (result.data.MessageType == 0) {//0:Error{
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            }

            $scope.EditCategory = function (category) {
                $scope.lastCategory = category;
                $rootScope.isFormVisible = true;
                $scope.mode = "Edit";
                $scope.saveText = "Update";
                $scope.editData.CategoryId = category.CategoryId;
                $scope.editData.CategoryName = category.CategoryName;
                $scope.editData.IsActive = category.IsActive;
                $scope.catform.$setPristine();
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
                //$scope.isFirstFocus = false;
            }

            /*delete Category*/
            $scope.DeleteCategory = function (category) {
                CS.DeleteCategory(category).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.cancel();
                            $scope.refreshTable();
                        } else if (result.data.MessageType == 2) { // 1:Warning
                            toastr.warning(result.data.Message, 'Warning!');
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            }

            /* Change the Status of Category */
            $scope.IsActive = function (category) {
                CS.IsActive(category).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 0:Error
                            toastr.success('Status changed successfully.', 'Success');
                            $timeout(function () {
                                $scope.cancel();
                                $scope.refreshTable();
                            }, 50);
                        } else if (result.data.MessageType == 2) { // 1:Warning
                            toastr.warning(result.data.Message, 'Warning!');
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            };

            /*reset the form*/
            $scope.reset = function () {
                $scope.category = $scope.lastCategory;
                if ($scope.category.CategoryId) {
                    $scope.EditCategory($scope.category);
                }
                else {
                    $scope.editData = {
                        CategoryName: '',
                        IsActive: true
                    };
                    $scope.saveText = "Save";
                    $scope.catform.$setPristine();
                    $scope.isFirstFocus = false;
                    $timeout(function () {
                        $scope.isFirstFocus = true;
                    });
                    //$scope.isFirstFocus = false;
                }
            }

            /*cancel button click event*/
            $scope.cancel = function () {
                $scope.editData = {};
                $scope.mode = "Add";
                $rootScope.isFormVisible = false;
                $scope.isFirstFocus = false;
            };

            $scope.localDate = function (utcDate) {
                var newDate = new Date(date.getTime());
                var offset = date.getTimezoneOffset() / 60;
                var hours = date.getHours();
                newDate.setHours(hours - offset);
                return newDate.toLocaleString();
            };

            /*delete selected records*/
            $scope.deleteAll = function () {
                var myArray = [];
                jQuery.each($scope.checkboxes.items, function (i, val) {
                    if (val == true) {
                        myArray.push(i);
                    }
                });

                if (myArray.length > 0) {
                    CS.deleteAll(myArray).then(function (result) {
                        if (result.data == "true") {
                            toastr.success('Your record(s) is successfully deleted', 'Success');
                            $scope.refreshTable();
                        } else {
                            toastr.error('An error occured while processing your request', 'Opps, Something went wrong');
                        }
                    });
                } else {
                    toastr.info('please select something for delete', 'Information');
                }
            };

            $scope.checkboxes = { 'checked': false, items: {} };

            // watch for check all checkbox
            $scope.$watch('checkboxes.checked', function (value) {
                angular.forEach($scope.documents, function (item) {
                    if (angular.isDefined(item.Id)) {
                        $scope.checkboxes.items[item.Id] = value;
                    }
                });
            });
            $scope.ExportToExcel = function () {
                document.location.href = "../../Handler/Categories.ashx?timezone=" + $scope.timeZone
            };

            /*refresh table after */
            $scope.refreshTable = function () {
                $scope.tableParams.reload();
            };

            // Reset Focus
            $scope.SetFocus = function () {
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            }
        }
    ])

})();