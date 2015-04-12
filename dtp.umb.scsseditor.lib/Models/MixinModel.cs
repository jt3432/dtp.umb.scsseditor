using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtp.umb.scsseditor.lib.Models
{
    public class MixinModel
    {
        public string Mixin { get; set; }
        public string Group { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Variables { get; set; }
    }
}
