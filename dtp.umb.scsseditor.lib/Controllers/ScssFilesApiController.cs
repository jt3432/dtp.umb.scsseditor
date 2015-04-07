using dtp.umb.scsseditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi.Filters;
using Umbraco.Core.Persistence;
using Umbraco.Core;
using ClientDependency.Core;
using ClientDependency.Core.Config;
using System.Configuration;
using System.Xml;

namespace dtp.umb.scsseditor.Controllers
{
    [PluginController("DtpScssEditor")]
    [DisableBrowserCache]
    public class ScssFilesApiController : UmbracoAuthorizedJsonController
    {
        private string _rootScssPath = String.Empty;
        private bool  _deleteEmptyDirectory = true;

        public ScssFilesApiController()
        {
            var configFile = WebConfigurationManager.OpenWebConfiguration("/");
            var settings = configFile.AppSettings.Settings;
            _rootScssPath = settings["dtpScssEditor:Root"].Value;

            Boolean.TryParse(settings["dtpScssEditor:DeleteEmptyDirectory"].Value, out _deleteEmptyDirectory);

            if (_rootScssPath.StartsWith("/") || _rootScssPath.StartsWith("~"))
            {
                _rootScssPath = HttpContext.Current.Server.MapPath(_rootScssPath);
            }

            if (!_rootScssPath.EndsWith(@"\"))
            {
                _rootScssPath = String.Format(@"{0}\", _rootScssPath);
            }
        }

        public IEnumerable<IScssEntity> GetFiles(string relativeDirectoryPath = "")
        {
            List<IScssEntity> scssFiles = new List<IScssEntity>();

            if (relativeDirectoryPath.StartsWith(@"\") || relativeDirectoryPath.StartsWith("/"))
            {
                relativeDirectoryPath = relativeDirectoryPath.TrimStart('\\').TrimStart('/');
            }

            DirectoryInfo currentDirectory = new DirectoryInfo(String.Format("{0}{1}", _rootScssPath, relativeDirectoryPath.Replace("/", @"\")));

            if (!currentDirectory.Exists)
            {
                currentDirectory.Create();
            }

            var directories = currentDirectory.GetDirectories();
            var files = currentDirectory.GetFiles("*.scss");

            foreach (var directory in directories)
            {
                var isDirectory = directory.GetFiles().Count() > 0;
                scssFiles.Add(new ScssDirectory(directory.FullName, _rootScssPath, directory.Name, isDirectory));
            }
             
            foreach (var file in files)
            {
                currentDirectory.Create();
                scssFiles.Add(new ScssFile(file.FullName, _rootScssPath, file.Name));
            }

            return scssFiles;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public bool Delete(string path)
        {
            var success = true;

            if (!path.StartsWith(_rootScssPath))
            {
                path =String.Format("{0}{1}", _rootScssPath, path);
            }

            if (path.EndsWith(".scss"))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch
                {
                    success = false;
                }
            }
            else
            {
                DirectoryInfo currentDirectory = new DirectoryInfo(path);
                if (!_deleteEmptyDirectory || (_deleteEmptyDirectory && currentDirectory.GetFiles().Count() == 0))
                {                    
                    try
                    {
                        currentDirectory.Delete(true);
                    }
                    catch
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public void Create(string path)
        {
            string relativeFilePath = path;

            if (relativeFilePath.EndsWith(".scss"))
            {
                relativeFilePath = relativeFilePath.Replace(".scss", String.Empty);
            }

            if (relativeFilePath.StartsWith(@"\") || relativeFilePath.StartsWith("/"))
            {
                relativeFilePath = relativeFilePath.TrimStart('\\').TrimStart('/');
            }

            var fullPath = String.Format("{0}{1}.scss", _rootScssPath, relativeFilePath.Replace("/", @"\"));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                fs.Close();
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public ScssFile GetFile(string path)
        {
            string relativeFilePath = path;

            if (relativeFilePath.EndsWith(".scss"))
            {
                relativeFilePath = relativeFilePath.Replace(".scss", String.Empty);
            }

            if (relativeFilePath.StartsWith(@"\") || relativeFilePath.StartsWith("/"))
            {
                relativeFilePath = relativeFilePath.TrimStart('\\').TrimStart('/');
            }

            var fullPath = String.Format("{0}{1}.scss", _rootScssPath, relativeFilePath.Replace("/", @"\"));
            string fileContents = System.IO.File.ReadAllText(fullPath);

            var file = new ScssFile(fullPath, _rootScssPath, Path.GetFileName(fullPath), fileContents);

            return file;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public bool SaveFile(ScssFile scssFile)
        {
            bool success = true;
            
            var path = Path.Combine(_rootScssPath, scssFile.PathRelative);
            try
            {
                System.IO.File.WriteAllText(path, scssFile.Content);

                ClientDependencySection cdSection = (ClientDependencySection)ConfigurationManager.GetSection("clientDependency");
                var version = cdSection.Version;

                var cdConfigPath = HttpContext.Current.Server.MapPath(@"~/Config/ClientDependency.config");

                XmlDocument cdConfig = new XmlDocument();
                cdConfig.Load(cdConfigPath);

                XmlNode clientDependencyNode = cdConfig.SelectSingleNode("clientDependency");
                clientDependencyNode.Attributes["version"].Value = String.Format("{0}", version + 1);

                cdConfig.Save(cdConfigPath);
                
            }
            catch
            {
                success = false;
            }

            if (success && !scssFile.PathFull.Contains(scssFile.PathRelative))
            {
                System.IO.File.Delete(scssFile.PathFull);
            }

            return success;
        }
    }
}
