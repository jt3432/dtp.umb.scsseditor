using System.Web;
using ClientDependency.Core;
using dtp.umb.scsseditor.cd;

[assembly: PreApplicationStartMethod(typeof(StartupHandler), "Initialize")]

namespace dtp.umb.scsseditor.cd
{
    /// <summary>
    /// Class called to register the less writer
    /// </summary>
    public static class StartupHandler
    {
        public static void Initialize()
        {
            //register the sass writer.
            FileWriters.AddWriterForExtension(".scss", new ScssWriter());
        }
    }
}