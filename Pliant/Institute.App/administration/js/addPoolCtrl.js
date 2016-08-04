(function (app) {
    'use strict';

    app.controller('addPoolCtrl', addPoolCtrl);

    addPoolCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams'];

    function addPoolCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams) {
        $scope.pageClass = 'page-test';
        $scope.AddPool = AddPool;
        var baseUrl = webApiLocationService.get('webapi');
        $scope.Test = {};
        $scope.newPool = {};
        $scope.Pools = {};
        $scope.IsEditMode = false
        $scope.editPool = editPool;
        $scope.updatePool = updatePool
        $scope.copyData = copyData;
        $scope.find = find;
        $scope.ValidatePool = ValidatePool;
        
        membershipService.redirectIfNotLoggedIn();
        $scope.PoolId = parseInt($stateParams.poolid);
        $scope.TestId = parseInt($stateParams.testid);
        $scope.TestName = ($stateParams.testname);
        $scope.IsLock = parseInt($stateParams.islock);

        $scope.loadvalues = function () {
            $scope.Test.status = [{ value: 1, Text: "Draft" },
                                  { value: 2, Text: "Ready" },
                                  { value: 3, Text: "Locked" }
            ];
            $scope.Test.difficulty = [            { value: 1, Text: "Easy" },
                                                  { value: 2, Text: "Medium" },
                                                  { value: 3, Text: "Difficult" }
            ];
        }

        $scope.loadvalues();
         
        function AddPool() {
            if ($scope.ValidatePool()) {

                var newPool = {};

                if (isNaN($scope.TestId)) {
                    notificationService.displayError("Need to map it to valid Test object.");
                }
                else {
                    newPool.Name = $scope.newPool.Name;
                    newPool.TestID = $scope.TestId;
                    newPool.Status = $scope.newPool.Status;
                    newPool.DifficultyLevel = $scope.newPool.DifficultyLevel;
                    newPool.NoOfQuestionsOutOf = $scope.newPool.NoOfQuestionsOutOf;
                    newPool.PoolTotalMarks = $scope.newPool.PoolTotalMarks;
                    newPool.PassingScore = $scope.newPool.PassingScore;
                    if ($scope.newPool.IsMandatoryToPass == true)
                        newPool.IsMandatoryToPass = 1;
                    else
                        newPool.IsMandatoryToPass = 0;

                    apiService.post(baseUrl + '/api/pools/add', newPool,
                        addPoolSucceded,
                        addPoolFailed);
                }
            }
        }

        function addPoolSucceded(response) {
            console.log(response);
            $scope.newPool = {};
            notificationService.displayInfo('Data uploaded successfully');
            $scope.loadvalues();
            

        }

        function addPoolFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        function ValidatePool()
        {
            var flag = true;
            if (angular.isUndefined($scope.newPool.Name)) {
                $scope.vName = true;
                flag = false;
            }
            if (isNaN($scope.newPool.Status)) {
                $scope.vStatus = true;
                flag = false;
            }
            if (isNaN($scope.newPool.DifficultyLevel)) {
                $scope.vDifficultyLevel = true;
                flag = false;
            }
            if (isNaN($scope.newPool.PassingScore)) {
                $scope.vPassingScore = true;
                flag = false;
            }
            if (isNaN($scope.newPool.PoolTotalMarks)) {
                $scope.vPoolTotalMarks = true;
                flag = false;
            }
            if (flag) {
                return true;
            }
            else {
                return false;
            }
        }
        $scope.editPool();
        function editPool(){
            if ($scope.PoolId != null && !isNaN($scope.PoolId)) {
                $scope.IsEditMode = true;
                $scope.find($scope.PoolId);
            }
        }
        function find(poolid) {
            var config = {
                params: {
                    id: poolid
                }
            };

            apiService.get(baseUrl + '/api/pools/pool/' + poolid, null,
                poolLoadCompleted,
                poolLoadFailed);
        }
        function poolLoadCompleted(result) {
            $scope.Pools = result.data[0];

            if ($scope.Pools != null) {
                $scope.newPool.Name = $scope.Pools.Name;
                $scope.newPool.Status = $scope.Pools.Status;
                $scope.newPool.DifficultyLevel = $scope.Pools.DifficultyLevel;
                $scope.newPool.NoOfQuestionsOutOf = $scope.Pools.NoOfQuestionsOutOf;
                $scope.newPool.PoolTotalMarks = $scope.Pools.PoolTotalMarks;
                $scope.newPool.PassingScore = $scope.Pools.PassingScore;
                if ($scope.Pools.IsMandatoryToPass == 1) {
                    $scope.newPool.IsMandatoryToPass = true;
                }
                else {
                    $scope.newPool.IsMandatoryToPass = false;

                }


            }
        }

        function poolLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function copyData() {
            var newPool = {};

            newPool.Name = $scope.newPool.Name;
            newPool.TestID = $scope.TestId;
            newPool.Status = $scope.newPool.Status;
            newPool.DifficultyLevel = $scope.newPool.DifficultyLevel;
            newPool.NoOfQuestionsOutOf = $scope.newPool.NoOfQuestionsOutOf;
            newPool.PoolTotalMarks = $scope.newPool.PoolTotalMarks;
            newPool.PassingScore = $scope.newPool.PassingScore;
            if ($scope.newPool.IsMandatoryToPass == true) {
                newPool.IsMandatoryToPass = 1;
            }
            else {
                newPool.IsMandatoryToPass = 0;
            }
            return newPool;
        }


        function updatePool() {

            if ($scope.PoolId != null && !isNaN($scope.PoolId)) {
                var newPool = {};
                newPool = copyData();
                newPool.ID = $scope.PoolId;
                
                apiService.post(baseUrl + '/api/pools/update', newPool,
                    updatePoolSucceded,
                    updatePoolFailed);
            }
        }

        function updatePoolSucceded(response) {
            console.log(response);
            
            notificationService.displayInfo('Data Updated successfully');
            $scope.loadvalues();
            
        }

        function updatePoolFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }
    }

})(angular.module('app-administration'));