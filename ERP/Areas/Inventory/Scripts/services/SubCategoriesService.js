/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services").service("SubCategoriesService", [
    "$http",
    function ($http) {

        var subCategory = {}

        subCategory.RetrieveCategories = function () {
            return $http({
                method: 'GET',
                url: "/api/SubCategories/RetrieveCategories?ts=" + new Date().getTime(),
                contentType: 'application/json; charset=utf-8'
            });
        }

        subCategory.CreateUpdateSubCategory = function (_subCategory) {
            var subCategory = {
                CategoryId: _subCategory.CategoryId,
                SubCategoryId: _subCategory.SubCategoryId,
                SubCategoryName: _subCategory.SubCategoryName,
                IsActive: _subCategory.IsActive
            }

            return $http({
                method: 'POST',
                url: "/api/SubCategories/CreateUpdateSubCategory?ts=" + new Date().getTime(),
                data: _subCategory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        subCategory.RetrieveSubCategories = function (topFilter, timezone, page, count, orderby, CategoryName, SubCategoryName) {
            return $http({
                method: "GET",
                url: "/api/SubCategories/RetrieveSubCategories?ts=" + new Date().getTime() + "&topFilter=" + topFilter + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&categoryName=" + CategoryName + "&subCategoryName=" + SubCategoryName,
                contentType: "application/json; charset=utf-8"
            });
        }

        subCategory.DeleteSubCategory = function (_subCategory) {
            var subCategory = {
                SubCategoryId: _subCategory.SubCategoryId,
                SubCategoryName: _subCategory.SubCategoryName,
                IsActive: _subCategory.IsActive
            }

            return $http({
                method: 'POST',
                url: "/api/SubCategories/DeleteSubCategory?ts=" + new Date().getTime(),
                data: subCategory,
                contentType: 'application/json; charset=utf-8'
            });
        }

        subCategory.ChangeStatus = function (_subCategory) {
            var subCategory = {
                SubCategoryId: _subCategory.SubCategoryId,
                SubCategoryName: _subCategory.SubCategoryName,
                IsActive: _subCategory.IsActive
            };
            return $http({
                method: "POST",
                url: "/api/SubCategories/ChangeStatus?ts=" + new Date().getTime(),
                data: subCategory,
                contentType: 'application/json; charset=utf-8'
            });
        };

        return subCategory;
    }
]);