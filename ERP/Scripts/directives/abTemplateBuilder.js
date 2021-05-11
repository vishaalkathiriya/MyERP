/// <reference path="../../PartialTemplates/ABAddressPrintTemplateGenerate.html" />

//Create Template Directive

angular.module("ERPApp.Directives").directive("abTemplateBuilder", ['$log', '$compile', '$rootScope', "ABPrintService","$http",
function ($log, $compile, $rootScope,ABPrintService,$http) {
    return{
        restrict: 'A',
        transclude: false,
        replace: true,
        scope: {
            //templatedata: "=",
            temptype: "=",
            group: "=",
           
        },
        link: function (scope, element, attrs, ctrl) {
           
            scope.AddressList = [];
            scope.TemplateList = [];
            var temp = "";
    
            scope.addressShow = function (line) {
                if (line == "" || line == null) {
                    return false;
                }
                return true;
            }
            scope.GeneateTemplate = function () {

                for (var i = 0; i < scope.TemplateList.length; i++) {
                    
                    var temp = scope.TemplateList[i];
                    if (scope.temptype == temp.TemplateId) {
                     
                        scope.template = temp.TemplateFormate;
                        

                        scope.maintgStart = "div";
                        scope.maintgStartattr = " class='addresslist' style='border:1px solid Red;'";
                        scope.maintgStart += scope.maintgStartattr;
                       
                        $(".template").html("");
                        //  console.log(scope.AddressList);
                        for (var i = 0; i < scope.AddressList.length; i++) {
                           
                            var a = scope.AddressList[i];

                            showcmp = scope.addressShow(a.CompanyName);
                            showAdd1 = scope.addressShow(a.Address1);
                            showAdd2 = scope.addressShow(a.Address2);
                            showArea = scope.addressShow(a.Area);
                            showCity = scope.addressShow(a.City);
                            showPin = scope.addressShow(a.Pincode);
                            showPhone = scope.addressShow(a.PhoneNo);
                            showLandline = scope.addressShow(a.LandlineNo);
                            temp = "<" + scope.maintgStart + ">";
                            temp += a.LangId == 2 ? "<div class='guj-scope' style='display: inline-block'>" : "<div style='display: inline-block'>";
                            temp += scope.template;
                            temp = temp.replace("{{Name}}", a.Name);
                            temp = showcmp ? temp.replace("{{cmpName}}", a.CompanyName) : temp;
                            temp = showAdd1 ? temp.replace("{{Address1}}", a.Address1) : temp;
                            temp = showAdd2 ? temp.replace("{{Address2}}", a.Address2) : temp;
                            temp = showArea ? temp.replace("{{Area}}", a.Area) : temp;
                            temp = showCity ? temp.replace("{{City}}", a.City) : temp;
                            temp = showPin ? temp.replace("{{PinCode}}", a.Pincode) : temp;
                            temp = showPhone ? temp.replace("{{PhoneNo}}", a.PhoneNo) : temp;
                            temp = showLandline ? temp.replace("{{LandlineNo}}", a.LandlineNo) : temp;
                            temp += "</div>" + "</" + scope.maintgStart + ">";;
                           
                            $(".template").append($compile(temp)(scope));

                        }

                    }
                }
            }
          
            scope.$watch('temptype', function (newValue) {
                ABPrintService.TemplateList().then(function (result) {
                    scope.TemplateList = result.data.DataList;
                    scope.GeneateTemplate()

                });
                scope.GeneateTemplate()
            
            });
            scope.$watch('group', function (newValue) {
                ABPrintService.PrintAddress(newValue).then(function (result) {
                    scope.AddressList = result.data.DataList.result;
                    scope.GeneateTemplate()
                 
                });
            });
        
        },
        templateUrl: "../PartialTemplates/ABAddressPrintTemplateGenerate.html"
}
       // return directive;
}]);