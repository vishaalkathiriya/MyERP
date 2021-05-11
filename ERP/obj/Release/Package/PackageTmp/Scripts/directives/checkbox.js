/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive("drcCheckbox", [
        function () {

            var setState = function (state, element, $input, $state_label, checked_class, ngModel) {
                $input.prop("checked", state);
                ngModel.$setViewValue(state);
                (state) ? $state_label.text("Active") : $state_label.text("InActive");
                (state) ? element.addClass(checked_class) : element.removeClass(checked_class);
            };

            var linker = function (scope, element, attrs, ngModel) {
                if (!ngModel) return;

                var checked_class = "checked",
                    $input = element.find("input"),
                    $state_label = element.find(".cb-state-text");

                ngModel.$render = function () {
                        setState(ngModel.$viewValue, element, $input, $state_label, checked_class, ngModel);
                };

                element.on("click", function () {
                    if ($input.is(":checked")) {
                        setState(false, element, $input, $state_label, checked_class, ngModel);
                    } else {
                        setState(true, element, $input, $state_label, checked_class, ngModel);
                    }
                });
            };


            return {
                restrict: "A",
                replace: true,
                scope: false,
                require: "?ngModel",
                template: '<div class="checkbox checkbox-replace neon-cb-replacement">' +
                            '<label class="cb-wrapper"><input type="checkbox"/><div class="checked"></div></label>' +
                            '<label class="cb-state-text">Active</label>' +
                          '</div>',
                link: linker
            };

        }
    ]);


