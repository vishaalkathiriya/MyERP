/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services").service("CategoryService", [
        "$http",
        function ($http) {

            var categorylist = {};

            /*get Category list*/
            categorylist.GetCategoryList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    cache: false,
                    contentType: 'application/json; charset=utf-8',
                    url: "/api/Categories/GetCatList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add Category*/
            categorylist.SaveCategory = function (_category) {
                return $http({
                    method: "POST",
                    url: "/api/Categories/SaveCategory?ts=" + new Date().getTime(),
                    data: _category,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            /*delete Category*/
            categorylist.DeleteCategory = function (_category) {
                //var category = {
                //    CategoryId: _category.CategoryId,
                //    CategoryName: _category.CategoryName,
                //    IsActive: _category.IsActive
                //};
                return $http({
                    method: "POST",
                    url: "/api/Categories/DeleteCategory?ts=" + new Date().getTime(),
                    data: _category,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            categorylist.IsActive = function (_category) {
                //var category = {
                //    CategoryId: _category.CategoryId,
                //    CategoryName: _category.CategoryName,
                //    IsActive: _category.IsActive
                //};
                return $http({
                    method: "POST",
                    url: "/api/Categories/ChangeStatus?ts=" + new Date().getTime(),
                    data: _category,
                    contentType: "application/json"
                });
            };

            /*delete selected records*/
            categorylist.deleteAll = function (strIds) {
                return $http({
                    method: "POST",
                    url: "/api/Categories/DeleteSelectedCat?ts=" + new Date().getTime(),
                    data: strIds,
                    contentType: "application/json"
                });
            };

            return categorylist;
        }
]);
