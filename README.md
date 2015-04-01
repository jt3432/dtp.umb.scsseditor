## dtp.umb.scsseditor
###Umbraco 7 Backoffice SCSS Editor

Once you go SCSS (http://sass-lang.com/) it is had to go back! This simple backoffice extention adds a SCSS file editor and ClientDependency compiler to our favorite CMS.


**To install:**

Download package and install via the Umbraco backoffice, than add the below httpHandler reference into your web.config under the <httpHandlers> node:

```
<add path="*.scss" verb="GET" type="dtp.umb.scsseditor.cd.SassHandler, dtp.umb.scsseditor.cd" />
```

Also add this handler under the <system.webServer><handlers> node:

```
<remove name="DtpScssHandler" />
<add name="DtpScssHandler" path="*.scss" verb="GET" type="dtp.umb.scsseditor.cd.ScssHandler, dtp.umb.scsseditor.cd" resourceType="File" preCondition="" />
```


**Web.Config settings:**

The extention will add three app settings key to the web.config:

```
<add key="dtpScssEditor:Version" value="0.01" />  
<add key="dtpScssEditor:Root" value="~/scss/" />  
<add key="dtpScssEditor:DeleteEmptyDirectory" value="true" />
```


* **dtpScssEditor:Version**, is the extention version. Just for reference, recommend not to change.
* **dtpScssEditor:Root**, is the configurable location of your scss files. Default is ~/scss/ and the directory will be create automaticly.
* **dtpScssEditor:DeleteEmptyDirectory**, when true prevents the deletion of directories that are not empty. Just a save guard to protect you from deleting all your hard work.


**Compile**
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

Note: See Client Dependency (CD) GitHub for CD documentation: https://github.com/Shazwazza/ClientDependency
