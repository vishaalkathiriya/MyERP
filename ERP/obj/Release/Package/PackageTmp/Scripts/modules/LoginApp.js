

(function () {

    'use strict';


    angular.module('ERPLogin', []);

    angular.module("ERPLogin").config([
      '$httpProvider',
      function ($httpProvider) {
        //$httpProvider.interceptors.push(function ($q) {
        //  return {
        //    'request': function (config) {
        //      config.url = 'drcnrr/erptest/' + config.url;
        //      return config || $q.when(config);
        //    }
        //  }
        //});
      }
    ]);

    angular.module('ERPLogin')
        .controller('LoginController', [
            '$scope', '$http', LoginController
        ]);

    function LoginController($scope, $http) {

        function resetForm() {
            $scope.userLogin = {
                UserId: 0,
                Username: "admin",
                Password: "admin"
            };
        };
        resetForm();

        $scope.submitLogin = function () {

            $http({
                method: 'POST',
                url: './api/Login/IsAuthenticatedUser',
                data: $scope.userLogin,
                contentType: 'application/json; charset=utf-8',
            }).success(function (result) {
                if (result.MessageType == 1) {
                    window.location.href = "./Dashboard";
                }
                else if (result.MessageType == 2) {
                    toastr.warning("You do not have enough access privileges for this application", "Access denied");
                }
                else if (result.MessageType == 0) {
                    toastr.error("Username or password is incorrect", 'Opps, Something went wrong');
                }
                else if (result.MessageType == -1) {
                    toastr.error("An error occured while processing your request", 'Opps, Something went wrong');
                }
                $scope.IsDisplay = false;

            }).error(function (error) {
                toastr.error("An error occured while processing your request", 'Opps, Something went wrong');
            });

        };

    };

})();