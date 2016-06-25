(function () {
    'use strict';

    angular.module('app-sysconfig', ['common.core', 'common.ui', 'agGrid', 'ui.router'])
        .config(config)
        .run(run);
    

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('Config', {
                url: '/system',
                templateUrl: '../system/configuration.html',
                resolve: { isAuthenticated: isAuthenticated }
            })
        .state('ConfigUserCreation', {
            url: '/system/user',
            templateUrl: '../system/html/usercreation.html',
            controller: "userCreationCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('ConfigRoleCreation', {
            url: '/system/role',
            templateUrl: '../system/html/rolecreation.html',
            controller: 'roleCreationCtrl',
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('ConfigPermission', {
            url: '/system/permission',
            templateUrl: '../system/html/permission.html',
            controller: "permissionCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('ConfigCompanyRegistration', {
            url: '/system/company',
            templateUrl: '../system/html/companyregistration.html',
            controller: "compRegistrationCtrl",
            resolve: { isAuthenticated: isAuthenticated }
        });
    }

    isAuthenticated.$inject = ['membershipService','$rootScope', '$location', '$modal', '$window'];

    function isAuthenticated(membershipService, $rootScope, $location, $modal, $window) {
        

        if (!membershipService.isUserLoggedIn()) {
            $rootScope.previousState = $location.path();
            //$location.path('/login');

            location.href = 'http://localhost:56387/account/html/login.html';
          
        }
    }

    run.$inject = ['$rootScope', '$location', '$cookieStore', '$http', 'membershipService'];

    function run($rootScope, $location, $cookieStore, $http, membershipService) {
        // handle page refreshes
   
        $rootScope.repository = $cookieStore.get('repository') || {};
        membershipService.redirectIfNotLoggedIn();
        if ($rootScope.repository.hasOwnProperty('loggedUser')) {
            if ($rootScope.repository.loggedUser) {
                $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
            }
        }
    }
})();