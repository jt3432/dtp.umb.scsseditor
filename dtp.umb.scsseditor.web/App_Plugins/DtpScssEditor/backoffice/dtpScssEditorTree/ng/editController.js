angular.module('umbraco')
.controller('DtpScssEditor.ScssEditorEditController',
    function ($scope, $routeParams, $timeout, $log, $q, $location, dtpScssEditorResource, dtpScssEditorFile, formHelper, contentEditingHelper, notificationsService, navigationService, dialogService, entityResource, treeService) {

        $scope.tabs = [{ id: 1, label: 'SCSS' }, { id: 2, label: 'Properties' }];

        dtpScssEditorResource.getVariables().then(function (response) {
            if (response.data) {
                $scope.variables = response.data;
            }
        });

        dtpScssEditorResource.getFile($routeParams.id).then(function (response) {
            //$log.debug('getFile response: ', response);
            dtpScssEditorFile.reset();
            dtpScssEditorFile.path = response.data.pathRelative;
            dtpScssEditorFile.absolutePath = response.data.pathFull;
            dtpScssEditorFile.name = response.data.name;
            dtpScssEditorFile.scss = response.data.content;
            $scope.scssFile = dtpScssEditorFile;

            navigationService.syncTree({ tree: 'dtpScssEditorTree', path: dtpScssEditorResource.buildTreePath(dtpScssEditorFile.path), forceReload: false }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });

            $scope.loaded = true;
        });

        $scope.save = function () {

            var deferred = $q.defer();

            if (formHelper.submitForm({ scope: $scope, statusMessage: 'Saving...' })) {

                var scssFile = {
                    content: dtpScssEditorFile.scss,
                    pathRelative: dtpScssEditorFile.path,
                    pathFull: dtpScssEditorFile.absolutePath
                };

                dtpScssEditorResource.saveFile(scssFile).then(function (response) {

                    if (response.data) {

                        formHelper.resetForm({ scope: $scope, notifications: '' });

                        contentEditingHelper.handleSuccessfulSave({
                            scope: $scope,
                            savedContent: response.data,
                            rebindCallback: function () {
                            },
                            redirectId: response.data.guid
                        });

                        if (scssFile.pathFull.indexOf(scssFile.pathRelative) === -1) {

                            $location.search('');
                            $location.path('/settings/dtpScssEditorTree/edit/' + scssFile.pathRelative);
                            $location.replace();

                            navigationService.syncTree({ tree: 'dtpScssEditorTree', path: [-1, -1], forceReload: false });
                            //navigationService.syncTree({ tree: 'dtpScssEditorTree', path: dtpScssEditorResource.buildTreePath(scssFile.pathRelative), forceReload: true }).then(function (syncArgs) {
                            //    $scope.currentNode = syncArgs.node;
                            //});                            
                        }

                        deferred.resolve(response.data);

                        notificationsService.success('Success', 'SCSS file ' + dtpScssEditorFile.name + ' has been saved');

                    } else {

                        notificationsService.error('Failed', 'SCSS file ' + dtpScssEditorFile.name + ' was NOT saved');

                    }

                }, function (err) {

                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: false,
                        err: err
                    });

                    deferred.reject(err);

                });

            }

            return deferred.promise;

        };

        $scope.openDialog = function (dialogType) {

            var dialog = dialogService.open({
                template: '/App_Plugins/DtpScssEditor/backoffice/views/dialog' + dialogType + '.html', show: true, callback: function () {
                    $log.info('Dialog' + dialogType + ' opened!');
                }
            });

        };

        $scope.closeDialog = function ($event) {
            $event.preventDefault();
            dialogService.closeAll();
            return false;
        }

        $scope.insertVariable = function () {
            if ($scope.selectedVariable !== undefined && $scope.selectedVariable !== '') {
                $scope.scssFile.editor.replaceSelection($scope.selectedVariable);
                dialogService.closeAll();
            }
        };

    });