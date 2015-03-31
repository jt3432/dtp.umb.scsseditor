angular.module("umbraco.directives")
     .directive('dtpngCodemirror', function ($timeout, $log) {
         return {             
             restrict: 'A',
             require: '?ngModel',
             link: function ($scope, element, attrs, ngModel) {

                 if (angular.isUndefined(window.CodeMirror)) {
                     throw new Error('dtpng-codemirror needs CodeMirror to work... ');
                 }

                 element.html('');

                 $scope.codemirror = new window.CodeMirror.fromTextArea(element[0], {
                     lineNumbers: true,
                     matchBrackets: true,
                     lineWrapping: true,
                     autoCloseTags: true,
                     mode: 'text/x-scss'
                 });

                 ngModel.$formatters.push(function (modelValue) {
                     if (modelValue) {
                         //$log.debug('codemirror ng-model: ', modelValue);
                         $scope.codemirror.setValue(modelValue.scss);
                     }
                 });
                 
                 $scope.codemirror.on('change', function(instance) {
                     var newValue = instance.getValue();
                     if (newValue !== ngModel.$viewValue) {
                         $scope.$evalAsync(function() {                          
                             ngModel.$modelValue.scss = newValue;
                         });
                     }
                 });

                 $scope.codemirror.setSize('auto', '100%');

                 $timeout(function () {
                     $scope.codemirror.refresh();
                 }, 1000);                 
             }
         };
     });