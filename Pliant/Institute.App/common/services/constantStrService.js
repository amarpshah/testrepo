(function (app) {
    'use strict';

    app.factory('constantStrService', constantStrService);

    constantStrService.$inject = ['$http', '$rootScope', '$cookieStore'];

    function constantStrService($http, $rootScope, $cookieStore) {

        var webapipath = 'http://localhost:51642';

        var service = {
            STANDARD: STANDARD,
            ADD_STANDARD: ADD_STANDARD,
            EDIT_STANDARD: EDIT_STANDARD,
            DELETE_STANDARD:DELETE_STANDARD, 

            SUBJECT: SUBJECT,
            ADD_SUBJECT: ADD_SUBJECT,
            EDIT_SUBJECT: EDIT_SUBJECT,
            DELETE_SUBJECT: DELETE_SUBJECT,

            STD_SUB_MAP: STD_SUB_MAP,

            TOPIC: TOPIC,
            ADD_TOPIC: ADD_TOPIC,
            EDIT_TOPIC: EDIT_TOPIC,
            DELETE_TOPIC: DELETE_TOPIC,

            QUESTION: QUESTION,
            ADD_QUESTION: ADD_QUESTION,
            EDIT_QUESTION: EDIT_QUESTION,
            DELETE_QUESTION: DELETE_QUESTION,

            TEST: TEST,
            ADD_TEST: ADD_TEST,
            EDIT_TEST: EDIT_TEST,
            DELETE_TEST: DELETE_TEST,

            POOL: POOL,
            ADD_POOL: ADD_POOL,
            EDIT_POOL: EDIT_POOL,
            DELETE_POOL: DELETE_POOL,

            POOL_QUESTION_MAP: POOL_QUESTION_MAP
        };

///////////////////////////////////////////////////////
        function STANDARD() {
            return "STD";
        }
        function ADD_STANDARD() {
            return "ADDSTD";
        }
        function EDIT_STANDARD() {
            return "EDITSTD";
        }
        function DELETE_STANDARD() {
            return "DELETESTD";
        }
///////////////////////////////////////////////////////

        function SUBJECT() {
            return "SUBJECT";
        }

        function ADD_SUBJECT() {
            return "ADDSUBJECT";
        }
        function EDIT_SUBJECT() {
            return "EDITSUBJECT";
        }
        function DELETE_SUBJECT() {
            return "DELETESUBJECT";
        }


///////////////////////////////////////////////////////
        function STD_SUB_MAP() {
            return "STDSUBMAP";
        }

///////////////////////////////////////////////////////
        function TOPIC() {
            return "TOPIC";
        }
        function ADD_TOPIC() {
            return "ADDTOPIC";
        }
        function EDIT_TOPIC() {
            return "EDITTOPIC";
        }
        function DELETE_TOPIC() {
            return "DELETETOPIC";
        }


///////////////////////////////////////////////////////
        function QUESTION() {
            return "QUESTION";
        }
        function ADD_QUESTION() {
            return "ADDQUESTION";
        }
        function EDIT_QUESTION() {
            return "EDITQUESTION";
        }
        function DELETE_QUESTION() {
            return "DELETEQUESTION";
        }


///////////////////////////////////////////////////////
        function TEST() {
            return "TEST";
        }
        function ADD_TEST() {
            return "ADDTEST";
        }
        function EDIT_TEST() {
            return "EDIT_TEST";
        }
        function DELETE_TEST() {
            return "DELETE_TEST";
        }


///////////////////////////////////////////////////////
        function POOL() {
            return "POOL";
        }
        function ADD_POOL() {
            return "ADDPOOL";
        }
        function EDIT_POOL() {
            return "EDITPOOL";
        }
        function DELETE_POOL() {
            return "DELETEPOOL";
        }

///////////////////////////////////////////////////////

        function POOL_QUESTION_MAP() {
            return "POOLQUEMAP";
        }

        return service;
    }
})(angular.module('common.core'));