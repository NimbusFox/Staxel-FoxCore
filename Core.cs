using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusFox.FoxCore {
    public class FxCore {
        public readonly ExceptionManager ExceptionManager;
        public readonly FileManager FileManager;

        public FxCore(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            FileManager = new FileManager(author, mod);
        }
    }
}
