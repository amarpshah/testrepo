(function () {
    'use strict';

    angular.module('app-administration', ['common.core', 'common.ui', 'agGrid', 'ui.router'])
        .config(config)
        .run(run);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('Admin', {
                url: '/Administration',
                templateUrl: '../administration/Administration.html',
                resolve: { isAuthenticated: isAuthenticated }
            })
        .state('AdminConfig', {
            url: '/Administration/Config',
            templateUrl: '../administration/html/configuration.html',
            controller: "configurationCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminStandard', {
            url: '/Standard',
            templateUrl: '../administration/html/standard.html',
            controller: 'standardCtrl',
            resolve: { isAuthenticated: isAuthenticated }
        })
            
        .state('AdminSubject', {
            url: '/Subject',
            templateUrl: '../administration/html/subject.html',
            controller: "subjectCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
           
        .state('AdminMapping', {
            url: '/Administration/Mapping',
            templateUrl: '../administration/html/SSMapping.html',
            controller: "ssMappingCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
              
                 .state('AdminTopic', {
                 url: '/Administration/Topic',
                 templateUrl: '../administration/html/topic.html',
                 controller: "topicCtrl",
                 resolve: { isAuthenticated: isAuthenticated }
             })

       
        .state('AdminTopicNav', {
            url: '/Administration/Topic/:stdid/:subid/:mapid',
            templateUrl: '../administration/html/topic.html',
            controller: "topicCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminQuestion', {
            url: '/Administration/Questions',
            templateUrl: '../administration/html/question.html',
            controller: "questionCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
            .state('AdminEditQuestion', {
                url: '/Administration/EditQuestion/:questionid/:islock',
                templateUrl: '../administration/html/addQuestion.html',
                controller: "addQuestionCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })

        .state('AdminQuestionNav', {
            url: '/Administration/Questionstdid/:stdid/:subid/:topicid',
            templateUrl: '../administration/html/addQuestion.html',
            controller: "addQuestionCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })

            .state('AdminAddQuestion', {
                url: '/Administration/AddQuestion',
                templateUrl: '../administration/html/addQuestion.html',
                controller: "addQuestionCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })

        .state('AdminTest', {
            url: '/Administration/Tests',
            templateUrl: '../administration/html/test.html',
            controller: "testCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminAddTest', {
            url: '/Administration/AddTest/',
            templateUrl: '../administration/html/addtest.html',
            controller: "addTestCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
          .state('AdminEditTest', {
              url: '/Administration/EditTest/:testId/:lockId',
              templateUrl: '../administration/html/addtest.html',
              controller: "addTestCtrl",
              resolve: { isAuthenticated: isAuthenticated }
          })
        .state('AdminPool', {
            url: '/Administration/Pool/:testid/:testname',
            templateUrl: '../administration/html/pool.html',
            controller: "poolCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
            .state('AdminEditPool', {
                url: '/Administration/EditPool/:poolid/:testid/:testname/:islock',
                templateUrl: '../administration/html/addPool.html',
                controller: "addPoolCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
        .state('AdminAddPool', {
            url: '/Administration/AddPool/:testid/:testname',
            templateUrl: '../administration/html/addpool.html',
            controller: "addPoolCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminAddPoolQuestion', {
            url: '/Administration/AddPoolQuestion/:poolid/:poolname/:testid/:testname',
            templateUrl: '../administration/html/addquestionpool.html',
            controller: "addQuestionPoolCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminViewPoolQuestion', {
            url: '/Administration/ViewPoolQuestion/:poolid/:poolname/:testid/:testname',
            templateUrl: '../administration/html/viewquestionpool.html',
            controller: "viewQuestionPoolCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminGeneratePaper', {
            url: '/Administration/AdminGeneratePaper/:testid/:testname',
            templateUrl: '../administration/html/generatePaper.html',
            controller: "generatePaperCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('AdminDisplayPaper', {
            url: '/Administration/AdminDisplayPaper/:paperid/:isfinalized',
            templateUrl: '../administration/html/displayPaper.html',
            controller: "displayPaperCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        });
    }

    isAuthenticated.$inject = ['membershipService', '$rootScope', '$location', '$modal', '$window'];

    function isAuthenticated(membershipService, $rootScope, $location, $modal, $window) {
        membershipService.redirectIfNotLoggedIn();


        if (!membershipService.isUserLoggedIn()) {
            $rootScope.previousState = $location.path();
            //$location.path('/login');

            location.href = 'http://localhost:56387/account/html/login.html';
            
        }
    }

    run.$inject = ['$rootScope', '$location', '$cookieStore', '$http'];

    function run($rootScope, $location, $cookieStore, $http) {
        // handle page refreshes
        $rootScope.repository = $cookieStore.get('repository') || {};
        if ($rootScope.repository.hasOwnProperty('loggedUser')) {
            if ($rootScope.repository.loggedUser) {
                $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
            }
        }
    }
})();