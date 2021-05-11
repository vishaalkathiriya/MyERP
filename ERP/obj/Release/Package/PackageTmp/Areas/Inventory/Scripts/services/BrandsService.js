/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services").service("BrandsService", [
    "$http",
    function ($http) {
        var brand = {};

        brand.CreateUpdateBrand = function (_brand) {
            var brand = {
                BrandId: _brand.brandId,
                BrandName: _brand.brandName,
                IsActive: _brand.isActive
            };
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Brands/CreateUpdateBrand?ts=" + new Date().getTime(),
                data: brand,
                contentType: 'application/json; charset=utf-8'
            });
        }

        brand.RetriveBrands = function (timezone, page, count, orderby, filter) {
            return $http({
                method: "GET",
                cache: false,
                url: "/api/Brands/RetrieveBrand?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
            });
        }

        brand.DeleteBrand = function (_brand) {
            var brand = {
                BrandId: _brand.BrandId,
                BrandName: _brand.BrandName,
                IsActive: _brand.IsActive
            }
            return $http({
                method: 'POST',
                cache: false,
                url: "/api/Brands/DeleteBrand?ts=" + new Date().getTime(),
                data: brand,
                contentType: 'application/json; charset=utf-8'
            });
        }

        brand.ChangeStatus = function (_brand) {
            var brand = {
                BrandId: _brand.BrandId,
                BrandName: _brand.BrandName,
                IsActive: _brand.IsActive
            };
            return $http({
                method: "POST",
                cache: false,
                url: "/api/Brands/ChangeStatus?ts=" + new Date().getTime(),
                data: brand,
                contentType: "application/json"
            });
        };

        return brand;
    }
]);