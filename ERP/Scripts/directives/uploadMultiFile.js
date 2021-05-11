angular.module("ERPApp.Directives")
    .directive('uploadMultiFile', ['$log', '$compile', '$rootScope', '$timeout', '$q', function ($log, $compile, $rootScope, $timeout, $q) {
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
                oldmultifilelist: "="
            },
            link: function (scope, element, attrs, ctrl) {

                scope.fileList = [];
                var setFileName = function (name) {
                    var part = name.split("-"); // added to display original file name in caption text by default after uploading
                    var val = part[0].split(".");
                    scope.fileList.push({ PKDocId: 0, file: part[0], caption: part[1], ext: val[1] });
                };

                $(element.find("input").first()).attr('id', scope.id).uploadify({
                    'uploader': scope.uploadhandler,
                    'swf': "/content/js/uploadify.swf",
                    'cancelImg': '',
                    'folder': "/data",
                    'buttonClass': scope.buttonclass,
                    //'queueID': "queue",
                    'auto': true,
                    'multi': true,
                    'buttonImg': '',
                    'width': '100%',
                    'height': '34',
                    'buttonText': 'Click here to upload files',
                    'fileSizeLimit': scope.filesize,
                    'fileTypeDesc': 'Files',
                    'fileTypeExts': scope.ext,
                    'onSelect': function (e, q, f) {
                        $log.debug("On select");
                        scope.fileList = scope.filename; //added for reset problem in edit mode
                    },
                    'onUploadError': function (event, ID, fileObj, errorObj) {
                        alert(errorObj.type + ' Error: ' + errorObj.info);
                    },
                    'onUploadSuccess': function (file, data, response) {
                        setFileName(data);
                    },
                    'onQueueComplete': function (queueData) {
                        if (scope.oldmultifilelist && scope.oldmultifilelist.length > 0) {
                            angular.forEach(scope.oldmultifilelist, function (value, key) {
                                var exists = false;
                                var prom = [];
                                angular.forEach(scope.fileList, function (v1, k1) {
                                    if (value.PKDocId == v1.PKDocId) {
                                        exists = true;
                                    }
                                    prom.push(v1);
                                });

                                $q.all(prom).then(function () {
                                    if (!exists) {
                                        scope.fileList.push({
                                            'PKDocId': value.PKDocId, //for identify newly added images in edit mode
                                            'file': value.file,
                                            'caption': value.caption,
                                            'ext': value.ext
                                        });
                                    }
                                });
                            });
                        }

                        $rootScope.$safeApply(scope, function () {
                            scope.filename = scope.fileList;
                        });
                    }
                });
            }
        };
        return directive;
    }]);