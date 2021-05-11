
//Inventory Details Directory
//Developed on: 1 Feb 2014

(function () {

    'use strict';

    angular.module("ERPApp.Directives")
        .directive("drcInventoryDetail", [
            "InventoriesService",
            "$timeout",
            function (IS, $timeout) {


                return {

                    restrict: "A",
                    scope: {
                        detail: "=drcInventoryDetail",
                        index: "@",
                        del: "&"
                    },
                    templateUrl: "StaticPages/InventoryDetail",
                    compile: function (tElement, tAttrs, transclude) {

                        var InventoryDetailUtils = {

                            Brands: function () {
                                return IS.RetrieveBrands().then(function (result) {
                                    if (result.data.MessageType === 1) {
                                        return result.data.DataList;
                                    }
                                    else {
                                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                                    }
                                });
                            },
                            Categories: function () {
                                return IS.RetrieveCategories().then(function (result) {
                                    if (result.data.MessageType === 1) {
                                        return result.data.DataList;
                                    }
                                    else {
                                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                                    }
                                });
                            },
                            SubCategories: function (CategoryId) {
                                return IS.RetrieveSubCategories(CategoryId).then(function (result) {
                                    if (result.data.MessageType === 1) {
                                        return result.data.DataList;
                                        //scope.details.SubCategoryId = 0;
                                    }
                                    else {
                                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                                    }
                                });
                            }

                        };


                        return function link(scope, element, attrs) {

                            InventoryDetailUtils.Brands().then(function (result) {
                                scope.Brands = result;
                            });

                            InventoryDetailUtils.Categories().then(function (result) {
                                scope.Categories = result;
                            });

                            scope.$watch('detail.CategoryId', function (newValue) {
                                scope.RetrieveSubCategories(newValue);
                            });

                            scope.RetrieveSubCategories = function (categoryId) {
                                InventoryDetailUtils.SubCategories(categoryId).then(function (result) {
                                    scope.SubCategories = result;
                                });
                            };

                        }


                    }

                }


            }
        ]);



})();

