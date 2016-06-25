(function (app) {
    'use strict';

    app.controller('configurationCtrl', configurationCtrl);

    configurationCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService'];

    function configurationCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-student';
        $scope.openStandardDialog = openStandardDialog;
        $scope.openSubjectDialog = openSubjectDialog;

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');

        function openStandardDialog() {
            //alert('Open Standard');
            $modal.open({
                templateUrl: 'scripts/spas/administration/html/standard.html',
                controller: 'standardCtrl'
            }).result.then(function () {
            }, function () {
            });
        }

        function openSubjectDialog() {
            //alert('Open Subject');
            $modal.open({
                templateUrl: 'scripts/spas/administration/html/subject.html',
                controller: 'subjectCtrl',
                scope: $scope
            }).result.then(function ($scope) {
            }, function () {
            });
        }
    }
})(angular.module('app-administration'));