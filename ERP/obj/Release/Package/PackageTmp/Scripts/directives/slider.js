angular.module("ERPApp.Directives")
    .directive('slider', function () {
        return {
            restrict: 'AE',
            replace: true,
            scope: {
                list: '=',
                index: '@',
                mainpath: '@',
                id:'@',
                callbackevent: '&'
            },
            templateUrl: '/scripts/template/slider.html',
            link: function (scope, elem, attrs) {
                if (scope.index) {
                    scope.currentIndex = scope.index;
                } else { // if index is not defined
                    scope.currentIndex = 0; // Initially the index is at the first image
                }

                scope.next = function () {
                    scope.currentIndex < scope.list.length - 1 ? scope.currentIndex++ : scope.currentIndex = 0;
                };

                scope.prev = function () {
                    scope.currentIndex > 0 ? scope.currentIndex-- : scope.currentIndex = scope.list.length - 1;
                };

                scope.close = function () {
                    scope.callbackevent();
                };

                scope.$watch('currentIndex', function () {
                    scope.list.forEach(function (item) {
                        item.visible = false; // make every item invisible
                        if (scope.id == 0) { //passing 0 for all other ref
                            item.mainPath = scope.mainpath;
                        } else { // comment is creating project folder
                            item.mainPath = scope.mainpath + "/" + scope.id;
                        }
                        
                    });

                    scope.list[scope.currentIndex].visible = true; // make the current item visible
                });
            }
        };
    });