/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcDragToSortForTodo', ["$compile", "$http", "$timeout", "$rootScope", "PMSModuleService", function ($compile, $http, $timeout, $rootScope, PMSModuleService) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $('.droppableTodo').sortable({
                    start: function(){
                        $(this).find('span.option-panel').removeClass('visible').addClass('invisible');
                    },
                    update: function (event, ui) {
                        scope.strIds = "";
                        $(".draggableTodo").each(function () {
                            var index = parseInt($(this).index()) + 1;
                            scope.strIds += $(this).data('id') + ":" + index + ","
                        });

                        $rootScope.IsAjaxLoading = true;
                        PMSModuleService.DoSortingForTodo(scope.strIds).then(function (result) {
                            if (result.data.IsValidUser) {
                                if (result.data.MessageType == 1) { // 1:Success
                                    toastr.success(result.data.Message, 'Success');
                                } else {
                                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                                }
                            } else {
                                $rootScope.redirectToLogin();
                            }
                            $rootScope.IsAjaxLoading = false;
                        });
                    }
                });
            }
        }
    }]);