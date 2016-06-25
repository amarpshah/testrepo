(function (app) {
    'use strict';

    app.factory('fileUploadService', fileUploadService);

    fileUploadService.$inject = ['$rootScope', '$http', '$timeout', '$upload', 'notificationService'];

    function fileUploadService($rootScope, $http, $timeout, $upload, notificationService) {

        $rootScope.upload = [];

        var service = {
            uploadImage: uploadImage
        }

        function uploadImage($files, uploadtype  ,movieId, callback) {
            //$files: an array of files selected
            for (var i = 0; i < $files.length; i++) {
                var $file = $files[i];
                (function (index) {
                    var strURL = "";
                    if (uploadtype == "movies") {
                        strURL = "api/movies/images/upload?movieId=" + movieId;
                    }
                    else {
                        strURL = "api/students/images/upload?studentId=" + movieId;
                    }
                    $rootScope.upload[index] = $upload.upload({
                        //url: "api/movies/images/upload?movieId=" + movieId, // webapi url
                        url: strURL,
                        method: "POST",
                        file: $file
                    }).progress(function (evt) {
                    }).success(function (data, status, headers, config) {
                        // file is uploaded successfully
                        notificationService.displaySuccess(data.FileName + ' uploaded successfully');
                        callback();
                    }).error(function (data, status, headers, config) {
                        notificationService.displayError(data.Message);
                    });
                })(i);
            }
        }

        function uploadxlfile($files)
        {
            //$files: an array of files selected
            for (var i = 0; i < $files.length; i++) {
                var $file = $files[i];
                (function (index) {
                    var strURL = "";
                        strURL = "api/students/files/upload" ;
                    $rootScope.upload[index] = $upload.upload({
                        //url: "api/movies/images/upload?movieId=" + movieId, // http://localhost:1487/Controllers/MoviesController.cswebapi url
                        url: strURL,
                        method: "POST",
                        file: $file
                    }).progress(function (evt) {
                    }).success(function (data, status, headers, config) {
                        // file is uploaded successfully
                        notificationService.displaySuccess(data.FileName + ' uploaded successfully');
                    }).error(function (data, status, headers, config) {
                        notificationService.displayError(data.Message);
                    });
                })(i);
            }
        }

        return service;
    }

})(angular.module('common.core'));