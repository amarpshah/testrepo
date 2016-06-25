(function () {
    'use strict';

    angular.module('app-exam', ['common.core', 'common.ui', 'agGrid', 'ui.router'])
        .config(config)
        .run(run);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('Exam', {
                url: '/examination',
                templateUrl: '../examination/exam.html',
                resolve: { isAuthenticated: isAuthenticated }
            })
        .state('ExamPoolCreation', {
            url: '/examination/pool',
            templateUrl: '../examination/html/pool.html',
            controller: "poolCtrl"
        })
        .state('ExamTestCreation', {
            url: '/examination/test',
            templateUrl: '../examination/html/test.html',
            controller: "testCtrl"
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