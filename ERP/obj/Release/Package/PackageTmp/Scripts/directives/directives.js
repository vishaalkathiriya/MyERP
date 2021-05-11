/// <reference path="../libs/angular/angular.min.js" />

var ERPAppsDirectives = angular.module("ERPApps.Directives", []);

ERPAppsDirectives.directive("DataTable", function () {
    var oTable = undefined;

    //Generate DataTable
    var tableID = "table-2";
    var oTable = $("#table-2").dataTable({
        "sPaginationType": "bootstrap",
        //"sDom": "t<'row'<'col-xs-6 col-left'i><'col-xs-6 col-right'p>>",
        "bStateSave": false,
        "iDisplayLength": 10,
        "aoColumns": [
            { "bSortable": false },
            null,
            null,
            null
        ]
    });

    return oTable;
});


ERPAppsDirectives.directive('ngHasfocus', function () {
    alert("sd");
    return function (scope, element, attrs) {
        scope.$watch(attrs.ngHasfocus, function (nVal, oVal) {
            if (nVal)
                element[0].focus();
        });

        element.bind('blur', function () {
            scope.$apply(attrs.ngHasfocus + " = false");
        });

        element.bind('keydown', function (e) {
            if (e.which == 13)
                scope.$apply(attrs.ngHasfocus + " = false");
        });
    }
});