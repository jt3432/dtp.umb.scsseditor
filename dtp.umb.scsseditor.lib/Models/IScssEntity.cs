using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtp.umb.scsseditor.Models
{
    public interface IScssEntity
    {
        bool IsDirectory
        {
            get;
            set;
        }
        bool IsFile
        {
            get;
            set;
        }
        bool HasChildren
        {
            get;
            set;
        }
        string PathFull
        {
            get;
            set;
        }
        string PathRelative
        {
            get;
            set;
        }
        string Name
        {
            get;
            set;
        }
    }
}
