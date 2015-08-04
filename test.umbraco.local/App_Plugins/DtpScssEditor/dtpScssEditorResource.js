angular.module("umbraco.resources")
	.factory("dtpScssEditorResource", function ($http, $log) {
	    return {
	        delete: function (path) {
	            return $http.get("backoffice/DtpScssEditor/ScssFilesApi/Delete?path=" + path);
	        },
	        create: function (path) {
	            return $http.get("backoffice/DtpScssEditor/ScssFilesApi/Create?path=" + path);
	        },
	        getFile: function (path) {
	            return $http.get("backoffice/DtpScssEditor/ScssFilesApi/GetFile?path=" + path);
	        },
	        saveFile: function (scssFile) {
	            return $http.post('backoffice/DtpScssEditor/ScssFilesApi/SaveFile', scssFile);
	        },
	        getVariables: function () {
	            return $http.get('backoffice/DtpScssEditor/ScssFilesApi/GetVariables');
	        },
	        getMixins: function () {
	            return $http.get('backoffice/DtpScssEditor/ScssFilesApi/GetMixins');
	        },
	        getCss: function (scssFile) {
	            return $http.post("backoffice/DtpScssEditor/ScssFilesApi/CompileScss", scssFile);
	        },
	        buildTreePath: function (currentNodeId) {

	            var fileExt = '';
	            if (arguments.length > 1) {
	                fileExt = arguments[1]
	            }

	            var navPath = [-1];

	            var path = currentNodeId.split('\\');

	            for (var i = 0; i < path.length; i++) {
	                var ancestorNodeId = '';
	                for (var j = 0; j <= i; j++) {
	                    ancestorNodeId += path[j] + '\\';
	                }
	                if (i === path.length - 1) {
	                    navPath.push(ancestorNodeId.substring(0, ancestorNodeId.length - 1) + fileExt);
	                } else {
	                    navPath.push(ancestorNodeId.substring(0, ancestorNodeId.length - 1));
                    }
	            }

	            //$log.debug('Tree Path: ', navPath);
	            return navPath;
	        }
	    };
	});

angular.module("umbraco.resources")
	.factory("dtpScssEditorFile", function ($http) {
	    return {
	        name: '',
	        path: '',
	        absolutePath: '',
	        rootPath: '',
	        scss: '',
            css: '',
	        reset: function () {
	            this.name = '';
	            this.path = '';
	            this.absolutePath = '';
	            this.rootPath = '';
	            this.scss = '';
	        },
            editor: null
	    };
	});