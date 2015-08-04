angular.module('umbraco')
.controller('DtpScssEditor.ScssEditorEditController',
    function ($scope, $routeParams, $timeout, $log, $q, $location, dtpScssEditorResource, dtpScssEditorFile, formHelper, contentEditingHelper, notificationsService, navigationService, dialogService, entityResource, treeService) {

        $scope.dialogTreeEventHandler = $({});

        $scope.dialogTreeEventHandler.bind("treeNodeSelect", function (event, args) {
            event.preventDefault();
            event.stopPropagation();

            var _selectedNode = args.node;
            var _treeRootElement = $(args.element).closest(".umb-tree");

            // Clear previously selected items (remove if you want to select multiple)
            // iterate through each child element in the current tree
            _treeRootElement.find("li[node='child']").each(function () {
                $(this).scope().currentNode = null;
            })

            var fileExt = '.scss';
            if (args.node.name.indexOf(fileExt, args.node.name - fileExt.length) !== -1) {
                // this will highlight the current node
                $(args.element).scope().currentNode = _selectedNode;
                $scope.importStatement = '@import \'' + args.node.id.replace(/\\/g, '/') + '\';';
            } else {
                $scope.importStatement = undefined;
            }

        });

        $scope.tabs = [{ id: 1, label: 'SCSS' }, { id: 2, label: 'Properties' }];

        if ($scope.mixins === undefined) {
            dtpScssEditorResource.getMixins().then(function (response) {
                if (response.data) {
                    $scope.mixins = response.data;
                }
            });
        }

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

            $scope.isPartial = dtpScssEditorFile.name.slice(0, 1) == '_';

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

                    $scope.saveSuccessful = response.data.saveSuccess;
                    $scope.compileSuccessful = response.data.compileSuccess;
                    $scope.cssOutput = response.data.cssOutput;

                    if (response.data.saveSuccess && response.data.compileSuccess) {

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
                        }

                        deferred.resolve(response.data);

                        notificationsService.success('Success', 'SCSS file ' + dtpScssEditorFile.name + ' has been saved.');

                    } else {

                        $scope.errorMessage = response.data.message;

                        //if (!response.data.CompileSuccess && !$scope.isPartial) {
                        //    notificationsService.warning('Warning', 'SCSS file ' + dtpScssEditorFile.name + ' has errors and did not compile.');
                        //}

                        if(response.data.saveSuccess) {
                            notificationsService.success('Success', 'SCSS file ' + dtpScssEditorFile.name + ' has been saved.');
                        } else {
                            notificationsService.error('Failed', 'SCSS file ' + dtpScssEditorFile.name + '  was NOT saved.');
                        }                        
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

        $scope.openPreviewDialog = function () {

            dtpScssEditorFile.css = 'Compiling SCSS, please wait...';

            var deferred = $q.defer();

            var scssFile = {
                content: dtpScssEditorFile.scss,
                pathRelative: dtpScssEditorFile.path,
                pathFull: dtpScssEditorFile.absolutePath
            };

            dtpScssEditorResource.getCss(scssFile).then(function (response) {
                dtpScssEditorFile.css = JSON.parse(response.data);
                
            });

            $scope.openDialog('Preview');

            return deferred.promise;
            
        };

        $scope.openDialog = function (dialogType) {

            var dialog = dialogService.open({
                template: '/App_Plugins/DtpScssEditor/backoffice/views/dialog' + dialogType + '.html', show: true, callback: function () {
                    //$log.info('Dialog' + dialogType + ' opened!');
                }
            });

        };

        $scope.closeDialog = function ($event) {
            $event.preventDefault();
            dialogService.closeAll();
            return false;
        }

        $scope.insertImport = function () {
            if ($scope.importStatement !== undefined && $scope.importStatement !== '') {                
                dtpScssEditorFile.editor.replaceSelection($scope.importStatement);
                dtpScssEditorFile.editor.focus();
                dialogService.closeAll();
            }
        };

        $scope.insertMixin = function () {
            if ($scope.selectedMixin !== undefined && $scope.selectedMixin !== '') {
                var mixin = $scope.selectedMixin.mixin.replace('{ }', '{\n\t\t\n\t}');

                $.each($scope.selectedMixin.variables, function (idx, variable) {
                    if (variable.Value === '' && variable.Key.indexOf(':') > -1)
                    {
                        var defaultValue = variable.Key.split(':');
                        variable.Value = defaultValue[1];
                    } else if (variable.Value === '') {
                        variable.Value = '\'\'';
                    }
                    //var re = new RegExp('\\' + variable.Key, 'g');
                    mixin = mixin.replace(variable.Key, variable.Value);
                });

                dtpScssEditorFile.editor.replaceSelection('@include ' + mixin);
                dtpScssEditorFile.editor.focus();
                dialogService.closeAll();
            }
        };

        $scope.insertVariable = function () {
            if ($scope.selectedVariable !== undefined && $scope.selectedVariable !== '') {
                dtpScssEditorFile.editor.replaceSelection($scope.selectedVariable.name);
                dtpScssEditorFile.editor.focus();
                dialogService.closeAll();
            }
        };

        $scope.addComment = function () {
            dtpScssEditorFile.editor.replaceSelection('/* ' + dtpScssEditorFile.editor.getSelection() + ' */');
            dtpScssEditorFile.editor.focus();
        }

    });