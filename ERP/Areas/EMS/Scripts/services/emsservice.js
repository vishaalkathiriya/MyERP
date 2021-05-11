; (function () {
    'use strict';

    angular
        .module('ERPApp')
        .factory('EMSService', EMSService);

    EMSService.$inject = ['$window', '$http', '$log'];
    function EMSService($window, $http, $log) {
        var baseUrl = '/api/EMSNewsletter';
        return {
            saveNewsletter: saveNewsletter,
            getClientGroups: getClientGroups,
            getclientGroupsByIds: getclientGroupsByIds,
            getAllNewsletters: getAllNewsletters,
            getNewsletterById: getNewsletterById,
            prepareNewsletters: prepareNewsletters
        };

        function generateHttpRequest(method, url, data) {
            return $http({
                method: method,
                url: baseUrl + url,
                responseType: 'json',
                data: data,
                dataType: 'json',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                }
            });
        };

        function saveNewsletter(newsletter) {
            return generateHttpRequest('POST', '/PostNewsletter', newsletter)
                .then(completeSaveNewsletter)
                .catch(completeSaveNewsletterFailed);

            function completeSaveNewsletter(response) {
                return response.data.DataList;
            }

            function completeSaveNewsletterFailed(error) {
                $log.info("====== Error while saving newsletter services/emsservice.js =======");
                $log.error(error.data);
            }
        }

        function prepareNewsletters(newsletterId, datetime) {
            return generateHttpRequest('POST', '/PostNewsletterPrepare', { newsletterId: newsletterId, datetime: datetime })
                .then(completeSaveNewsletter)
                .catch(completeSaveNewsletterFailed);

            function completeSaveNewsletter(response) {
                return response.data.DataList;
            }

            function completeSaveNewsletterFailed(error) {
                $log.info("====== Error while saving newsletter services/emsservice.js =======");
                $log.error(error.data);
            }
        }

        function getClientGroups(searchString) {
            return generateHttpRequest('GET', '/GetClientGroups?searchString=' +searchString, {})
                .then(completeGetClientGroups)
                .catch(completeGetClientGroupsFailed);

            function completeGetClientGroups(response) {
                return response.data.DataList;
            }

            function completeGetClientGroupsFailed(error) {
                $log.info("====== Error while getting ems client groups services/emsservice.js =======");
                $log.error(error.data);
            }
        }

        function getclientGroupsByIds(groupIds) {
            return generateHttpRequest('GET', '/GetclientGroupsByIds?groupIds=' + groupIds, {})
                .then(completeGetclientGroupsByIds)
                .catch(completeGetclientGroupsByIdsFailed);

            function completeGetclientGroupsByIds(response) {
                return response.data.DataList;
            }

            function completeGetclientGroupsByIdsFailed(error) {
                $log.info("====== Error while getting client groups by ids services/emsservice.js =======");
                $log.error(error.data);
            }
        }

        function getAllNewsletters() {
            return generateHttpRequest('GET', '/GetAllNewsletters', {})
                .then(completeGetAllNewsletters)
                .catch(completeGetAllNewslettersFailed);

            function completeGetAllNewsletters(response) {
                return response.data.DataList;
            }

            function completeGetAllNewslettersFailed(error) {
                $log.info("====== Error while getting newsletters services/emsservice.js =======");
                $log.error(error.data);
            }
        }

        function getNewsletterById(newsletterId) {
            return generateHttpRequest('GET', '/GetNewsletterById/' + newsletterId, {})
                .then(completeGetNewsletterById)
                .catch(completeGetNewsletterByIdFailed);

            function completeGetNewsletterById(response) {
                return response.data.DataList;
            }

            function completeGetNewsletterByIdFailed(error) {
                $log.info("====== Error while getting newsletter by id services/emsservice.js =======");
                $log.error(error.data);
            }
        }
    };
})();