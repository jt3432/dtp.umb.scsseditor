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
    public class MixinModel
    {
        [DataMember(Name = "mixin")]
        public string Mixin { get; set; }

        [DataMember(Name = "group")]
        public string Group { get; set; }

        [DataMember(Name = "variables")]
        public IEnumerable<KeyValuePair<string, string>> Variables { get; set; }
    }
}
