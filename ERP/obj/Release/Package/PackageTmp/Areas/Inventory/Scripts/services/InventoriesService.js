/// <reference path="../libs/angular/angular.js" />

(function () {

    angular.module("ERPApp.Services").service("InventoriesService", [
        "$http",
        "$q",
        "$filter",
        InventoriesService
    ]);

    function InventoriesService($http, $q, $filter) {

        var inventories = {};

        inventories.RetrieveVendors = function () {
            return $http({
                method: 'GET',
                cache: false,
                url: "/api/Inventories/RetrieveVendors?ts=" + new Date().getTime(),
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetrieveLocations = function () {
            return $http({
                method: 'GET',
                cache: false,
                url: "/api/Inventories/RetrieveLocations?ts=" + new Date().getTime(),
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetrieveBrands = function () {
            return $http({
                method: 'GET',
                cache: false,
                url: "/api/Inventories/RetrieveBrands?ts=" + new Date().getTime(),
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetrieveCategories = function () {
            return $http({
                method: 'GET',
                cache: false,
                url: "/api/Inventories/RetrieveCategories?ts=" + new Date().getTime(),
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetrieveSubCategories = function (_CategoryId) {
            var category = {
                CategoryId: _CategoryId
            }

            return $http({
                method: "POST",
                cache: false,
                url: "/api/Inventories/RetrieveSubCategories?ts=" + new Date().getTime(),
                data: category,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.CreateUpdateInventories = function (_inventory, _timezone) {
            var date = _inventory.PurchaseDate.split('-');
            var _purchaseDate = new Date(date[2], date[1] - 1, date[0]);
            var inventory = {
                InventoryId: _inventory.InventoryId,
                InventoryName: _inventory.InventoryName,
                IssueTo: _inventory.IssueTo,
                VendorId: _inventory.VendorId,
                LocationId: _inventory.LocationId,
                BrandId: _inventory.BrandId,
                CategoryId: _inventory.CategoryId,
                SubCategoryId: _inventory.SubCategoryId,
                PurchaseDate: _purchaseDate,
                Amount: _inventory.Amount,
                SerialNumber: _inventory.SerialNumber,
                IsAvailable: _inventory.IsAvailable,
                IsScrap: _inventory.IsScrap,
                Remarks: _inventory.Remarks
            }
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/CreateUpdateInventories?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetriveInventory = function (_inventoryId, timezone) {
            var inventory = {
                InventoryId: _inventoryId
            };
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/RetriveInventory?ts=" + new Date().getTime() + "&timezone=" + timezone,
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.RetrieveInventories = function (filters, timezone, page, count, orderby, InventoryName, IssueTo) {
            var inventory = {
                VendorId: filters.VendorId,
                LocationId: filters.LocationId,
                BrandId: filters.BrandId,
                CategoryId: filters.CategoryId,
                SubCategoryId: filters.SubCategoryId
            };
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/RetrieveInventories?ts=" + new Date().getTime() + "&filters" + filters + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&InventoryName=" + InventoryName + "&IssueTo=" + IssueTo,
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.DeleteInventory = function (_inventory) {
            var inventory = {
                InventoryId: _inventory.InventoryId
            }
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/DeleteInventory?ts=" + new Date().getTime(),
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.CreateUpdateInventoriesDetail = function (_inventoryDetail) {
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/CreateUpdateInventoriesDetail?ts=" + new Date().getTime(),
                data: _inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.DeleteInventoryDetail = function (detail) {
            var inventoryDetail = {
                SrNo: detail.SrNo
            };

            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/DeleteInventoryDetail?ts=" + new Date().getTime(),
                data: inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.ReceiveInventoryDetail = function (detail) {
            var inventoryDetail = {
                SrNo: detail.SrNo
            };

            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/ReceiveInventoryDetail?ts=" + new Date().getTime(),
                data: inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.ScarpInventoryDetail = function (detail) {
            var inventoryDetail = {
                SrNo: detail.SrNo
            };

            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/ScarpInventoryDetail?ts=" + new Date().getTime(),
                data: inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.AddToStock = function (detail) {
            var inventoryDetail = {
                SrNo: detail.SrNo
            };

            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/AddToStock?ts=" + new Date().getTime(),
                data: inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.ChangeStatus = function (detail) {
            var inventoryDetail = {
                SrNo: detail.SrNo
            };

            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/ChangeStatus?ts=" + new Date().getTime(),
                data: inventoryDetail,
                contentType: 'application/json; charset=utf-8'
            });
        }

        inventories.ShowInventory = function (_inventory) {
            var inventory = {
                InventoryId: _inventory.InventoryId
            }
            var deferred = $q.defer();

            $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/RetriveInventoriesForPopup?ts=" + new Date().getTime(),
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            }).success(function (result) { deferred.resolve(result) }).error(function (error) { deferred.reject(error); });

            return deferred.promise;
        }

        inventories.ShowInventoryDetail = function (_inventoryId, timezone) {
            var inventory = {
                InventoryId: _inventoryId
            }

            var deferred = $q.defer();

            $http({
                method: 'POST',
                cache: false,
                url: "/api/Inventories/RetriveInventoryDetailForPopup?ts=" + new Date().getTime() + "&timezone=" + timezone,
                data: inventory,
                contentType: 'application/json; charset=utf-8'
            }).success(function (result) { deferred.resolve(result) }).error(function (error) { deferred.reject(error); });
            return deferred.promise;
        }

        return inventories;
    }

})();