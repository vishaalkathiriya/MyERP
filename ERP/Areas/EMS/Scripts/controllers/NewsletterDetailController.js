; (function () {
    'use strict';

    angular
        .module('ERPApp')
        .controller('NewsletterDetailController', NewsletterDetailController)
        .controller('ScheduleNewsletterController', ScheduleNewsletterController);

    NewsletterDetailController.$inject = ['EMSService', '$location', 'newsletter', '$modal'];
    function NewsletterDetailController(EMSService, $location, newsletter, $modal) {
        /* jshint validthis: true */
        var vm = this;
        vm.title = "Newsletter Detail";
        vm.loadGroups = loadGroups;
        vm.newsletter = newsletter || dummyNewsletter();
        vm.saveNewsletter = saveNewsletter;
        vm.newsletter.selectedGroups = [];
        vm.scheduleNewsletter = scheduleNewsletter;
        vm.isPreviewShow = false;
        vm.closePreview = closePreview;

        initialize();
        /////////////////////////////////////

        function initialize() {
            var groupIds = vm.newsletter.Too || "";
            if (groupIds !== "") {
                EMSService.getclientGroupsByIds(groupIds)
                    .then(function successCallback(result) {
                        vm.newsletter.selectedGroups = result;
                    }, function errorCallback(reason) {
                        toastr.error(reason, 'Opps, Something went wrong');
                    });
            }
        }

        function closePreview() {
            vm.isPreviewShow = false;
        }

        function loadGroups(query) {
            return EMSService.getClientGroups(query).then(function (result) {
                return result;
            }, function (reason) {
                toastr.error(reason, 'Opps, Something went wrong');
            });
        }

        function saveNewsletter() {
            var newsletterForSave = angular.copy(vm.newsletter);
            var selectedGroupIds = [];
            angular.forEach(newsletterForSave.selectedGroups, function (group, id) {
                selectedGroupIds.push(group.ClientGroupID);
            });
            newsletterForSave.Too = selectedGroupIds.join(',');
            EMSService.saveNewsletter(newsletterForSave)
                .then(function successCallbck(result) {
                    vm.newsletter.NewsletterID = result.NewsletterID;
                    if (newsletterForSave.NewsletterID === 0) {
                        $location.path('/detail/' + result.NewsletterID);
                    }
                    toastr.success(result, 'Newsletter saved');
                }, function errorCallback(reason) {
                    toastr.error(reason, 'Opps, Something went wrong');
                });
            
        }

        function scheduleNewsletter() {
            var modalInstance = $modal.open({
                templateUrl: 'scheduleNewsletterModal.html',
                controller: 'ScheduleNewsletterController',
                resolve: {
                    newsletter: function () {
                        return vm.newsletter
                    }
                }
            });
        }

        function dummyNewsletter() {
            return {
                "NewsletterID": 0,
                "FromName": "",
                "FromEmail": "",
                "Too": "",
                "Subject": "",
                "HTML": "",
                "HeaderAndFooter": "",
                "IsDeleted": 0,
                "NrOpened": 0
            }
        }
    }

    ScheduleNewsletterController.$inject = ['EMSService', '$scope', '$modalInstance', 'newsletter', '$filter'];
    function ScheduleNewsletterController(EMSService, $scope, $modalInstance, newsletter, $filter) {
        /* jshint validthis: true */
        $scope.newsletter = newsletter;
        $scope.cancel = cancel;
        $scope.scheduleNewsletter = scheduleNewsletter;
        $scope.dt = moment().format('DD-MM-YYYY');
        $scope.minDate = $scope.minDate ? null : new Date();
        $scope.open = open;
        $scope.dateOptions = { formatYear: 'yy', startingDay: 1 };
        $scope.scheduleTime = new Date();

        initialize();
        //////////////////////////////////////////////////////
        function initialize() {
            
        }

        function open($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        }

        function scheduleNewsletter() {
            var selectedDate;
            if (typeof $scope.dt === 'string') {
                selectedDate = $filter('date')($scope.dt, 'DD-MM-YYYY').split('-');
            } else {
                selectedDate = moment($scope.dt).format('DD-MM-YYYY').split('-');
            }
            var selectedTime = moment($scope.scheduleTime).format('HH-mm').split('-');
            var selectedDateTime = new Date(selectedDate[2], (selectedDate[1] - 1), selectedDate[0], selectedTime[0], selectedTime[1], 0, 0);
            EMSService.prepareNewsletters(newsletter.NewsletterID, moment(selectedDateTime).format('DD-MM-YYYY HH:mm'))
                .then(function (result) {
                    toastr.success(result, 'Newsletter prepared');
                    $modalInstance.dismiss('cancel');
                }, function (reason) {
                    toastr.error(reason, 'Opps, Something went wrong');
                });
        }

        function cancel() {
            $modalInstance.dismiss('cancel');
        }
    }
})();