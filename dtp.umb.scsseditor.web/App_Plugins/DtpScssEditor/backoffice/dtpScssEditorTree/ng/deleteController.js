angular.module('umbraco')
.controller('DtpScssEditor.ScssEditorDeleteController',
	function ($scope, $routeParams, $log, dtpScssEditorResource, notificationsService, navigationService, dialogService, entityResource, treeService) {

	    $scope.delete = function (path) {
	        dtpScssEditorResource.delete(path).then(function (response) {

	            var success = JSON.parse(response.data);

	            $log.debug(success);

	            if (success) {

	                //$scope.currentNode.loading = false;

	                treeService.removeNode($scope.currentNode);

	                //navigationService.syncTree({ tree: 'ScssEditorTree', path: [-1, 'menu', $scope.currentNode.parentId], forceReload: true });
	                navigationService.hideMenu();
	            }

	        });

	    };

	    $scope.cancel = function () {
	        navigationService.hideNavigation();
	    };
	});