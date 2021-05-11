/// <reference path="../libs/angular/angular.js" />

(function () {

    angular.module("ERPApp.Controllers").controller("InventoriesListCtrl", [
    "$scope",
    "$modal",
    "$rootScope",
    "$timeout",
    "InventoriesService",
    "$filter",
    "ngTableParams",
    "$q",
    InventoriesCtrl
    ]);

    function InventoriesCtrl($scope, $modal, $rootScope, $timeout, IS, $filter, ngTableParams, $q) {

        // BEGIN VARIABLES
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.filters = $scope.filters || { VendorId: 0, LocationId: 0, BrandId: 0, CategoryId: 0, SubCategoryId: 0 };
        $scope.isDetailGridVisibleForPopup = true;
        // END VARIABLES

        // BEGIN RETRIEVE DATA FOR COMBOBOX
        IS.RetrieveVendors().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.vendors = result.data.DataList;
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }
        });
        IS.RetrieveLocations().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.locations = result.data.DataList;
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }
        });
        IS.RetrieveBrands().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.brands = result.data.DataList;
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }
        });
        IS.RetrieveCategories().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.categories = result.data.DataList;
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }
        });
        $scope.RetrieveSubCategories = function (CategoryId) {
            IS.RetrieveSubCategories(CategoryId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.subCategories = result.data.DataList;
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
        // END RETRIEVE DATA FOR COMBOBOX

        // BEGIN INVENTORY
        $scope.FilterData = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        }
        $scope.AddInventory = function () {
            window.location.href = "Inventories/Create";
        }
        $scope.RetrieveInventories = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    InventoryName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    IS.RetrieveInventories($scope.filters, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().InventoryName, params.filter().IssueTo).then(function (result) {
                        $scope.Inventories = result.data.DataList.result;
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    //$scope.filterText = params.filter().InventoryName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.Inventories = result.data.DataList.result);
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
        var popupInventoryDetailList = {
            mainDetail: {},
            childDetails: []
        };
        $scope.popupInventoryDetails = function (inventory) {
            return IS.ShowInventory(inventory).then(function (result) {
                popupInventoryDetailList.mainDetail = result;
                return IS.ShowInventoryDetail(result.DataList.InventoryId, $scope.timeZone).then(function (result) {
                    popupInventoryDetailList.childDetails = result.DataList;
                    return popupInventoryDetailList;
                });
            });
        }
        $scope.ShowInventory = function (inventory) {
            var modalInstance = $modal.open({
                templateUrl: 'InventoryDetailsPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    inventoryDetails: function () { return $scope.popupInventoryDetails(inventory); }
                }
            });
        }
        $scope.UpdateInventory = function (inventory) {
            window.location.href = "./Inventories/Create/" + inventory.InventoryId;
        }
        $scope.DeleteInventory = function (inventory) {
            IS.DeleteInventory(inventory).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }
        $scope.CloseInventory = function () {
            $scope.masterData = [];
            $scope.detailData = [];
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            $scope.isDetailFormVisible = false;
            $scope.isFirstFocus = false;
        }
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        }
        $scope.UploadFromExcel = function () {
            var modalInstance = $modal.open({
                templateUrl: 'UploadExcel.html',
                controller: UploadExcelCtrl
            });
        }
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Inventories.ashx?timezone=" + $scope.timeZone + "&vendor=" + $scope.filters.VendorId + "&location=" + $scope.filters.LocationId + "&brand=" + $scope.filters.BrandId + "&category=" + $scope.filters.CategoryId + "&subcategory=" + $scope.filters.SubCategoryId
        }
        // END INVENTORY
    }

    // BEGIN MODAL INSTANCE CONTROLLER
    var ModalInstanceCtrl = function ($scope, $modalInstance, inventoryDetails) {
        $scope.items = inventoryDetails;
        if ($scope.items.childDetails.length > 0)
            $scope.isDetailGridVisibleForPopup = true;
        else
            $scope.isDetailGridVisibleForPopup = false;

        $scope.Close = function () {
            $modalInstance.close();
        };
    };
    // END MODAL INSTANCE CONTROLLER

    // BEGIN UPLOAD EXCEL CONTROLLER
    var UploadExcelCtrl = function ($scope, $modalInstance) {
        $scope.Close = function () {
            $modalInstance.close();
            $scope.RetrieveInventories();
        };
    }
    // END UPLOAD EXCEL CONTROLLER
})();

