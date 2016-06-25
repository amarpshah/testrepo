(function (app) {
    'use strict';

    app.controller('compRegistrationCtrl', compRegistrationCtrl);

    compRegistrationCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService'];

    function compRegistrationCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-companyregistration';
        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');


    }
})(angular.module('app-sysconfig'));