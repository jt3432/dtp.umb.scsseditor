# dtp.umb.scsseditor
Umbraco 7 Backoffice SCSS Editor

Once you go SCSS it is had to go back! This simple backoffice extention adds a SCSS file editor and ClientDependency compiler to our favorite CMS.

To install:

Download package and install, than add the below httpHandlers reference into your web.config

<add path="*.scss" verb="GET" type="dtp.umb.scsseditor.cd.SassHandler, dtp.umb.scsseditor.cd" />
The extention will add a three app settings key to the web.config:
<add key="dtpScssEditor:Version" value="0.01" />
<add key="dtpScssEditor:Root" value="~/scss/" />
<add key="dtpScssEditor:DeleteEmptyDirectory" value="true" />
dtpScssEditor:Version, is the extention version
dtpScssEditor:Root, is the configurable location of your scss files
dtpScssEditor:DeleteEmptyDirectory, when true prevents the deletion of directories that are not empty
To use your SCSS file in your site include the ClientDependency MVC handler in your view, then add you SCSS file with the RequiresCss helper.
@using ClientDependency.Core.Mvc;
@{
    Html.RequiresCss("~/scss/main.scss");  
}
Don't forget to add the output helper where you want the compiled css to render.
@Html.RenderCssHere();  
Note: See ClientDependency (CD) GitHub for CD documentation: https://github.com/Shazwazza/ClientDependency
