(function (app) {
    'use strict';

    app.controller('addQuestionPoolCtrl', addQuestionPoolCtrl);

    addQuestionPoolCtrl.$inject = ['$scope', '$modal', '$route', '$routeParams', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', '$cookieStore'];

    function addQuestionPoolCtrl($scope, $modal, $route, $routeParams, apiService, membershipService, webApiLocationService, notificationService, $stateParams, $cookieStore) {

        $scope.question = {};
        $scope.filterQuestion = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.newQuestion = {};
        $scope.Questions = [];
        $scope.QuestionsData = [];
        $scope.PoolQuestions = {};
        $scope.question.questionstandard = [];
        $scope.question.questionsubject = [];
        $scope.ShowList = false;
        $scope.SearchQuestions = SearchQuestions;
        $scope.clearSearch = clearSearch;
        $scope.ShowAdvancedSearch = ShowAdvancedSearch;
        $scope.loadPoolQuestionCnt = loadPoolQuestionCnt;
        $scope.AddQuestionsToPool = AddQuestionsToPool;
        $scope.loadSubject = loadSubject;
        $scope.loadStandard = loadStandard;
        $scope.loadTopic = loadTopic;
        $scope.StandardChange = StandardChange;
        $scope.SubjectChange = SubjectChange;
        $scope.PoolQuestionCnt = 0;

        membershipService.redirectIfNotLoggedIn();
        $scope.PoolId = parseInt($stateParams.poolid);
        $scope.PoolName = ($stateParams.poolname);
        $scope.TestId = parseInt($stateParams.testid);
        $scope.TestName = ($stateParams.testname);
        var baseUrl = webApiLocationService.get('webapi');

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
            console.log(result.data);
            $scope.PoolQuestionCnt = result.data;
        }

        function poolQuestionCntFailed(response) {
            notificationService.displayError(response.data);
        }

        $scope.loadPoolQuestionCnt();
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
            console.log(result.data);
            $scope.question.questionstandard = result.data;
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
            console.log(result.data);
            $scope.question.questiontopic = result.data;
        }

        function topicLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        /////////////////////////////////////////

        function StandardChange(load) {
            if (!isNaN($scope.newQuestion.StandardId)) {
                $scope.newQuestion.StandardName = $("#selectStd option:selected").text();

                var config = {
                    params: {
                        stdid: $scope.newQuestion.StandardId
                    }
                };
                apiService.get(baseUrl + '/api/subjects/filtersubject', config,
                    standardChangeLoadCompleted,
                    standardChangeLoadFailed);

                //Once Standard is changed we need to reset the subject
                if (load == true)
                    $scope.newQuestion.SubjectId = -1;
            }
        }

        function standardChangeLoadCompleted(result) {
            $scope.question.questionsubject = result.data.Items;
            if ($scope.question.questionsubject.length == 0)
                $scope.newQuestion.SubjectId = -1;
        }

        function standardChangeLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function SubjectChange(load) {

            $scope.newQuestion.SubjectName = $("#selectSubject option:selected").text();

            var config = {
                params: {
                    stdid: $scope.newQuestion.StandardId,
                    subid: $scope.newQuestion.SubjectId
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
                $scope.newQuestion.MappingID = Mappings[0]['ID'];
            else
                $scope.newQuestion.MappingID = -1;

            var config = {
                params: {
                    mappingid: $scope.newQuestion.MappingID

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


        $scope.ShowAdvancedSearch();

        //Functions to show Advance search
        function ShowAdvancedSearch() {
            $('.questionControl .panel-heading').toggleClass('showAdvanceAddPoolQuestion');
        }

        function ToggleAddSearch() {
            $('.searchBlock').toggleClass('active');
            $('.addBlock').toggleClass('active');

        }


        function clearSearch() {
            $scope.newQuestion = {};
            $scope.Questions = {};
            $scope.pagesCount = 0;

        }


        //Add Question To Pool
        function AddQuestionsToPool(question) {

            var question2Pool = [];
            if (question) {
                var entry = {}
                entry.QuestionId = question.ID;
                entry.PoolId = $scope.PoolId;
                entry.IsMandatory = false;
                question2Pool.push(entry);

            }
            else {

                for (var i = 0; i < $scope.Questions.length; i++) {
                    if ($scope.Questions[i].IsChecked == true) {
                        var entry = {}
                        entry.QuestionId = $scope.Questions[i].ID;
                        entry.PoolId = $scope.PoolId;
                        entry.IsMandatory = false;
                        question2Pool.push(entry);
                    }
                }
            }
            if (question2Pool.length > 0) {
                apiService.post(baseUrl + '/api/pools/addquestiontopool', question2Pool,
                    addquestionCompleted,
                    addquestionFailed);
            }
        }

        function addquestionCompleted() {
            notificationService.displaySuccess('Question Added Successfully');
            $scope.loadPoolQuestionCnt();
            $scope.SearchQuestions($scope.page);
        }

        function addquestionFailed(response) {
            notificationService.displayError(response.data);
        }


        //Search Questions
        function SearchQuestions(page, searchItem) {
            var item = {};
            if (searchItem) {
                item = searchItem;

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
                        pageSize: 10,
                        code: item.Code,
                        text: item.Text,
                        type: item.Type,
                        topicid: item.TopicId,
                        standardid: item.StandardId,
                        subjectid: item.SubjectId,
                        status: item.Status,
                        difficultyLevel: item.DifficultyLevel,
                        qPoolId: $scope.PoolId
                    }
                };

                apiService.get(baseUrl + '/api/question/searchquestions', config,
                    searchCompleted,
                    searchFailed);

            }
        }

        function searchCompleted(result) {
            console.log(result.data.Items);
            $scope.Questions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.ShowList = true;
        }

        function searchFailed(response) {
            notificationService.displayError(response.data);
        }

    }
})(angular.module('app-administration'));