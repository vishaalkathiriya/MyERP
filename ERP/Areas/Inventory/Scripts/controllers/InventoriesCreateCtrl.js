/// <reference path="../libs/angular/angular.js" />

(function () {

    'use strict';

    angular.module("ERPApp.Controllers").controller("InventoriesCreateCtrl", [
        "$scope",
        "$modal",
        "$rootScope",
        "$timeout",
        "InventoriesService",
        "$filter",
        "$q",
        InventoriesCtrl
    ]);

    function InventoriesCtrl($scope, $modal, $rootScope, $timeout, IS, $filter, $q) {

        // BEGIN VARIABLES
        $scope.masterData = $scope.masterData || {};
        $scope.detailData = $scope.masterData || [];
        $scope.isDetailTabEnabled = false;
        $scope.isReceiveTabEnabled = false;
        $scope.isDetailGridVisible = true;
        $scope.isReceiveGridVisible = true;
        $scope.isFirstFocus = false;
        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $scope.modeDetail = "Add";
        $scope.SaveTextDetail = "Save";
        $scope.lastInventoryDetails = $scope.lastInventoryDetails || [];
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.subCategoriesForDetail = [];
        $scope.pDate = "";
        // END VARIABLES

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
        };
        $scope.calendarOpen = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.calendarOpened = true;
        };
        $scope.maxDate = $scope.maxDate || new Date();
        // END DATE PICKER

        // BEGIN RETRIEVE DATA FOR COMBOBOX
        IS.RetrieveVendors().then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType === 1) {
                    $scope.vendors = result.data.DataList;
                    $scope.RetrieveLocations();
                }
                else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            }
            else {
                $rootScope.redirectToLogin();
            }
        });
        $scope.RetrieveLocations = function () {
            IS.RetrieveLocations().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.locations = result.data.DataList;
                        $scope.RetrieveBrands();
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
        $scope.RetrieveBrands = function () {
            IS.RetrieveBrands().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.brands = result.data.DataList;
                        $scope.RetrieveCategories();
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
        $scope.RetrieveCategories = function () {
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
        }
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
        $scope.RetrieveSubCategoriesForDetail = function (CategoryId) {
            IS.RetrieveSubCategories(CategoryId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.subCategoriesForDetail = result.data.DataList;
                        // $scope.detailData.SubCategoryId = 0;
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
        $scope.PurchaseDateCalendarOpened = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isPurchaseDateCalendarOpened = true;
        };
        // END RETRIEVE DATA FOR COMBOBOX

        // BEGIN INVENTORY
        $scope.InitInventory = function (inventoryId) {
            $rootScope.IsAjaxLoading = true;
            $scope.InventoryId = inventoryId;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            if (inventoryId === 0) {
                $scope.mode = "Add";
                $scope.SaveText = "Save";
                $scope.isDetailTabEnabled = false;
                $scope.isReceiveTabEnabled = false;
                $scope.masterData = {
                    InventoryId: 0,
                    InventoryName: "",
                    IssueTo: "",
                    VendorId: 0,
                    LocationId: 0,
                    BrandId: 0,
                    CategoryId: 0,
                    SubCategoryId: 0,
                    PurchaseDate: "",
                    Amount: "",
                    SerialNumber: "",
                    IsAvailable: true,
                    IsScrap: false,
                    Remarks: ""
                }
                $scope.detailData = {
                    SrNo: 0,
                    BrandId: 0,
                    CategoryId: 0,
                    SubCategoryId: 0,
                    SerialNumber: ""
                }
                $scope.details = [];
            }
            else {
                $timeout(function () {
                    IS.RetriveInventory(inventoryId, $scope.timeZone).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType === 1) {
                                $scope.mode = "Edit";
                                $scope.SaveText = "Update";
                                $scope.masterData = result.data.DataList;
                                $scope.masterData.VendorId = result.data.DataList.VendorId;
                                $scope.masterData.LocationId = result.data.DataList.LocationId;
                                $scope.masterData.BrandId = result.data.DataList.BrandId;
                                $scope.masterData.CategoryId = result.data.DataList.CategoryId;
                                $scope.masterData.pDate = result.data.DataList.PurchaseDate;
                                $scope.masterData.PurchaseDate = $filter('date')(result.data.DataList.PurchaseDate, 'dd-MM-yyyy');
                                $scope.detailData = {
                                    SrNo: 0,
                                    BrandId: 0,
                                    CategoryId: 0,
                                    SubCategoryId: 0,
                                    SerialNumber: ""
                                }
                                $scope.isDetailTabEnabled = true;
                            }
                        }
                    });
                    $scope.RetrieveInventoryDetails();
                }, 0);
            }
            $rootScope.IsAjaxLoading = false;
        }
        $scope.EnableDisableReceiveTab = function (flag) {
            $scope.isReceiveTabEnabled = !flag;
        }
        $scope.CreateUpdateInventories = function (inventory, frmInventories) {
            IS.CreateUpdateInventories(inventory, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        frmInventories.$setPristine();
                        $scope.InventoryId = result.data.DataList;
                        $scope.isDetailTabEnabled = true;
                        $scope.isFirstFocus = false;
                        $timeout(function () {
                            $scope.isFirstFocus = true;
                        });
                        toastr.success(result.data.Message, 'Success');
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
        $scope.$watch('masterData.CategoryId', function (newValue) {
            $scope.RetrieveSubCategories(newValue);
        });
        $scope.ResetInventory = function (frmInventories) {
            $scope.InitInventory($scope.InventoryId);
            frmInventories.$setPristine();
        }
        $scope.CloseInventory = function () {
            $scope.masterData = [];
            $scope.detailData = [];
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            $scope.isDetailTabEnabled = false;
            $scope.isFirstFocus = false;
        }
        // END INVENTORY

        // BEGIN INVENTORY DETAIL
        $scope.CreateUpdateInventoriesDetail = function (_details, frmInventoryDetails) {
            // INSERTING ENTRIES IN INVENTORY DETAIL
            var detail = {
                SrNo: _details.SrNo,
                InventoryId: $scope.InventoryId,
                BrandId: _details.BrandId,
                CategoryId: _details.CategoryId,
                SubCategoryId: _details.SubCategoryId,
                SerialNumber: _details.SerialNumber,
                IsAvailable: false,
                IsScrap: false,
                Status: 'I'
            };
            IS.CreateUpdateInventoriesDetail(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.detailData = {
                            BrandId: 0,
                            CategoryId: 0,
                            SubCategoryId: 0,
                            SerialNumber: ''
                        };
                        $scope.modeDetail = "Add";
                        $scope.SaveTextDetail = "Save";
                        frmInventoryDetails.$setPristine();
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveInventoryDetails();
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
        $scope.UpdateInventoryDetail = function (detail) {
            $scope.lastInventoryDetails = {
                SrNo: detail.SrNo,
                InventoryId: detail.InventoryId,
                BrandId: detail.BrandId,
                CategoryId: detail.CategoryId,
                SubCategoryId: detail.SubCategoryId,
                SerialNumber: detail.SerialNumber,
                IsScrap: detail.IsScrap,
                Status: detail.Status
            };
            $scope.detailData = {
                SrNo: detail.SrNo,
                InventoryId: detail.InventoryId,
                BrandId: detail.BrandId,
                CategoryId: detail.CategoryId,
                SubCategoryId: detail.SubCategoryId,
                SerialNumber: detail.SerialNumber,
                IsScrap: detail.IsScrap,
                Status: detail.Status
            };
            $scope.RetrieveSubCategoriesForDetail(detail.CategoryId);
            $scope.modeDetail = "Edit";
            $scope.SaveTextDetail = "Update";
        }
        $scope.ResetInventoryDetail = function (frmInventoryDetails) {
            if ($scope.modeDetail === "Add") {
                $scope.detailData = {
                    SrNo: 0,
                    BrandId: 0,
                    CategoryId: 0,
                    SubCategoryId: 0,
                    SerialNumber: ""
                }
                $scope.modeDetail = "Add";
                $scope.SaveTextDetail = "Save";
            }
            else if ($scope.modeDetail === "Edit") {
                $scope.detailData = {
                    SrNo: $scope.lastInventoryDetails.SrNo,
                    InventoryId: $scope.lastInventoryDetails.InventoryId,
                    BrandId: $scope.lastInventoryDetails.BrandId,
                    CategoryId: $scope.lastInventoryDetails.CategoryId,
                    SubCategoryId: $scope.lastInventoryDetails.SubCategoryId,
                    SerialNumber: $scope.lastInventoryDetails.SerialNumber
                };
                $scope.modeDetail = "Edit";
                $scope.SaveTextDetail = "Update";
            }
            frmInventoryDetails.$setPristine();
        }
        $scope.DeleteInventoryDetail = function (detail) {
            // DELETE ENTRIES FROM INVENTORY DETAIL
            IS.DeleteInventoryDetail(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrieveInventoryDetails();
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
        $scope.$watch('detailData.CategoryId', function (newValue) {
            $scope.RetrieveSubCategoriesForDetail(newValue);
        });
        $scope.RetrieveInventoryDetails = function () {
            IS.ShowInventoryDetail($scope.InventoryId, $scope.timeZone).then(function (result) {
                $scope.details = result.DataList;
                if ($scope.details.length > 0) {
                    $scope.isDetailTabEnabled = true;
                    $scope.isDetailGridVisible = true;
                    $scope.isReceiveTabEnabled = true;
                    $scope.isReceiveGridVisible = true;
                }
                else {
                    $scope.isDetailGridVisible = false;
                    $scope.isReceiveGridVisible = false;
                }
            });
        }
        $scope.ReceiveInventoryDetail = function (detail) {
            IS.ReceiveInventoryDetail(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveInventoryDetails();

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
        $scope.ScarpInventoryDetail = function (detail) {
            IS.ScarpInventoryDetail(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrieveInventoryDetails();
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
        $scope.AddToStock = function (detail) {
            IS.AddToStock(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrieveInventoryDetails();
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
        $scope.ChangeStatus = function (detail) {
            IS.ChangeStatus(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrieveInventoryDetails();
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
        // END INVENTORY DETAIL

        // BEGIN COMBO VALIDATION
        $scope.ValidateMasterVendor = function () {
            if ($scope.masterData.VendorId && $scope.masterData.VendorId != 0)
                return false;
            return true;
        }
        $scope.ValidateMasterLocation = function () {
            if ($scope.masterData.LocationId && $scope.masterData.LocationId != 0)
                return false;
            return true;
        }
        $scope.ValidateMasterBrand = function () {
            if ($scope.masterData.BrandId && $scope.masterData.BrandId != 0)
                return false;
            return true;
        }
        $scope.ValidateMasterCategory = function () {
            if ($scope.masterData.CategoryId && $scope.masterData.CategoryId != 0)
                return false;
            return true;
        }
        $scope.ValidateMasterSubCategory = function () {
            if ($scope.masterData.SubCategoryId && $scope.masterData.SubCategoryId != 0)
                return false;
            return true;
        }
        $scope.ValidateDetailBrand = function () {
            if ($scope.detailData.BrandId && $scope.detailData.BrandId != 0)
                return false;
            return true;
        }
        $scope.ValidateDetailCategory = function () {
            if ($scope.detailData.CategoryId && $scope.detailData.CategoryId != 0)
                return false;
            return true;
        }
        $scope.ValidateDetailSubCategory = function () {
            if ($scope.detailData.SubCategoryId && $scope.detailData.SubCategoryId != 0)
                return false;
            return true;
        }

        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };

        $scope.ValidatePurchaseDate = function (purchaseDate, frmInventories) {
            if (!purchaseDate) {
                frmInventories.txtPurchaseDate.$setValidity("invalidPurchaseDate", true);
                return;
            } else if (purchaseDate.length == 10) {
                if ($scope.ValidateDate(purchaseDate)) {
                    frmInventories.txtPurchaseDate.$setValidity("invalidPurchaseDate", true);
                    var dt = purchaseDate.split('-');
                    $scope.masterData.pDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    //$scope.masterData.pDate = new Date(dt[2], dt[1] - 1, dt[0]);
                } else {
                    frmInventories.txtPurchaseDate.$setValidity("invalidPurchaseDate", false);
                }
            } else {
                frmInventories.txtPurchaseDate.$setValidity("invalidPurchaseDate", false);
            }
        }
        $scope.$watch('masterData.pDate', function (newValue) {
            $scope.masterData.PurchaseDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        // END COMBO VALIDATION
    }
})();

