/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcFocusNext', function ($parse, $timeout) {
        return {
            restrict: "A",
            link: function (scope, element, attr) {
                //apply focus on next element on which we are using this directive
                element.bind('keydown', function (e) {
                    //get the next index of text input element
                    var next_idx = $('input[type=text]').index(this) + 1;
                    //get number of text input element in a html document
                    var tot_idx = $('body').find('input[type=text]').length;

                    //enter button in ASCII code
                    var code = e.keyCode || e.which;
                    if (code == 13) {
                        e.preventDefault();
                        if (tot_idx == next_idx)
                            //go to the first text element if focused in the last text input element
                            $('input[type=text]:eq(0)').focus();
                        else
                            //go to the next text input element
                            $('input[type=text]:eq(' + next_idx + ')').focus();
                    }
                });
            }
        };
    });