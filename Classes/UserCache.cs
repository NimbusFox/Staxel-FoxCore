using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staxel.FoxCore.Classes {
    public class UserCache {
        public string DisplayName { get; set; }
        public string Uid { get; set; }
        public DateTime Expires { get; set; }
    }
}
