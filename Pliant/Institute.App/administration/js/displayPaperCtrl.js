(function (app) {
    'use strict';

    app.controller('displayPaperCtrl', displayPaperCtrl);

    displayPaperCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', '$cookieStore'];

    function displayPaperCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams, $cookieStore) {

        $scope.Questions = {};
        $scope.TestID = null;
        $scope.TestName = "";
        $scope.loadPaper = loadPaper;
        $scope.finalize = finalize;

        membershipService.redirectIfNotLoggedIn();

        $scope.IsFinalized = ($stateParams.isfinalized);
        $scope.PaperID = parseInt($stateParams.paperid);
        var baseUrl = webApiLocationService.get('webapi');

        $scope.loadPaper();
        function loadPaper() {
            if (isNaN($scope.PaperID)) {
                $scope.PaperID = -1;
            }
            var config = {
                params: {
                    paperid: $scope.PaperID
                }
            };
            apiService.get(baseUrl + '/api/papers/filterpaper/', config,
               paperLoadCompleted,
               paperLoadFailed);
        }

        function paperLoadCompleted(result) {
            $scope.Questions = result.data;
            if ($scope.Questions) {
                $scope.TestID = $scope.Questions[0].TestSetPools[0].TestSetQuestions[0].TestID;
                $scope.TestName = $scope.Questions[0].TestSetPools[0].TestSetQuestions[0].TestName;
            }

        }

        function paperLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function finalize() {

            if (isNaN($scope.PaperID)) {
                $scope.PaperID = -1;
            }
            var config = {
                params: {
                    paperid: $scope.PaperID
                }
            };
            apiService.get(baseUrl + '/api/papers/finalize/', config,
               finalizeCompleted,
               finalizeFailed);
        }

        function finalizeCompleted(response) {
            console.log(response);
            notificationService.displayInfo('Question Paper Finalized');

        }


        function finalizeFailed(response) {
            notificationService.displayError(response.data);
        }


    }

})(angular.module('app-administration'));