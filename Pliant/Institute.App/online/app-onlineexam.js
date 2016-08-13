(function () {
    'use strict';

    angular.module('app-onlineexam', ['common.core', 'common.ui', 'agGrid', 'ui.router'])
        .config(config)
        .run(run);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('ExamDashboard', {
                url: '/examdashboard',
                templateUrl: '../online/onlineexam.html',
                resolve: { isAuthenticated: isAuthenticated }
            })
        .state('ExamHome', {
            url: '/home',
            templateUrl: '../online/html/quiz.html',
            controller: "quizCtrl"
        })
        .state('ExamCreate', {
            url: '/online/create',
            templateUrl: '../online/html/create.html',
            controller: "createCtrl"
        });
    }

    isAuthenticated.$inject = ['membershipService', '$rootScope', '$location', '$modal', '$window'];

    function isAuthenticated(membershipService, $rootScope, $location, $modal, $window) {
        if (!membershipService.isUserLoggedIn()) {
            $rootScope.previousState = $location.path();
            //$location.path('/login');

            location.href = 'http://localhost:56387/account/html/login.html';
            //location.href = 'http://www.w3schools.com';

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