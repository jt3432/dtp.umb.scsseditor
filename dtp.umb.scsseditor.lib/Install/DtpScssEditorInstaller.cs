using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using umbraco;
using Umbraco.Core;

namespace dtp.umb.scsseditor.lib.Install
{
    public class DtpScssEditorInstaller
    {
        #region Constants

        private const string APP_VERSION_KEY = "dtpScssEditor:Version";
        private const double APP_VERSION = 2.2;

        private const string APP_ROOT_KEY = "dtpScssEditor:Root";
        private const string APP_ROOT = "~/scss/";

        private const string APP_DELETE_EMPTY_DIRECTORY_KEY = "dtpScssEditor:DeleteEmptyDirectory";
        private const string APP_DELETE_EMPTY_DIRECTORY = "true";

        private const string HANDLER_NAME = "DtpScssHandler";
        private const string HANDLER_PATH = "*.scss";
        private const string HANDLER_VERB = "GET";
        private const string HANDLER_TYPE = "dtp.umb.scsseditor.cd.ScssHandler, dtp.umb.scsseditor.cd";

        #endregion

        #region Private Variables

        #endregion

        #region Public Methods

        public static void InstallAppSettingKeys()
        {
            if (ReadSetting(APP_VERSION_KEY) == "Not Found")
            {
                AddUpdateAppSettings(APP_VERSION_KEY, APP_VERSION.ToString());
                AddUpdateAppSettings(APP_ROOT_KEY, APP_ROOT);
                AddUpdateAppSettings(APP_DELETE_EMPTY_DIRECTORY_KEY, APP_DELETE_EMPTY_DIRECTORY);
            }
        }

        public static void ResetClientDependencyVersion()
        {
            var cdConfigPath = HttpContext.Current.Server.MapPath(@"~/Config/ClientDependency.config");

            XmlDocument cdConfig = new XmlDocument();
            cdConfig.Load(cdConfigPath);

            XmlNode clientDependencyNode = cdConfig.SelectSingleNode("clientDependency");
            clientDependencyNode.Attributes["version"].Value = "0";

            cdConfig.Save(cdConfigPath);
        }

        // Thank you to Package Action Contrib for most of this code.
        public static void InstallHttpHandlerReferences()
        {
            var webConfigPath = HttpContext.Current.Server.MapPath(@"~/web.config");

            XmlDocument webConfig = new XmlDocument();
            webConfig.Load(webConfigPath);

            XmlNode rootNode = webConfig.SelectSingleNode("//configuration/system.web/httpHandlers");

            bool modified = false;
            bool insertNode = true;

            if (rootNode != null)
            {
                if (rootNode.HasChildNodes)
                {
                    XmlNode node = rootNode.SelectSingleNode(String.Format("add[@path = '{0}']", HANDLER_PATH));
                    insertNode = (node == null);
                }

                if (insertNode)
                {
                    XmlNode newNode = webConfig.CreateElement("add");
                    newNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "path", HANDLER_PATH));
                    newNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "verb", HANDLER_VERB));
                    newNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "type", HANDLER_TYPE));

                    rootNode.AppendChild(newNode);

                    modified = true;
                }
            }

            insertNode = true;

            rootNode = webConfig.SelectSingleNode("//configuration/system.webServer/handlers");

            if (rootNode != null)
            {
                if (rootNode.HasChildNodes)
                {
                    XmlNode node = rootNode.SelectSingleNode(String.Format("add[@name = '{0}']", HANDLER_NAME));
                    insertNode = (node == null);
                }

                if (insertNode)
                {
                    XmlNode newRemoveNode = webConfig.CreateElement("remove");
                    newRemoveNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "name", HANDLER_NAME));

                    XmlNode newAddNode = webConfig.CreateElement("add");
                    newAddNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "name", HANDLER_NAME));
                    newAddNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "path", HANDLER_PATH));
                    newAddNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "verb", HANDLER_VERB));
                    newAddNode.Attributes.Append(XmlHelper.AddAttribute(webConfig, "type", HANDLER_TYPE));

                    rootNode.AppendChild(newRemoveNode);
                    rootNode.AppendChild(newAddNode);

                    modified = true;

                }
            }

            if (modified)
            {
                try
                {
                    webConfig.Save(webConfigPath);
                }
                catch (Exception ex)
                {
                    string message = "Error at execute AddHttpHandler package action: " + ex.Message;
                    Umbraco.Core.Logging.LogHelper.Error(typeof(DtpScssEditorInstaller), message, ex);
                }
            }


        }

        #endregion

        #region Private Methods

        private static string ReadSetting(string key)
        {
            string result = String.Empty;
            try
            {
                var appSettings = WebConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
            }
            catch (Exception)
            {
                // Opps
            }

            return result;
        }

        private static bool AddUpdateAppSettings(string key, string value)
        {
            bool success = false;
            try
            {
                var configFile = WebConfigurationManager.OpenWebConfiguration("/");
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                success = true;
            }
            catch (Exception)
            {
                // Opps
            }
            return success;
        }

        #endregion

    }
}
