angular.module("ERPApp.Filters")
    .filter('removehtmltags', function () {
        return function (OriginalString) {
            var string = OriginalString ? OriginalString.replace(/(<([^>]+)>)/ig, "") : "";
            return string;
        }
    });