
angular.module("ERPApp.Controllers").controller("ABPrintAddressCtrl", [
"$scope",
"$rootScope",
"ABPrintService",
"$timeout",
"$compile",
function ($scope, $rootScope, ABPrintService,$timeout,$compile) {
  
    $scope.isFormVisible = false;
    $scope.SaveText = "Set";
    $scope.editData = $scope.editData || {};
    $scope.editData.Group = 0;
    $scope.editData.Template = $scope;
    $scope.SetFeature = function ()
    {
        $scope.isFormVisible = true;
        $scope.SaveText = "Set";       
    }
    function DefaultStyle()
    {
        $scope.height = 110;
        $scope.width = 33;
        $scope.margin = 1;
        $scope.fontSize = 12;
        $scope.padding = 5;
        $scope.lineHeight = 5;

    }
    DefaultStyle();
    $scope.$watch('editData.Group', function (newValue) {
        ABPrintService.PrintAddress(newValue).then(function (result) {
            $scope.AddressList = result.data.DataList.result;
            return false;
        });
    });
  
    // Retrive Group
    function GroupList() {
        ABPrintService.GroupList().then(function (result) {
            $scope.GroupList = result.data.DataList;
            $timeout(function () {
                $scope.editData.Group = 0;
            },1000);
             
        });
    }
    GroupList();

    // Retrive Template
    function TemplateList() {
        ABPrintService.TemplateList().then(function (result) {
            $scope.TemplateList = result.data.DataList;
            $timeout(function () {
                $scope.editData.Template = $scope.TemplateList[0].TemplateId;
            }, 1000);

        });
    }
    TemplateList();


    $scope.addressShow = function (line) {
        if (line =="" || line==null) {
            return false;
        }
        return true;
    }
    $scope.CloseFeature = function ()
    {
        $scope.isFormVisible = false;
        $scope.SaveText = "Set";
    
    }
    $scope.PrintAddress = function ()
    {
        window.print();
    }
    function SetToCSS() {
        $('.addresslist').css({
            'width': $scope.width + '%',
            'line-height': $scope.lineHeight + 'px',
            'margin': $scope.margin + 'px',
            'height': $scope.height + 'px',
            'padding': $scope.padding,
            'font-size': $scope.fontSize + 'px',
        });
        $('.addresslist p').css({
            'font-size': $scope.fontSize + 'px',
        });
        $('.addresslist h1,.addresslist h2,.addresslist h3,.addresslist h4,.addresslist h5,.addresslist h6').css({
            'font-size': $scope.fontSize + 'px',
        });
    }
    $scope.ResetFeature = function ()
    {
        DefaultStyle();
        SetToCSS();
    }
    $scope.StyleForm = function ()
    {
        SetToCSS();
    }

    $scope.Set3by8 = function ()
    {
        $scope.height = 110;
        $scope.width = 33;
        $scope.margin = 1;
        $scope.fontSize = 12;
        $scope.padding = 5;
        $scope.lineHeight = 5;
        SetToCSS();
    }
    $scope.Set3by6 = function () {
        $scope.height = 140;
        $scope.width = 33;
        $scope.margin = 1;
        $scope.fontSize = 14;
        $scope.padding = 5;
        $scope.lineHeight = 8;
        SetToCSS();
    }

   
}
]);