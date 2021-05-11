; (function () {
    'use strict';

    angular
        .module('ERPApp')
        .directive('emsEmailPreview', emsEmailPreview);

    function emsEmailPreview() {

        EMSEmailPreviewController.$inject = ['$scope'];
        function EMSEmailPreviewController($scope) {
            /* jshint validthis: true */
            var vm = this;
            vm.showPreview = showPreview;

            function setIframeHtml(html) {
                var $previewIframe = $('#' + vm.previewIframeId);
                var doc = $previewIframe[0].contentWindow.document;
                var $body = $('body', doc);
                var $head = $('head', doc)
                $head.html('<meta charset="utf-8"><meta name="viewport" content="width=device-width">')
                $body.html(html);

                setTimeout(function () {
                    var iframeHeight = $previewIframe[0].contentWindow.document.body.scrollHeight;
                    $previewIframe[0].height = iframeHeight;
                }, 100);
            }

            function showPreview() {
                vm.isPreviewShow = true;
                var bodyWrapper = "<div><header>||HEADER||</header><div>||BODY||</div></footer>||FOOTER||</foote></div>";
                vm.bodyWrapper = vm.bodyWrapper || bodyWrapper;
                vm.header = vm.header || "<div></div>";
                vm.footer = vm.footer || "<div></div>";
                var bodyHTML = vm.bodyWrapper.replace('||BODY||', vm.html).replace('||HEADER||', vm.header).replace('||FOOTER||', vm.footer);
                setIframeHtml(bodyHTML);
            };

            $scope.$watch(function watchExp() {
                return vm.isPreviewShow;
            }, function watcher(newValue, oldValue) {
                if (newValue !== oldValue) {
                    if (newValue === false) {
                        setIframeHtml('');
                    }
                }
            });
        }

        var directive = {
            restrict: 'E',
            scope: {
                html: '=',
                header: '@',
                bodywrapper: '@',
                footer: '@',
                isPreviewShow: '=',
                previewIframeId: '@'
            },
            templateUrl: './Newsletter/EmailPreview',
            replace: true,
            controller: EMSEmailPreviewController,
            controllerAs: 'vm',
            bindToController: true
        };
        return directive;
    }

})();