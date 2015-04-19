using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClientDependency.Core;
using ClientDependency.Core.CompositeFiles;
using ClientDependency.Core.CompositeFiles.Providers;
using ClientDependency.Core.Config;
using System.Web.Configuration;
using LibSassNet;

namespace dtp.umb.scsseditor.cd
{
    /// <summary>
    /// A file writer for dotLess
    /// </summary>
    public sealed class ScssWriter : IFileWriter
    {
        private static readonly ISassCompiler _sassCompiler = new SassCompiler();

        public bool WriteToStream(BaseCompositeFileProcessingProvider provider, StreamWriter sw, FileInfo fi, ClientDependencyType type, string origUrl, HttpContextBase http)
        {
            try
            {
                //this stores a reference of all referenced files during processing
                var accessedFiles = new List<string>();

                //NOTE: We don't want this compressed since CDF will do that ourselves
                var output = _sassCompiler.CompileFile(fi.FullName);

                DefaultFileWriter.WriteContentToStream(provider, sw, output.CSS, type, http, origUrl);

                return true;
            }
            catch (Exception ex)
            {
                ClientDependencySettings.Instance.Logger.Error(string.Format("Could not write file {0} contents to stream. EXCEPTION: {1}", fi.FullName, ex.Message), ex);
                return false;
            }
        }

        /// <summary>
        /// Get the Sass compiled string output from the file
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public CompileFileResult GetOutput(FileInfo fileInfo)
        {
            CompileFileResult result = new CompileFileResult();
            try
            {
                CompilationSection compilationSection = (CompilationSection)System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation");
                if (compilationSection.Debug)
                {
                    //result = _sassCompiler.CompileFile(fileInfo.FullName, outputStyle: OutputStyle.Nested, precision: 10, includeSourceComments: false, sourceMapPath: String.Format("{0}.map",fileInfo.FullName));
                    result = _sassCompiler.CompileFile(fileInfo.FullName, outputStyle: OutputStyle.Nested, precision: 10, includeSourceComments: false);
                }
                else
                {
                    result = _sassCompiler.CompileFile(fileInfo.FullName, outputStyle: OutputStyle.Compressed, precision: 10, includeSourceComments: false);
                }
            }
            catch (SassCompileException sassCompEx)
            {
                result = new CompileFileResult(String.Format("/* {0} */", sassCompEx.Message.Replace("srdin:", "line ")), String.Empty);
            }
            catch (Exception ex)
            {
                result = new CompileFileResult(String.Format("/* {0} */", ex.Message), String.Empty);
            }

            return result;
        }
    }
}