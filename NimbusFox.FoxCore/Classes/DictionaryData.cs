using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staxel.FoxCore.Classes {
    public class DictionaryData {
        public Dictionary<string, string> Variants { get; set; }

        public DictionaryData() {
            Variants = new Dictionary<string, string>();
        }
    }
}
