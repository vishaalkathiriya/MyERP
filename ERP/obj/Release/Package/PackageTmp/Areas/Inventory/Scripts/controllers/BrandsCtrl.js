/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Controllers").controller("BrandsCtrl", [
    "$scope",
    "$rootScope",
    "$timeout",
    "BrandsService",
    "$http",
    "$filter",
    "ngTableParams",
    function ($scope, $rootScope, $timeout, BS, $http, $filter, ngTableParams) {

        $scope.editData = $scope.editData || {};
        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;
        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $scope.lastBrand = $scope.lastBrand || {};
        // GET TIME OFFSET IN MINUTES
        $scope.timeZone = new Date().getTimezoneOffset().toString();

        $scope.AddBrand = function () {
            $scope.mode = "Add";
            $scope.SaveText = "Save";
            $scope.editData = {};
            $scope.lastBrand = {};
            $rootScope.isFormVisible = true;
            $scope.editData.isActive = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            $scope.frmBrand.$setPristine();
        }

        $scope.CreateUpdateBrand = function (brand) {
            BS.CreateUpdateBrand(brand).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.editData = {
                            brandName: '',
                            isActive: true
                        };
                        $scope.RefreshTable();

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                        }
                        $scope.isFirstFocus = false;
                        $scope.frmBrand.$setPristine();
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

        $scope.RetriveBrands = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    BrandName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    BS.RetriveBrands($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().BrandName).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType != 0) { // 0:Error
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().BrandName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.Brands = result.data.DataList.result);
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
                }
            })
        }
        $scope.RetriveBrands();

        $scope.UpdateBrand = function (brand) {
            $scope.lastBrand = {
                BrandId: brand.BrandId,
                BrandName: brand.BrandName,
                IsActive: brand.IsActive
            };
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.SaveText = "Update";
            $scope.editData.brandId = brand.BrandId;
            $scope.editData.brandName = brand.BrandName;
            $scope.editData.isActive = brand.IsActive;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        $scope.DeleteBrand = function (brand) {
            BS.DeleteBrand(brand).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 0:Error
                        toastr.success('Your record is successfully deleted', 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.ResetBrand = function () {
            if ($scope.mode === "Add") {
                $scope.editData = {
                    brandName: '',
                    isActive: true
                }
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    brandId: $scope.lastBrand.BrandId,
                    brandName: $scope.lastBrand.BrandName,
                    isActive: $scope.lastBrand.IsActive
                }
            }
            $scope.frmBrand.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            $scope.isFirstFocus = false;
        }

        $scope.CloseBrand = function () {
            $scope.editData = {};
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
        }

        $scope.ChangeStatus = function (brand) {
            BS.ChangeStatus(brand).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
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
            document.location.href = "../../Handler/Brands.ashx?timezone=" + $scope.timeZone
        }
    }
]);