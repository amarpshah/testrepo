(function (app) {
    'use strict';

    app.controller('addTestCtrl', addTestCtrl);

    addTestCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams'];

    function addTestCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService,  $stateParams) {
        $scope.pageClass = 'page-test';
        $scope.AddTest = AddTest;
        $scope.UpdateTest = UpdateTest;
        $scope.copyData = copyData;
        $scope.find = find;
        $scope.Test = {};
        $scope.newTest = {};

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');
        $scope.TestId = parseInt($stateParams.testId);
        $scope.IsLock = parseInt($stateParams.lockId);

        $scope.IsEditMode = false;

        
      

        $scope.loadvalues = function () {
            

            $scope.Test.status = [{ value: 1, Text: "Draft" },
                                  { value: 2, Text: "Ready" },
                                  { value: 3, Text: "Locked" }
            ];
            $scope.Test.difficulty = [{ value: 1, Text: "Easy" },
                                                  { value: 2, Text: "Medium" },
                                                  { value: 3, Text: "Difficult" }
            ];

           
        }


        $scope.editTest = function ()
        {
            if ($scope.TestId != null && !isNaN($scope.TestId)) {
                $scope.IsEditMode = true;
                $scope.find($scope.TestId);
            }

        }

        $scope.loadvalues();
        $scope.editTest();

        function find(testid) {
            var config = {
                params: {
                    id: testid
                }
            };

            apiService.get(baseUrl + '/api/tests/test/' + testid, null,
                testLoadCompleted,
                testLoadFailed);
        }

        function testLoadCompleted(result) {
            $scope.Tests = result.data;

            if ($scope.Tests != null) {
                $scope.newTest.Code = $scope.Tests[0].Code;
                $scope.newTest.Text = $scope.Tests[0].Text;
                $scope.newTest.Description = $scope.Tests[0].Description;
                $scope.newTest.Objective = $scope.Tests[0].Objective;

                $scope.newTest.Status = $scope.Tests[0].Status;
                $scope.newTest.Difficulty = $scope.Tests[0].DifficultyLevel;
                $scope.newTest.TotalMarks = $scope.Tests[0].TotalMarks;
                $scope.newTest.PassingMarks = $scope.Tests[0].PassingMarks;
            }
        }

        function testLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function copyData() {
            var newTest = {};

            newTest.Code = $scope.newTest.Code;
            newTest.Text = $scope.newTest.Text;
            newTest.Description = $scope.newTest.Description;
            newTest.Objective = $scope.newTest.Objective;

            newTest.Status = $scope.newTest.Status;
            newTest.DifficultyLevel = $scope.newTest.Difficulty;
            newTest.TotalMarks = $scope.newTest.TotalMarks;
            newTest.PassingMarks = $scope.newTest.PassingMarks;

            return newTest;
        }

        function AddTest() {
            
            var newTest = {};
            
            newTest = $scope.copyData();

            apiService.post(baseUrl + '/api/tests/add', newTest,
                    registerQuestionSucceded,
                    registerQuestionFailed);
            
        }

        function registerQuestionSucceded(response) {
            console.log(response);
            $scope.newTest = {};
            notificationService.displayInfo('Data uploaded successfully');
            $scope.loadvalues();
            //$scope.search();

        }

        function registerQuestionFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        function UpdateTest() {

            if ($scope.TestId != null && !isNaN($scope.TestId)) {
                var newTest = {};
                newTest = copyData();
                newTest.ID = $scope.TestId;
                apiService.post(baseUrl + '/api/tests/update', newTest,
                    updateTestSucceded,
                    updateTestFailed);
            }
        }

        function updateTestSucceded(response) {
            console.log(response);
            $scope.newTest = {};
            notificationService.displayInfo('Data Updated successfully');
            $scope.loadvalues();
            //$scope.search();
        }

        function updateTestFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }
    }

})(angular.module('app-administration'));