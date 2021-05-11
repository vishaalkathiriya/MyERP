angular.module("ERPApp.Filters")
    .filter('decimalToTime', function () {
        return function (hours) {
            var _hours = hours.toString().split('.')[0];
            var _minutes = hours.toString().split('.')[1];
            _hours = parseInt(_hours) + parseInt(Math.floor(_minutes / 60));
            _minutes = _minutes % 60;
            return _hours + "." + _minutes;
        }
    });