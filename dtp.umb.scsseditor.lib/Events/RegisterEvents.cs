using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using dtp.umb.scsseditor.lib.Install;

namespace dtp.umb.foodmenu.Events
{
    public class RegisterEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DtpScssEditorInstaller.InstallAppSettingKeys();
        }
    }
}