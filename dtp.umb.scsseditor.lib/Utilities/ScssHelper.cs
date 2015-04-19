using LibSassNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtp.umb.scsseditor.lib.Utilities
{
    public static class ScssHelper
    {
        public static bool ScssCompile(string scss, string rootPath, out string cssOutput, out string message)
        {
            cssOutput = String.Empty;
            message = String.Empty;

            bool success = true;
            ISassCompiler sassCompiler = new SassCompiler();

            List<string> paths = new List<string>() { rootPath };

            foreach (var directory in Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories))
            {
                paths.Add(directory);
            }

            try
            {
                cssOutput = sassCompiler.Compile(scss, OutputStyle.Nested, precision: 10, includePaths: paths, includeSourceComments: false);
                message = "Successfully compiles SCSS file.";
            }
            catch (SassCompileException sassCompEx)
            {
                message = sassCompEx.Message.Replace("stdin:", "line ");
                success = false;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                success = false;
            }

            return success;
        }
    }
}
