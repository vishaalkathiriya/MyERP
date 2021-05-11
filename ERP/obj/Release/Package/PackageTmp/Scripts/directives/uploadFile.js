angular.module("ERPApp.Directives")
    .directive('uploadFile', ['$log', '$compile', '$rootScope', function ($log, $compile, $rootScope) {
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
                        if (scope.ischanged){
                            scope.ischanged = true;
                        }
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
                    'auto': true,
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