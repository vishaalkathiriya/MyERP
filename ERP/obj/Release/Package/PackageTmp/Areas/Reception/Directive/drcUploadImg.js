angular.module("ERPApp.Directives")
    .directive('drcUploadImg', ["$compile", "$http", "$timeout", "$rootScope", "VisitorMasterService", function ($compile, $http, $timeout, $rootScope, VisitorMasterService) {
        return {
            restrict: 'A',
            //scope: {
            //    photo_url: "=",
            //    callback: "&"
            //},
            link: function (scope, element, attrs, $parse) {
                //success: function(){ scope.$apply(function(){ scope.photoUrl = data; }); }
                //element.on('change', function () {
                //    debugger;
                //    console.log("test");
                //    //scope.$eval(attrs.ngModel + "='" + element.val() + "'");

                //});

                //element.photobooth().on("image", function (event, dataUrl) {
                //    console.log(dataUrl);
                //    $("#gallery").html('');
                //    $("#gallery").append('<img  src="' + dataUrl + '" >');
                //    $.ajax({
                //        method: 'POST',
                //        url: 'WebCam/Capture',
                //        type: 'json',
                //        contentType: 'application/json',
                //        data: JSON.stringify({ fileData: dataUrl }),
                //        success: function (data) {
                //            console.log(data);
                //            alert(data);
                //        }
                //    })

                //});

                element.on('click', function () {
                    debugger;
                    //scope.photo_url = $('#gallery img').attr('src');
                    console.log(scope.photo_url);
                    scope.photo_url = "Test";
                    //scope.$apply(function () {
                    //    $parse('photo_url').assign(scope.$parent, $('#gallery img').attr('src'));
                    //});
                    if ($('#gallery img').attr('src'))
                        scope.$parent.photoUrlChanged($('#gallery img').attr('src'));

                    console.log(scope);
                    //scope.callback("$('#gallery img')");

                })
            }
        }
    }]);