using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using umbraco;

namespace dtp.umb.scsseditor.lib.Install
{
    public class DtpScssEditorInstaller
    {
        #region Constants

        private const string APP_VERSION_KEY = "dtpScssEditor:Version";
        private const double APP_VERSION = 0.01;

        private const string APP_ROOT_KEY = "dtpScssEditor:Root";
        private const string APP_ROOT = "~/scss/";

        private const string APP_DELETE_EMPTY_DIRECTORY_KEY = "dtpScssEditor:DeleteEmptyDirectory";
        private const string APP_DELETE_EMPTY_DIRECTORY = "true";

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
                AddUpdateAppSettings(APP_DELETE_EMPTY_DIRECTORY_KEY, APP_DELETE_EMPTY_DIRECTORY_KEY);
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
