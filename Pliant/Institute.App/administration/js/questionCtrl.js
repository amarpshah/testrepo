(function (app) {
    'use strict';

    app.controller('questionCtrl', questionCtrl);

    questionCtrl.$inject = ['$scope', '$modal', '$route', '$routeParams', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', '$cookieStore', 'permissionService', 'constantStrService'];

    function questionCtrl($scope, $modal, $route, $routeParams, apiService, membershipService, webApiLocationService, notificationService, $stateParams, $cookieStore, permissionService, constantStrService) {

        $scope.question = {};
        $scope.filterQuestion = '';
        $scope.page = 0;
        $scope.newQuestion = {};
        $scope.Questions = [];
        $scope.question.questionstandard = [];
        $scope.question.questionsubject = [];
        $scope.search = search;
        $scope.clearSearch = clearSearch;
        $scope.deleteQuestion = deleteQuestion;
        $scope.ShowAdvancedSearch = ShowAdvancedSearch;
        $scope.advancedSearch = advancedSearch;
        $scope.SearchText = {};
        $scope.questionLock = questionLock;
        $scope.lock = lock;
        $scope.unLock = unLock;
        $scope.showSelected = showSelected;
        $scope.loadSubject = loadSubject;
        $scope.loadStandard = loadStandard;
        $scope.loadTopic = loadTopic;
        $scope.StandardChange = StandardChange;
        $scope.SubjectChange = SubjectChange;

        membershipService.redirectIfNotLoggedIn();

        var userInfo = $cookieStore.get('repository');
        $scope.userName = userInfo.loggedUser.username;
        var userId = userInfo.loggedUser.userid;
        $scope.checkId = userId;

        var baseUrl = webApiLocationService.get('webapi');
        $scope.permissionADDQUESTION = permissionService.get(constantStrService.ADD_QUESTION());
        $scope.permissionUPDATEQUESTION = permissionService.get(constantStrService.UPDATE_QUESTION());
        $scope.permissionDELETEQUESTION = permissionService.get(constantStrService.DELETE_QUESTION());
        $scope.permissionLOCKQUESTION = permissionService.get(constantStrService.LOCK_QUESTION());


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



        $scope.search();


        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;
                $scope.loadingQuestion = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 10,
                        filter: $scope.filterQuestion
                    }
                };

                apiService.get(baseUrl + '/api/question/search/', config,
                    questionLoadCompleted,
                    questionLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem);
            }
        }

        function questionLoadCompleted(result) {
            $scope.Questions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingQuestion = false;
            if ($scope.filterQuestion && $scope.filterQuestion.length) {
                notificationService.displayInfo(result.data.Items.length + ' questions found');
            }
        }

        function questionLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //Delete Question
        function deleteQuestion(questionid) {

            if (questionid != null) {
                var config = {
                    params: {
                        id: questionid
                    }
                };
                apiService.post(baseUrl + '/api/question/delete/' + questionid, null,
            deleteSucceded,
            deleteFailed);
            }
        }


        function deleteSucceded(response) {
            console.log(response);
            notificationService.displayInfo('Deleted successfully');
        }

        function deleteFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

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
            if (!isNaN($scope.SearchText.StandardId)) {
                $scope.newQuestion.StandardName = $("#selectStd option:selected").text();

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

            $scope.newQuestion.SubjectName = $("#selectSubject option:selected").text();

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

        //*****************Lock/Unlock*********************

        function questionLock(value) {

            if (value.IsLock == 1 && value.LockedBy == userId) {   //Unlock Test if valid user
                value.LockedBy = 0;
                unLock(value);
            }
            else if (value.IsLock == 0) {                           //Lock Test if unlock
                value.LockedBy = userId;
                lock(value);
            }
        }
        function lock(value) {
            apiService.post(baseUrl + '/api/question/lockQuestion/', value,
              lockSucceded,
              lockFailed);
        }

        function lockSucceded(response) {
            console.log(response);
            $scope.search($scope.page);
            notificationService.displayInfo('Locked successfully');
        }

        function lockFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }



        function unLock(value) {
            apiService.post(baseUrl + '/api/question/unLockQuestion/', value,
              unLockSucceded,
              unLockFailed);
        }

        function unLockSucceded(response) {
            console.log(response);
            $scope.search($scope.page);
            notificationService.displayInfo('Unlocked successfully');
        }

        function unLockFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }




        //*******************Multi Lock / Unlock**************

        function showSelected(button) {
            $scope.button = button;
            $scope.newArray = [];
            angular.forEach($scope.Questions, function (question) {

                if (!!question.selected) $scope.newArray.push(question.ID)  //Get all  testID in newArray
            })

            angular.forEach($scope.newArray, function (questionId) {  //Find object of each QuestionID
                //and Call web api 
                var config = {
                    params: {
                        id: questionId
                    }
                };

                apiService.get(baseUrl + '/api/question/question/' + questionId, null,
                  questionCheckCompleted,
                  questionCheckFailed);

            })
        }

        function questionCheckCompleted(result) {
            var data = result.data;
            if ($scope.button == 'lock' && data[0].IsLock == 0)  //if Multi Lock button clicked
            {                                                    // then check if unlock             

                $scope.questionLock(data[0]);
                $scope.search();
            }
            if ($scope.button == 'unLock' && data[0].IsLock == 1) //if Multi Unlock button clicked
            {                                                     //then check if locked               
                $scope.questionLock(data[0]);
                $scope.search();
            }
            $scope.search();
        }

        function questionCheckFailed(response) {
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
                        pageSize: 10,
                        code: item.Code,
                        text: item.Text,
                        type: item.Type,
                        topicid: item.TopicId,
                        standardid: item.StandardId,
                        subjectid: item.SubjectId,
                        status: item.Status,
                        difficultyLevel: item.DifficultyLevel
                    }
                };


                apiService.get(baseUrl + '/api/question/advancedsearch/', config,
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
})(angular.module('app-administration')); 1