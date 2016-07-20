(function (app) {
    'use strict';

    app.controller('viewQuestionPoolCtrl', viewQuestionPoolCtrl);

    viewQuestionPoolCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', 'permissionService', 'constantStrService'];

    function viewQuestionPoolCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams, permissionService, constantStrService) {

        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.PoolQuestionCnt = 0;
        $scope.PoolId = parseInt($stateParams.poolid);
        $scope.PoolName = ($stateParams.poolname);
        $scope.TestName = ($stateParams.testname);
        $scope.TestId = parseInt($stateParams.testid);
        $scope.question = {};
        $scope.Questions = [];
        $scope.loadPoolQuestionCnt = loadPoolQuestionCnt;
        $scope.search = search;
        $scope.clearSearch = clearSearch;
        $scope.RemoveQuestionsFromPool = RemoveQuestionsFromPool;
        $scope.ShowAdvancedSearch = ShowAdvancedSearch;
        $scope.advancedSearch = advancedSearch;
        $scope.loadStandard = loadStandard;
        $scope.loadSubject = loadSubject;
        $scope.loadTopic = loadTopic;
        $scope.StandardChange = StandardChange;
        $scope.SubjectChange = SubjectChange;
        $scope.SearchText = {};

        var baseUrl = webApiLocationService.get('webapi');
        $scope.permissionADDPOOLQUESTIONMAP = permissionService.get(constantStrService.ADD_POOL_QUESTION_MAP());
        $scope.permissionDELETEPOOLQUESTIONMAP = permissionService.get(constantStrService.DELETE_POOL_QUESTION_MAP());


        membershipService.redirectIfNotLoggedIn();
        $scope.question.questiontype = [
                                                  { value: 1, Text: "Descriptive", type: "DES" },
                                                  { value: 2, Text: "True or False", type: "TOF" },
                                                  { value: 3, Text: "Match the following", type: "MTF" },
                                                  { value: 4, Text: "Single choice", type: "SCQ" },
                                                  { value: 5, Text: "Multiple choice", type: "MCQ" },
                                                  { value: 6, Text: "Fill in the blanks", type: "FTB" }
        ];

        $scope.question.questionstatus = [{ value: 1, Text: "Draft" },
                                            { value: 2, Text: "Ready" },
                                            { value: 3, Text: "Locked" }
        ];

        $scope.question.questiondifficulty = [
                                                { value: 1, Text: "Easy" },
                                                { value: 2, Text: "Medium" },
                                                { value: 3, Text: "Difficult" }
        ];




        function RemoveQuestionsFromPool(questionid)
        {
            var question2Pool = [];
            if (questionid) {
                var entry = {};
                entry.QuestionId = questionid;
                entry.PoolId = $scope.PoolId;
                question2Pool.push(entry);

            }
            else {
                for (var i = 0; i < $scope.Questions.length; i++) {
                    if ($scope.Questions[i].IsChecked == true) {
                        var entry = {}
                        entry.QuestionId = $scope.Questions[i].ID;
                        entry.PoolId = $scope.PoolId;
                        //entry.IsMandatory = false;
                        question2Pool.push(entry);
                    }
                }
            }
            if (question2Pool.length > 0) {
                apiService.post(baseUrl + '/api/pools/removequestionpoolmap/', question2Pool,
                removePoolQuestionCompleted,
                removePoolQuestionFailed);
            }
        }
        function removePoolQuestionCompleted(result) {
            /*$scope.Standards = result.data;*/
            console.log(result.data);
            notificationService.displaySuccess('Records removed successfully');
            $scope.search();
            $scope.loadPoolQuestionCnt();
            //StandardChange(false);
        }

        function removePoolQuestionFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadPoolQuestionCnt() {

            var config = {
                params: {
                    poolid: $scope.PoolId
                }
            };
            apiService.get(baseUrl + '/api/pools/questionpoolcnt/', config,
               poolQuestionCntCompleted,
               poolQuestionCntFailed);
        }
        function poolQuestionCntCompleted(result) {
            /*$scope.Standards = result.data;*/
            console.log(result.data);
            $scope.PoolQuestionCnt = result.data;
            //StandardChange(false);
        }

        function poolQuestionCntFailed(response) {
            notificationService.displayError(response.data);
        }

        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;
                var config = {
                    params: {
                        page: page,
                        pageSize: 4,
                        poolid: $scope.PoolId
                    }
                };
                apiService.get(baseUrl + '/api/pools/questionpool/', config,
                   poolQuestionCompleted,
                   poolQuestionFailed);

            }
            else {

                $scope.advancedSearch(page, searchItem);
            }
        }
        function poolQuestionCompleted(result) {
            /*$scope.Standards = result.data;*/
            console.log(result.data);
            $scope.Questions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            //StandardChange(false);
        }

        function poolQuestionFailed(response) {
            notificationService.displayError(response.data);
        }

        $scope.loadPoolQuestionCnt();
        $scope.search();


        $scope.loadStandard();
        $scope.loadSubject();
        $scope.loadTopic();


        StandardChange(false);
        SubjectChange(false);




        /////////////////////////////////////////////////////

        function loadStandard() {
            apiService.get(baseUrl + '/api/standards/', null,
                standardLoadCompleted,
                standardLoadFailed);
        }

        function standardLoadCompleted(result) {
            /*$scope.Standards = result.data;*/
            console.log(result.data);
            $scope.question.questionstandard = result.data;

            //StandardChange(false);
        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadSubject() {
            apiService.get(baseUrl + '/api/subjects/', null,
                subjectLoadCompleted,
                subjectLoadFailed);
        }

        function subjectLoadCompleted(result) {
            console.log(result.data);
            $scope.question.questionsubject = result.data;

        }

        function subjectLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadTopic() {
            apiService.get(baseUrl + '/api/topic/', null,
                topicLoadCompleted,
                topicLoadFailed);
        }

        function topicLoadCompleted(result) {
            //$scope.Topics = result.data;
            console.log(result.data);
            $scope.question.questiontopic = result.data;
        }

        function topicLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        /////////////////////////////////////////

        function StandardChange(load) {
            if (!isNaN($scope.SearchText.StandardId)) {
              //  $scope.newQuestion.StandardName = $("#selectStd option:selected").text();

                var config = {
                    params: {
                        stdid: $scope.SearchText.StandardId
                    }
                };
                apiService.get(baseUrl + '/api/subjects/filtersubject', config,
                    standardChangeLoadCompleted,
                    standardChangeLoadFailed);

                //Once Standard is changed we need to reset the subject
                if (load == true)
                    $scope.SearchText.SubjectId = -1;


            }
        }

        function standardChangeLoadCompleted(result) {
            $scope.question.questionsubject = result.data.Items;
            if ($scope.question.questionsubject.length == 0)
                $scope.SearchText.SubjectId = -1;
        }

        function standardChangeLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function SubjectChange(load) {

            //$scope.newQuestion.SubjectName = $("#selectSubject option:selected").text();

            var config = {
                params: {
                    stdid: $scope.SearchText.StandardId,
                    subid: $scope.SearchText.SubjectId
                }
            };

            if (load == true)

                apiService.get(baseUrl + '/api/mapping/filtermappings/', config,
                    mappingLoadCompleted,
                    mappingLoadFailed);

        }

        function mappingLoadCompleted(result) {
            var Mappings = result.data.Items;

            if (Mappings.length > 0)
                $scope.SearchText.MappingID = Mappings[0]['ID'];
            else
                $scope.SearchText.MappingID = -1;

            var config = {
                params: {
                    mappingid: $scope.SearchText.MappingID

                }
            };

            apiService.get(baseUrl + '/api/topic/filtertopics', config,

            subjectChangeLoadCompleted,
              subjectChangeLoadFailed);

        }

        function mappingLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function subjectChangeLoadCompleted(result) {
            var topic = result.data.Items;
            $scope.question.questiontopic = result.data.Items;

        }

        function subjectChangeLoadFailed(response) {
            notificationService.displayError(response.data);
        }




        //Functions to show Advance search
        function ShowAdvancedSearch() {
            $('.questionControl .panel-heading').toggleClass('showAdvance');
            $scope.search();
        }

        function ToggleAddSearch() {
            $('.searchBlock').toggleClass('active');
            $('.addBlock').toggleClass('active');

        }

        //Advance Search start ------------->

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {

                if (angular.isUndefined(item.Code)) {
                    item.Code = "";
                }
                if (angular.isUndefined(item.Text)) {
                    item.Text = "";
                }
                if (angular.isUndefined(item.Type)) {
                    item.Type = -1;
                }
                if (angular.isUndefined(item.Status)) {
                    item.Status = -1;
                }
                if (angular.isUndefined(item.TopicId)) {
                    item.TopicId = -1;
                }
                if (angular.isUndefined(item.StandardId)) {
                    item.StandardId = -1;
                }
                if (angular.isUndefined(item.SubjectId)) {
                    item.SubjectId = -1;
                }
                if (angular.isUndefined(item.DifficultyLevel)) {
                    item.DifficultyLevel = -1;
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 4,
                        code: item.Code,
                        text: item.Text,
                        type: item.Type,
                        topicid: item.TopicId,
                        standardid: item.StandardId,
                        subjectid: item.SubjectId,
                        status: item.Status,
                        difficultyLevel: item.DifficultyLevel,
                        poolid: $scope.PoolId
                    }
                };


                apiService.get(baseUrl + '/api/pools/questionadvancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Questions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingQuestion = false;

            if ($scope.filterQuestion && $scope.filterQuestion.length) {
                notificationService.displayInfo(result.data.Items.length + ' questions found');
            }

        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }
        //End ------------->


    }
})(angular.module('app-administration'));