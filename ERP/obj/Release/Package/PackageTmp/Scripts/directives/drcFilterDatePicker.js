

angular.module("ERPApp.Directives").directive("drcFilterDatePicker",
    ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            scope: {
                daterange: "=daterange"
            },
            link: function (scope, element, attrs, controller) {

                $(element).on("blur", function () {
                    if ($.trim($(element).val()) == "") {
                        $rootScope.$safeApply($rootScope, function () {
                            scope.daterange.startDate = "";
                            scope.daterange.endDate = "";
                        });
                    }
                });

                function setDate(currentValue) {
                    $rootScope.$safeApply($rootScope, function () {
                        var dateRanges = currentValue.split("-");
                        if (dateRanges[0]) {
                            //scope.daterange.startDate = dateRanges[0].replace(/\//g, "-");
                            scope.daterange.startDate = dateRanges[0];
                        }
                        if (dateRanges[1]) {
                            //scope.daterange.endDate = dateRanges[1].replace(/\//g, "-");
                            scope.daterange.endDate = dateRanges[1];
                        } else {
                            if (dateRanges[0]) {
                                //scope.daterange.endDate = dateRanges[0].replace(/\//g, "-");
                                scope.daterange.endDate = dateRanges[0];
                            }
                        }
                    });
                };

                $(element).daterangepicker({
                    dateFormat: "dd/mm/yy",
                    onChange: function () {
                        var currentValue = $(element).val();
                        if (currentValue) {
                            setDate(currentValue);
                        }
                    }
                });

                
                if ($(element).val() && $.trim($(element).val() != "")) {
                    setDate($(element).val());
                };
                
            }
        }
    }]);