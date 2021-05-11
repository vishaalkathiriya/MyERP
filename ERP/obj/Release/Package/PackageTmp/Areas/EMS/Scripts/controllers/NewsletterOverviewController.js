; (function () {
    'use strict';

    angular
        .module('ERPApp')
        .controller('NewsletterOverviewController', NewsletterOverviewController);
    
    NewsletterOverviewController.$inject = ['EMSService', 'newsletters'];
    function NewsletterOverviewController(EMSService, newsletters) {
        /* jshint validthis: true */
        var vm = this;
        vm.title = "Newsletters";
        vm.newsletters = newsletters;
        vm.deleteNewsletter = deleteNewsletter;
        

        initialize();
        /////////////////////////////////////

        function initialize() {
        }

        function deleteNewsletter(selectedNewsletter) {
            if(confirm('Are you sure you want to delete this newsletter')){
            selectedNewsletter.IsDeleted = 1;
            EMSService.saveNewsletter(selectedNewsletter)
                .then(function successCallbck(result) {
                    vm.newsletters.splice(vm.newsletters.indexOf(selectedNewsletter), 1);
                    toastr.success(result, 'Newsletter deleted');
                }, function errorCallback(reason) {
                    toastr.error(reason, 'Opps, Something went wrong');
                });
            }
        }
    }
})();