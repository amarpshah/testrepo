var mainApp = angular.module('app-main', ['ui.router','common.core', 'app-administration']);

mainApp.config(function($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state('BackHome',{
            url: '/',
            templateUrl: '/mainDashboard/MainDashboard.html',
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('Administration', {
            url: '/Administration',
            templateUrl: '../administration/Administration.html',
            resolve: { isAuthenticated: isAuthenticated }
        })
        .state('Library', {
            url: '/Library',
            templateUrl: '../library/Library.html',
            resolve: { isAuthenticated: isAuthenticated }
        });
        
});

mainApp.run(function ($rootScope, $location, $cookieStore, $http) {
    // handle page refreshes

    $rootScope.repository = $cookieStore.get('repository') || {};

    if ($rootScope.repository.loggedUser) {
        $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
    }

    
    ////$(document).ready(function () {
    ////    $(".fancybox").fancybox({
    ////        openEffect: 'none',
    ////        closeEffect: 'none'
    ////    });

    ////    $('.fancybox-media').fancybox({
    ////        openEffect: 'none',
    ////        closeEffect: 'none',
    ////        helpers: {
    ////            media: {}
    ////        }
    ////    });

    ////    $('[data-toggle=offcanvas]').click(function () {
    ////        $('.row-offcanvas').toggleClass('active');
    ////    });
    ////});
});


isAuthenticated.$inject = ['membershipService', '$rootScope', '$location', '$modal'];

function isAuthenticated(membershipService, $rootScope, $location, $modal) {
    if (!membershipService.isUserLoggedIn()) {
        $rootScope.previousState = $location.path();
        //$location.path('/login');

        $modal.open({
            keyboard: false,
            backdrop: 'static',
            templateUrl: "../account/html/login.html",
            controller: "loginCtrl"
        }).result.then(function () {
        }, function () {
        });
    }
}