(function (app) {
    'use strict';

    app.factory('constantStrService', constantStrService);

    constantStrService.$inject = ['$http', '$rootScope', '$cookieStore'];

    function constantStrService($http, $rootScope, $cookieStore) {

        var webapipath = 'http://localhost:51642';

        var service = {
            STANDARD: STANDARD,
            ADD_STANDARD: ADD_STANDARD,
            UPDATE_STANDARD: UPDATE_STANDARD,
            DELETE_STANDARD:DELETE_STANDARD, 

            SUBJECT: SUBJECT,
            ADD_SUBJECT: ADD_SUBJECT,
            UPDATE_SUBJECT: UPDATE_SUBJECT,
            DELETE_SUBJECT: DELETE_SUBJECT,

            STD_SUB_MAP: STD_SUB_MAP,
            ADD_STD_SUB_MAP: ADD_STD_SUB_MAP,
            ADD_TO_TOPIC_STD_SUB_MAP: ADD_TO_TOPIC_STD_SUB_MAP,
            DELETE_STD_SUB_MAP: DELETE_STD_SUB_MAP,

            TOPIC: TOPIC,
            ADD_TOPIC: ADD_TOPIC,
            UPDATE_TOPIC: UPDATE_TOPIC,
            DELETE_TOPIC: DELETE_TOPIC,
            ADD_TO_QUESTION_TOPIC: ADD_TO_QUESTION_TOPIC,

            QUESTION: QUESTION,
            ADD_QUESTION: ADD_QUESTION,
            UPDATE_QUESTION: UPDATE_QUESTION,
            DELETE_QUESTION: DELETE_QUESTION,
            LOCK_QUESTION: LOCK_QUESTION,

            TEST: TEST,
            ADD_TEST: ADD_TEST,
            UPDATE_TEST: UPDATE_TEST,
            DELETE_TEST: DELETE_TEST,
            LOCK_TEST: LOCK_TEST,
            POOLS_TEST: POOLS_TEST,
            GENERATE_PAPER_TEST: GENERATE_PAPER_TEST,


            POOL: POOL,
            ADD_POOL: ADD_POOL,
            UPDATE_POOL: UPDATE_POOL,
            DELETE_POOL: DELETE_POOL,
            LOCK_POOL: LOCK_POOL,
            ADD_QUESTION_POOL: ADD_QUESTION_POOL,
            VIEW_QUESTION_POOL: VIEW_QUESTION_POOL,

            POOL_QUESTION_MAP: POOL_QUESTION_MAP,
            ADD_POOL_QUESTION_MAP: ADD_POOL_QUESTION_MAP,
            DELETE_POOL_QUESTION_MAP: DELETE_POOL_QUESTION_MAP
        };

///////////////////////////////////////////////////////
        function STANDARD() {
            return "VIEWSTANDARD";
        }
        function ADD_STANDARD() {
            return "ADDSTANDARD";
        }
        function UPDATE_STANDARD() {
            return "UPDATESTANDARD";
        }
        function DELETE_STANDARD() {
            return "DELETESTANDARD";
        }
///////////////////////////////////////////////////////

        function SUBJECT() {
            return "VIEWSUBJECT";
        }

        function ADD_SUBJECT() {
            return "ADDSUBJECT";
        }
        function UPDATE_SUBJECT() {
            return "UPDATESUBJECT";
        }
        function DELETE_SUBJECT() {
            return "DELETESUBJECT";
        }


///////////////////////////////////////////////////////
        function STD_SUB_MAP() {
            return "VIEWSTDSUBMAP";
        }
        function ADD_STD_SUB_MAP() {
            return "ADDSTDSUBMAP";
        }
        function ADD_TO_TOPIC_STD_SUB_MAP() {
            return "ADDTOTOPICTDSUBMAP";
        }
        function DELETE_STD_SUB_MAP() {
            return "DELETESTDSUBMAP";
        }

///////////////////////////////////////////////////////
        function TOPIC() {
            return "VIEWTOPIC";
        }
        function ADD_TOPIC() {
            return "ADDTOPIC";
        }
        function UPDATE_TOPIC() {
            return "UPDATETOPIC";
        }
        function DELETE_TOPIC() {
            return "DELETETOPIC";
        }
        function ADD_TO_QUESTION_TOPIC() {
            return "ADDTOQUESTIONTOPIC";
        }


///////////////////////////////////////////////////////
        function QUESTION() {
            return "VIEWQUESTION";
        }
        function ADD_QUESTION() {
            return "ADDQUESTION";
        }
        function UPDATE_QUESTION() {
            return "UPDATEQUESTION";
        }
        function DELETE_QUESTION() {
            return "DELETEQUESTION";
        }
        function LOCK_QUESTION() {
            return "LOCKQUESTION";
        }


///////////////////////////////////////////////////////
        function TEST() {
            return "VIEWTEST";
        }
        function ADD_TEST() {
            return "ADDTEST";
        }
        function UPDATE_TEST() {
            return "UPDATETEST";
        }
        function DELETE_TEST() {
            return "DELETETEST";
        }
        function LOCK_TEST() {
            return "LOCKTEST";
        }
        function POOLS_TEST() {
            return "POOLSTEST";
        }
        function GENERATE_PAPER_TEST() {
            return "GENERATEPAPERTEST";
        }


///////////////////////////////////////////////////////
        function POOL() {
            return "VIEWPOOL";
        }
        function ADD_POOL() {
            return "ADDPOOL";
        }
        function UPDATE_POOL() {
            return "UPDATEPOOL";
        }
        function DELETE_POOL() {
            return "DELETEPOOL";
        }
        function LOCK_POOL() {
            return "LOCKPOOL";
        }
        function ADD_QUESTION_POOL() {
            return "ADDQUESTIONPOOL";
        }
        function VIEW_QUESTION_POOL() {
            return "VIEWQUESTIONPOOL";
        }

///////////////////////////////////////////////////////

        function POOL_QUESTION_MAP() {
            return "VIEWPOOLQUESTIONMAP";
        }
        function ADD_POOL_QUESTION_MAP() {
            return "ADDPOOLQUESTIONMAP";
        }
        function DELETE_POOL_QUESTION_MAP() {
            return "DELETEPOOLQUESTIONMAP";
        }

        return service;
    }
})(angular.module('common.core'));