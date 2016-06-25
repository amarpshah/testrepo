(function (app) {
    'use strict';

    app.controller('testCtrl', testCtrl);

    testCtrl.$inject = ['$scope', '$modal', 'apiService', 'webApiLocationService', 'notificationService'];

    function testCtrl($scope, $modal, apiService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-test';
        var baseUrl = webApiLocationService.get('webapi');


    }
})(angular.module('app-exam'));