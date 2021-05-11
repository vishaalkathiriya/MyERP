/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Controllers").controller("SubCategoriesCtrl", [
    "$scope",
    "$rootScope",
    "$timeout",
    "SubCategoriesService",
    "$filter",
    "ngTableParams",
    function ($scope, $rootScope, $timeout, SCS, $filter, ngTableParams) {

        $scope.editData = $scope.editData || {};
        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;
        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $scope.lastSubCategory = $scope.lastSubCategory || {};
        // GET TIME OFFSET IN MINUTES
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.filterData = {};
        $scope.topFilter = 0;

        // BEGIN GETTING LIST OF CATEGORIES
        SCS.RetrieveCategories().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.Categories = result.data.DataList;
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }

        })
        // END GETTING LIST OF CATEGORIES

        $scope.AddSubCategory = function () {

            $scope.mode = "Add";
            $scope.editData = {};
            $scope.lastSubCategory = {};
            $rootScope.isFormVisible = true;
            $scope.editData.CategoryId = 0;
            $scope.editData.IsActive = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            $scope.frmSubCategory.$setPristine();
        }

        $scope.CreateUpdateSubCategory = function (subCategory) {
            SCS.CreateUpdateSubCategory(subCategory).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.editData = {
                            CategoryId: 0,
                            SubCategoryName: '',
                            IsActive: true
                        };
                        $scope.RefreshTable();

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                        }
                        $scope.isFirstFocus = false;
                        $timeout(function () {
                            $scope.isFirstFocus = true;
                        });
                        $scope.frmSubCategory.$setPristine();
                        if ($scope.mode == "Add") {
                            toastr.success(result.data.Message, 'Success');
                        }
                        else if ($scope.mode == "Edit") {
                            toastr.success(result.data.Message, 'Success');
                        }
                    } else if (result.data.MessageType === 2) {
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }
        
        $scope.RetrieveSubCategories = function () {

            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    CategoryName: 'asc',
                    SubCategoryName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SCS.RetrieveSubCategories($scope.topFilter, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().CategoryName, params.filter().SubCategoryName).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    //$scope.filterText = params.filter().SubCategoryName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.SubCategories = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }

                    });
                },
                $scope: { $data: {} }
            })
        }

        /*filter data*/
        $scope.FilterByCategory = function (filter) {
            $scope.topFilter = filter.CategoryId;
            $scope.RefreshTable();
        };

        $scope.UpdateSubCategory = function (subCategory) {
            $scope.lastSubCategory = {
                CategoryId: subCategory.CategoryId,
                SubCategoryId: subCategory.SubCategoryId,
                SubCategoryName: subCategory.SubCategoryName,
                IsActive: subCategory.IsActive
            };
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.SaveText = "Update";
            $scope.editData.CategoryId = subCategory.CategoryId;
            $scope.editData.SubCategoryId = subCategory.SubCategoryId;
            $scope.editData.SubCategoryName = subCategory.SubCategoryName;
            $scope.editData.IsActive = subCategory.IsActive;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        $scope.DeleteSubCategory = function (subCategory) {
            SCS.DeleteSubCategory(subCategory).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.ResetSubCategory = function () {
            if ($scope.mode === "Add") {
                $scope.editData = {
                    CategoryId: 0,
                    SubCategoryId: 0,
                    SubCategoryName: '',
                    IsActive: true
                }
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    CategoryId: $scope.lastSubCategory.CategoryId,
                    SubCategoryId: $scope.lastSubCategory.SubCategoryId,
                    SubCategoryName: $scope.lastSubCategory.SubCategoryName,
                    IsActive: $scope.lastSubCategory.IsActive
                }
            }
            $scope.frmSubCategory.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            $scope.isFirstFocus = false;
        }

        $scope.CloseSubCategory = function () {
            $scope.editData = {};
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
        }

        $scope.ChangeStatus = function (subCategory) {
            SCS.ChangeStatus(subCategory).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        $scope.ExportToExcel = function () {
            var catId = 0;
            if (!$scope.filterData.CategoryId) {
                catId = 0;
            } else {
                catId = $scope.filterData.CategoryId;
            }
            document.location.href = "../../Handler/SubCategories.ashx?timezone=" + $scope.timeZone + "&category=" + catId;
        }
    }
]);