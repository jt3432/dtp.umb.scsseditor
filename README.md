##Umbraco 7 Backoffice SCSS Editor

Once you go SCSS (http://sass-lang.com/) it is hard to go back! This simple back office extension adds a SCSS file editor and ClientDependency compiler to our favorite CMS.

Umbraco Package Download: https://our.umbraco.org/projects/backoffice-extensions/backoffice-scss-editor

<br />
**To install:**<br />
Download package and install via the Umbraco back office, done.

<br />
**Web.Config settings:**<br />
The extension will add three appSettings key to the web.config:
```
<add key="dtpScssEditor:Version" value="0.01" />  
<add key="dtpScssEditor:Root" value="~/scss/" />  
<add key="dtpScssEditor:DeleteEmptyDirectory" value="true" />
```

* **dtpScssEditor:Version**, is the extension version. Just for reference, recommend not to change.
* *dtpScssEditor:Root**, is the configurable location of your scss files. Default is ~/scss/ and the directory will be created automatically.
* **dtpScssEditor:DeleteEmptyDirectory**, when true prevents the deletion of directories that are not empty. Just a safeguard to protect you from deleting all your hard work.

<br />
**Using your SCSS files:**<br />
The point of this is to have some runtime compiled SCSS to CSS. To use your SCSS file in your site/templates include the Client Dependency MVC handler in your view, then add you SCSS file with the Client Dependency RequiresCss helper.
```
@using ClientDependency.Core.Mvc;
@{
    Html.RequiresCss("~/scss/main.scss");  
}
```

Don't forget to add the output helper where you want the compiled SCSS to render.
```
@Html.RenderCssHere();  
```

If you are making changes to your SCSS files and not seeing any change in your site it could be because of SCSS error or the Client Dependency Framework cachingd. Try turning on debugging in the web.config. 

```
<compilation defaultLanguage="c#" debug="true" batch="false" targetFramework="4.5" /> 
```
<br />
**Note:** See Client Dependency (CD) GitHub for CD documentation: https://github.com/Shazwazza/ClientDependency
