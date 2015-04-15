﻿using System.IO;
using System.Web;
using ClientDependency.Core;

namespace dtp.umb.scsseditor.cd
{
    /// <summary>
    /// An http handler for .coffee extensions which is used when in debug mode
    /// </summary>
    public class ScssHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var localPath = context.Request.Url.LocalPath;
            if (string.IsNullOrEmpty(localPath)) return;
            var file = new FileInfo(context.Server.MapPath(localPath));
            var writer = new ScssWriter();
            var output = writer.GetOutput(file);
            context.Response.ContentType = "text/css";
            context.Response.Write(output.CSS);
            if(!string.IsNullOrEmpty(output.SourceMap))
            {
                System.IO.File.WriteAllText(string.Format("{0}.map", file.FullName), output.SourceMap);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}