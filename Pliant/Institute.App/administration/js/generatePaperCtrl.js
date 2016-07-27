(function (app) {
    'use strict';

    app.controller('generatePaperCtrl', generatePaperCtrl);

    generatePaperCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', '$cookieStore'];

    function generatePaperCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams, $cookieStore) {

        $scope.newPaper = {};
        $scope.addPaper = addPaper;
        $scope.loadPools = loadPools;
        $scope.loadPaper = loadPaper;
        $scope.Questions = {};
        $scope.Pool = {};
        $scope.Pools = {};
        $scope.setID = -1;
        $scope.IsFinalized = false;
        $scope.ShowPaper = false;
        $scope.ValidatePaperSets = ValidatePaperSets;
        membershipService.redirectIfNotLoggedIn();

        $scope.TestName = ($stateParams.testname)
        $scope.TestId = parseInt($stateParams.testid);
        var userInfo = $cookieStore.get('repository');
        $scope.userName = userInfo.loggedUser.username;

        var userId = userInfo.loggedUser.userid;
        var baseUrl = webApiLocationService.get('webapi');


        function addPaper() {
            if ($scope.ValidatePaperSets()) {
                var newPaper = {};
                newPaper.TestID = $scope.TestId
                newPaper.TestName = $scope.TestName
                newPaper.NoOfSets = $scope.newPaper.NoOfSets;
                newPaper.Description = $scope.newPaper.Description;

                newPaper.CreatedBy = userId;

                apiService.post(baseUrl + '/api/papers/add', newPaper,
                     addPaperSucceded,
                     addPaperFailed);
            }

        }

        function addPaperSucceded(response) {
            console.log(response);

            notificationService.displayInfo('Question Paper Generated');

        }

        function addPaperFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        function ValidatePaperSets() {
            if (isNaN($scope.newPaper.NoOfSets)) {
                $scope.vNoOfSets = true;
            }
            if (angular.isUndefined($scope.newPaper.Description)) {
                $scope.vDescription = true;

            }
            if ($scope.PoolCount < 1) {
                $scope.vPoolCount = true;
                return false;
            }
            else {

                return true;
            }

        }

        $scope.loadPools();
        $scope.loadPaper();

        function loadPools() {
            if (isNaN($scope.TestId)) {
                $scope.TestId = -1;
            }
            var config = {
                params: {
                    testid: $scope.TestId
                }
            };

            apiService.get(baseUrl + '/api/pools/filtertests/', config,
                poolsLoadCompleted,
                poolsLoadFailed);
        }

        function poolsLoadCompleted(result) {
            $scope.Pools = result.data.Items;
            $scope.PoolCount = result.data.Count;

        }

        function poolsLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadPaper() {

            if (isNaN($scope.TestId)) {
                $scope.TestId = -1;
            }
            var config = {
                params: {
                    testid: $scope.TestId
                }
            };
            apiService.get(baseUrl + '/api/papers/filtertests/', config,
               paperLoadCompleted,
               paperLoadFailed);
        }

        function paperLoadCompleted(result) {
            var data = result.data[0];
            if (data) {
                $scope.newPaper.NoOfSets = data.NoOfSets;
                $scope.newPaper.Description = data.Description;
                $scope.newPaper.PaperID = data.ID;
                $scope.IsFinalized = data.IsFinalized;
                $scope.ShowPaper = true;

            }
        }

        function paperLoadFailed(response) {
            notificationService.displayError(response.data);
        }

    }

})(angular.module('app-administration'));