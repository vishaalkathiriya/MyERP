
//Directive for excel upload

//Dependencies
//Uploadify jQuery Plugin

//Parameters
//data-AB-Excel-Upload          = Initialize directive
//data-handler-path             = Specify handler path
//data-temp-save-path           = Temporary excel save path
//data-allowed-files            = Specify all allowed files "*.xls,*.xlsx"
//data-first-row-column-name    = Specify first row is column names or not
//data-button-text              = Specify text for button
//data-callback-fun             = Specify callback function of controller, this function will call automatically with data after uploading finished

angular.module("ERPApp.Directives").directive("excelUpload", ['$log', '$compile', '$rootScope',
    function ($log, $compile, $rootScope) {
        
        var directive = {
            restrict: 'A',
            transclude: false,
            replace: true,
            scope: {
                filename: "=",
                id: "@",
                uploadhandler: "@",
                buttonclass: "@",
                ext: "@",
                filesize: "@",
                ischanged: "="
            },
            link: function (scope, element, attrs, ctrl) {
                              $log.debug("Init caUploader");
                var setFileName = function (name) {
                    $rootScope.$safeApply(scope, function () {
                        scope.ischanged = true;
                        scope.filename = name;
                    });
                };

                $(element.find("input").first()).attr('id', scope.id).uploadify({
                    'uploader': scope.uploadhandler,
                    'swf': "/content/js/uploadify.swf",
                    'cancelImg': '',
                    'folder': "/data",
                    'buttonClass': scope.buttonclass,
                    'queueID': "queue",
                    'auto': false,
                    'multi': false,
                    'buttonImg': '',
                    'width': '100%',
                    'height': '34',
                    'buttonText': 'SELECT',
                    'fileSizeLimit': scope.filesize,
                    'fileTypeDesc': 'Image Files',
                    'fileTypeExts': scope.ext,
                    'onSelect': function (e, q, f) {
                        $log.debug("On select");
                    },
                    'onUploadError': function (event, ID, fileObj, errorObj) {
                        alert(errorObj.type + ' Error: ' + errorObj.info);
                    },
                    'onUploadSuccess': function (file, data, response) {
                        //setFileName($.parseJSON(data));
                        setFileName(data);
                    }
                });

                //FOR MANUALL UPLOAD
                //$(element).after('<p><a href="javascript:$(\'#'+ scope.id +'\').uploadify(\'upload\')">Upload Files</a></p>');
            }
        };
        return directive;
    }]);