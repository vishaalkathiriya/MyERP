
angular.module("ERPApp.Controllers").controller("testCtrl", [
    "$scope",
    function ($scope) {
        
        //$scope.$watch("isChecked", function (newValue, oldValue) {
        //    console.log("new value: " + newValue);
        //});
        $scope.isChecked = false;
        $scope.change = function () {
            $scope.isChecked = false;
        };


        $scope.people = [
            { name: 'Person 1', age: 12 },
            { name: 'Person 2', age: 14 },
            { name: 'Person 3', age: 15 }
        ];

        $scope.onConfirm = function (person) {
            console.log(person);
            alert("deleted");
        }
    }
]);