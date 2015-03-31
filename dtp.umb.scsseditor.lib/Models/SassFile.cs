using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Umbraco.Core.IO;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace dtp.umb.scsseditor.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ScssFile : IScssEntity
    {
        public ScssFile()
        { }

        public ScssFile(string fullPath, string rootPath, string name, string content = "")
        {
            IsFile = true;
            IsDirectory = false;
            HasChildren = false;
            PathFull = fullPath;
            PathRelative = fullPath.Replace(rootPath, "");
            Name = name;
            Content = content;
        }

        [DataMember(Name = "isDirectory")]
        public bool IsDirectory
        {
            get;
            set;
        }
        [DataMember(Name = "isFile")]
        public bool IsFile
        {
            get;
            set;
        }
        [DataMember(Name = "hasChildren")]
        public bool HasChildren
        {
            get;
            set;
        }
        [DataMember(Name = "pathFull")]
        public string PathFull
        {
            get;
            set;
        }
        [DataMember(Name = "pathRelative")]
        public string PathRelative
        {
            get;
            set;
        }
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        [DataMember(Name = "content")]
        public string Content
        {
            get;
            set;
        }    
    }
}