using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dtp.umb.scsseditor.lib.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class SavedFileResponseModel
    {
        [DataMember(Name = "cssOutput")]
        public string CssOutput { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "saveSuccess")]
        public bool SaveSuccess { get; set; }

        [DataMember(Name = "compileSuccess")]
        public bool CompileSuccess { get; set; }

        [DataMember(Name = "isPartial")]
        public bool IsPartial { get; set; }
    }
}
