
angular.module("ERPApp.Directives")
    .directive("drcConfirmBox", [
        "$rootScope",
        function ($rootScope) {

            return {
                restrict: "A",
                scope: {
                    callback: "&callback",
                    message: "@",
                    cancelcallback: "&cancelcallback"
                },
                link: function (scope, element, attrs) {
                    //bind on click event and apply bootbox confirmbox on click event
                    element.on("click", function () {
                        bootbox.confirm(scope.message, function (result) {
                            if (result) {
                                scope.callback();
                            } else {
                                //directive call if it is available
                                scope.cancelcallback();
                            }
                        });
                    });

                }
            }

        }]);