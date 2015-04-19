angular.module('umbraco')
.controller('DtpScssEditor.ScssEditorCreateController',
	function ($scope, $routeParams, $q, $location, dtpScssEditorResource, formHelper, notificationsService, navigationService, dialogService, entityResource) {

	    $scope.name = $scope.currentNode.id === '-1' ? '' : $scope.currentNode.id + '/';

	    $scope.create = function (name) {

	        var deferred = $q.defer();

	        if (formHelper.submitForm({ scope: $scope, statusMessage: 'Creating...' })) {

	            name = name.substr(0, name.lastIndexOf('.')) || name;

	            var path = name.replace(/$\.scss/, '').replace(/\//g, '\\');

	            dtpScssEditorResource.create(path).then(function (response) {

	                $scope.ScssEditorItem = response.data;

	                formHelper.resetForm({ scope: $scope, notifications: '' });

	                $location.search('');
	                $location.path('/settings/dtpScssEditorTree/edit/' + path + '.scss');
	                $location.replace();

	                navigationService.syncTree({ tree: 'dtpScssEditorTree', path: dtpScssEditorResource.buildTreePath(path, '.scss'), forceReload: true }).then(function (syncArgs) {
	                    $scope.currentNode = syncArgs.node;
	                });

	                navigationService.hideNavigation();

	                deferred.resolve(response.data);

	                notificationsService.success('Success', 'Scss file ' + name + '.scss has been created');
	            });

	        }

	        return deferred.promise;
	    };

	    $scope.cancelCreate = function () {
	        navigationService.hideNavigation();
	    };
	});