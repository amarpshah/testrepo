(function (app) {
    'use strict';

    app.controller('poolCtrl', poolCtrl);

    poolCtrl.$inject = ['$scope', '$modal', 'apiService', 'webApiLocationService', 'notificationService'];

    function poolCtrl($scope, $modal, apiService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-pool';
        var baseUrl = webApiLocationService.get('webapi');


    }
})(angular.module('app-exam'));