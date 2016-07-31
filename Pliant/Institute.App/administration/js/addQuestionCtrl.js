(function (app) {
    'use strict';

    app.controller('addQuestionCtrl', addQuestionCtrl);

    addQuestionCtrl.$inject = ['$scope', '$modal', '$route', '$routeParams', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams'];

    function addQuestionCtrl($scope, $modal, $route, $routeParams, apiService, membershipService, webApiLocationService, notificationService, $stateParams) {
        $scope.question = {};
        $scope.pagesCount = 0;
        $scope.loadingQuestion = true;
        $scope.Standards = [];
        $scope.Subjects = [];
        $scope.question.questionstandard = [];
        $scope.question.questionsubject = [];
        $scope.Questions = [];
        $scope.EditedQuestion = {};
        $scope.newQuestion = {};
        $scope.IsEditMode = false;
        $scope.ShowAddQuestion = ShowAddQuestion;
        $scope.AddQuestion = AddQuestion;
        $scope.editQuestion = editQuestion;
        $scope.find = find;
        $scope.UpdateQuestion = UpdateQuestion;
        $scope.loadSubject = loadSubject;
        $scope.loadStandard = loadStandard;
        $scope.loadTopic = loadTopic;
        $scope.StandardChange = StandardChange;
        $scope.SubjectChange = SubjectChange;
        $scope.TopicChange = TopicChange;
        $scope.RemoveMatch = RemoveMatch;
        $scope.AddMatch = AddMatch;
        $scope.RemoveChoice = RemoveChoice;
        $scope.AddChoice = AddChoice;
        $scope.RadioChange = RadioChange;
        $scope.RemoveMChoice = RemoveMChoice;
        $scope.AddMChoice = AddMChoice;
        $scope.ValidateQuestion = ValidateQuestion;


        membershipService.redirectIfNotLoggedIn();
        $scope.QuestionId = parseInt($stateParams.questionid);
        $scope.IsLock = parseInt($stateParams.islock);

        $scope.newQuestion.StandardId = parseInt($stateParams.stdid);
        $scope.newQuestion.SubjectId = parseInt($stateParams.subid);
        $scope.newQuestion.TopicId = parseInt($stateParams.topicid);

        $scope.newQuestion.Matches = [];
        var baseUrl = webApiLocationService.get('webapi');


        function editQuestion() {
            if ($scope.QuestionId != null && !isNaN($scope.QuestionId)) {
                $scope.IsEditMode = true;
                $scope.find($scope.QuestionId);
            }
        }

        $scope.editQuestion();

        function find(questionid) {
            var config = {
                params: {
                    id: questionid
                }
            };

            apiService.get(baseUrl + '/api/question/question/' + questionid, null,
                questionLoadCompleted,
                questionLoadFailed);
        }

        function questionLoadCompleted(result) {
            var question = result.data[0];

            $('.questionForm .proceed').addClass('update');
            $('.questionForm .proceed span').toggleClass('active');
            $(this).closest('.questionItem').addClass('updating')
            $('.questionControl').removeClass('active');
            $('.questionForm').addClass('active');

            if ($scope.Questions != null) {
                $scope.newQuestion.QuestionID = question.ID;
                $scope.newQuestion.StandardId = question.StandardID;
                $scope.newQuestion.SubjectId = question.SubjectID;
                $scope.newQuestion.MappingID = question.MappingID;
                $scope.newQuestion.TopicId = question.TopicID;
                $scope.newQuestion.Code = question.Code;
                $scope.newQuestion.Question = question.Text;
                $scope.newQuestion.Objective = question.Objective;
                $scope.newQuestion.Hint = question.Hint;
                $scope.newQuestion.QuestionType = question.Type;
                $scope.newQuestion.Points = question.Points;
                $scope.newQuestion.Status = question.Status;
                $scope.newQuestion.Difficulty = question.DifficultyLevel;

                var questiontype = question.sType;

                $('.questionForm .questionType').removeClass('active');
                $('.questionForm .questionType[data-type="' + (question.Type - 1) + '"]').addClass('active');

                if (questiontype == "Please select a question type") {
                    notificationService.displayError("Need to select all the necessary fields.");
                }
                else if (questiontype == "Descriptive") {
                    $scope.newQuestion.AnsKeywords = question.Descriptive[0].Keywords;

                }
                else if (questiontype == "True or False") {
                    $('.displayGroup').addClass('active');
                    $scope.newQuestion.TFAnswer = question.Choices[0].Text;
                    $scope.newQuestion.QuestionDisplayType = question.Choices[0].DisplayType;
                    $('.answerText').val($(this).find('option:selected').text());
                }
                else if (questiontype == "Match the following") {
                    var matches = [];
                    var i = 0;

                    $scope.newQuestion.Matches = [];

                    for (i = 0; i < question.Matches.length; i++) {
                        $scope.newQuestion.Matches.push(
                               {
                                   ID: i.toString(),
                                   ChoiceId: i,
                                   choiceA: question.Matches[i].ChoiceA,
                                   choiceB: question.Matches[i].ChoiceB
                               });
                    }
                }
                else if (questiontype == "Single choice" || questiontype == "Multiple choice") {
                    if (questiontype == "Single choice") {
                        var i = 0;
                        for (i = 0; i < question.Choices.length; i++) {
                            question.Choices[i].ID = question.Choices[i].ChoiceId;
                            if (question.Choices[i].IsAnswer == true) {
                                $scope.newQuestion.cIsAnswer = question.Choices[i].ChoiceId;
                            }
                        }
                        $scope.newQuestion.Choices = question.Choices;
                    }
                    else {
                        $scope.newQuestion.MChoices = question.Choices;
                        $scope.newQuestion.MChoices.PointsPerChoice = question.Choices[0].PointsPerChoice;
                    }
                }
            }
        }

        function questionLoadFailed(response) {
            notificationService.displayError(response.data);
        }



        function AddMChoice() {
            var lastItem = $scope.newQuestion.MChoices[$scope.newQuestion.MChoices.length - 1]
            if (lastItem != null) {
                var m = {
                    ID
                        : (lastItem.ChoiceId + 1).toString(), ChoiceId: (lastItem.ChoiceId + 1), IsAnswer: 0, Text: ''
                };
                $scope.newQuestion.MChoices.push(m);
            }
        }

        function AddChoice() {
            var lastItem = $scope.newQuestion.Choices[$scope.newQuestion.Choices.length - 1]
            if (lastItem != null) {
                var m = {
                    ID
                        : (lastItem.ChoiceId + 1).toString(), ChoiceId: (lastItem.ChoiceId + 1), IsAnswer: false, Text: ''
                };
                $scope.newQuestion.Choices.push(m);
            }
        }

        function RadioChange(ID) {
            var i = 0;
            for (i = 0; i < $scope.newQuestion.Choices.length; i++) {
                if (ID == $scope.newQuestion.Choices[i].ID) {
                    $scope.newQuestion.Choices[i].IsAnswer = true;
                }
                else {
                    $scope.newQuestion.Choices[i].IsAnswer = false;
                }
            }

        }

        function RemoveChoice(employee, id) {
            var index = $scope.newQuestion.Choices.indexOf(employee);
            $scope.newQuestion.Choices.splice(index, 1);
        }

        function RemoveMChoice(employee, id) {
            var index = $scope.newQuestion.MChoices.indexOf(employee);
            $scope.newQuestion.MChoices.splice(index, 1);
        }

        function AddMatch() {
            var lastItem = $scope.newQuestion.Matches[$scope.newQuestion.Matches.length - 1]
            if (lastItem != null) {
                var m = {
                    ID
                        : (lastItem.ChoiceId + 1).toString(), ChoiceId: (lastItem.ChoiceId + 1), choiceA: '', choiceB: ''
                };
                $scope.newQuestion.Matches.push(m);
            }
        }

        function RemoveMatch(index) {
            var removingIndex = -1;
            for (var i = 0; i < $scope.newQuestion.Matches.length; i++) {
                if ($scope.newQuestion.Matches[i].ChoiceId == index) {
                    removingIndex = i;
                    break;
                }
            }
            if (removingIndex != -1) {
                $scope.newQuestion.Matches.splice(removingIndex, 1);
            }
        }


        $scope.loadvalues = function () {
            $scope.newQuestion.Matches = [];
            $scope.newQuestion.Choices = [];
            $scope.newQuestion.MChoices = [];

            $scope.newQuestion.Matches = [{ ID: '1', ChoiceId: 1, choiceA: '', choiceB: '' }, { ID: '2', ChoiceId: 2, choiceA: '', choiceB: '' }];

            $scope.newQuestion.Choices = [{ ID: '1', ChoiceId: 1, IsAnswer: false, Text: '' }, { ID: '2', ChoiceId: 2, IsAnswer: false, Text: '' }];

            $scope.newQuestion.MChoices = [{ ID: '1', ChoiceId: 1, IsAnswer: 0, Text: '' }, { ID: '2', ChoiceId: 2, IsAnswer: 0, Text: '' }];

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

            $scope.question.questionDisplayType = [
                                                  { value: 1, Text: "True/False", name: "trueOrFalse", val1: "true", val2: "false" },
                                                  { value: 2, Text: "Yes/No", name: "yesOrNo", val1: "yes", val2: "no" }
            ];

        };
        $scope.ShowAddQuestion();
        $scope.loadvalues();
        $scope.loadStandard();
        $scope.loadSubject();
        $scope.loadTopic();

        StandardChange(false);
        SubjectChange(false);

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

        ///START*************** Event Handlers ****************************

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

        function TopicChange() {

            $scope.newQuestion.TopicName = $("#selectTopic option:selected").text();

            var config = {
                params: {
                    topicid: $scope.newQuestion.TopicId
                }
            };
            apiService.get(baseUrl + '/api/question/filterquestions', config,
                topicChangeLoadCompleted,
                topicChangeLoadFailed);
        }

        function topicChangeLoadCompleted(result) {

        }

        function topicChangeLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        ///END*************** Event Handlers ****************************

        function ShowAddQuestion() {
            var flag = false;
            if ($('.questionControl select').val() == "0") {
                alert("Please select question type!");
            } else {
                flag = true;
            }
            if (flag) {

                $scope.IsNewQuestion = true;

                $('#curtain').addClass('active');
                $('.easy, .medium, .difficult').removeClass('active');
                $('.questionForm, .questionControl').toggleClass('active');
                setTimeout(function () {
                    $('#curtain').addClass('overlay');
                    $('.questionForm').addClass('active');
                    //$('.questionForm .questionType').removeClass('active');
                    $('.questionForm .questionType[data-type="' + $('.questionControl select').val() + '"]').addClass('active');
                    // $('.questionForm .type').val($('.questionControl select').val());
                    $('#curtain').removeClass('overlay active');
                }, 1000);
            };

        }


        function UpdateQuestion() {
            var newQuestion = {};
            newQuestion = FillUpControlData();
            newQuestion.ID = $scope.newQuestion.QuestionID;

            apiService.post(baseUrl + '/api/question/update', newQuestion,
                updateQuestionSucceded,
                updateQuestionFailed);

        }

        function updateQuestionSucceded(response) {
            console.log(response);
            $scope.newQuestion = response.data;
            notificationService.displayInfo('Data updated successfully');
            $scope.loadvalues();

        }

        function updateQuestionFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        function FillUpControlData() {
            var newQuestion = {};

            $scope.IsNewQuestion = true;
            newQuestion.TopicId = $scope.newQuestion.TopicId;
            newQuestion.Code = $scope.newQuestion.Code;
            newQuestion.Text = $scope.newQuestion.Question;
            newQuestion.Objective = $scope.newQuestion.Objective;
            newQuestion.Hint = $scope.newQuestion.Hint;
            newQuestion.Type = $scope.newQuestion.QuestionType;
            newQuestion.Points = $scope.newQuestion.Points;
            newQuestion.Status = $scope.newQuestion.Status;
            newQuestion.DifficultyLevel = $scope.newQuestion.Difficulty;
            newQuestion.Descriptive = [];
            newQuestion.MatchPairs = [];
            newQuestion.Choices = [];

            var questiontype = $("#qqtype option:selected").text();

            if (questiontype == "Please select a question type") {
                notificationService.displayError("Need to select all the necessary fields.");
            }
            else if (questiontype == "Descriptive") {
                var d = [];
                var desc = {};
                desc.Keywords = $scope.newQuestion.AnsKeywords;
                d.push(desc);
                newQuestion.Descriptive = d;
            }
            else if (questiontype == "True or False") {
                var options1 = [];

                options1.push({
                    ChoiceId: 0,
                    IsAnswer: true,
                    Text: $scope.newQuestion.TFAnswer,
                    DisplayType: $scope.newQuestion.QuestionDisplayType.value
                });
                newQuestion.Choices = options1;
            }
            else if (questiontype == "Match the following") {
                var matches = [];

                var i = 0;
                for (i = 0; i < $scope.newQuestion.Matches.length; i++) {
                    var itm = $scope.newQuestion.Matches[i];
                    matches.push({
                        ChoiceId: i,
                        ChoiceA: itm.choiceA,
                        ChoiceB: itm.choiceB
                    });
                }

                newQuestion.Matches = $scope.newQuestion.Matches;

            }
            else if (questiontype == "Single choice" || questiontype == "Multiple choice") {


                if (questiontype == "Single choice") {
                    var i = 0;
                    for (i = 0; i < $scope.newQuestion.Choices.length; i++) {
                        $scope.newQuestion.Choices[i].ChoiceId = i;
                        if ($scope.newQuestion.cIsAnswer == $scope.newQuestion.Choices[i].ID) {
                            $scope.newQuestion.Choices[i].IsAnswer = true;
                        }
                        else {
                            $scope.newQuestion.Choices[i].IsAnswer = false;
                        }
                    }
                    newQuestion.Choices = $scope.newQuestion.Choices;
                }
                else {
                    for (i = 0; i < $scope.newQuestion.MChoices.length; i++) {
                        $scope.newQuestion.MChoices[i].ChoiceId = i;
                        $scope.newQuestion.MChoices[i].PointsPerChoice = $scope.newQuestion.MChoices.PointsPerChoice;
                    }
                    newQuestion.Choices = $scope.newQuestion.MChoices;
                }

            }
            else if (questiontype == "Fill in the blanks") {

            }
            return newQuestion;
        }

        /////// add question ///////////////////////////////////////
        function AddQuestion() {
            if ($scope.ValidateQuestion()) {
                var newQuestion = {};

                newQuestion = FillUpControlData();

                $('.fillInTheBlankSelect').removeClass('active');
                $('.displayGroup').removeClass('active');
                $('.difficulty').siblings().removeClass('active');


                apiService.post(baseUrl + '/api/question/add', newQuestion,
                    registerQuestionSucceded,
                    registerQuestionFailed);
            }
        }

        function registerQuestionSucceded(response) {
            console.log(response);
            $scope.newQuestion = response.data;
            notificationService.displayInfo('Data uploaded successfully');
            $scope.loadvalues();
        }

        function registerQuestionFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }



        //Validation Question

        function ValidateQuestion() {
            if (isNaN($scope.newQuestion.StandardId)) {
                $scope.vStandardId = true;
            }
            if (isNaN($scope.newQuestion.SubjectId)) {
                $scope.vSubjectId = true;
            }
            if (isNaN($scope.newQuestion.TopicId)) {
                $scope.vTopicId = true;
            }
            if (angular.isUndefined($scope.newQuestion.Code)) {
                $scope.vCode = true;
            }
            if (angular.isUndefined($scope.newQuestion.Question)) {
                $scope.vQuestion = true;
            }
            if (angular.isUndefined($scope.newQuestion.Objective)) {
                $scope.vObjective = true;
            }
            if (angular.isUndefined($scope.newQuestion.Hint)) {
                $scope.vHint = true;
            }
            if (isNaN($scope.newQuestion.QuestionType)) {
                $scope.vQuestionType = true;
            }
            if (isNaN($scope.newQuestion.Points)) {
                $scope.vPoints = true;
            }
            if (isNaN($scope.newQuestion.Status)) {
                $scope.vStatus = true;
            }
            if (isNaN($scope.newQuestion.Difficulty)) {
                $scope.vDifficulty = true;
                return false;
            }

            else {
                return true;
            }

        }
    }

})(angular.module('app-administration'));