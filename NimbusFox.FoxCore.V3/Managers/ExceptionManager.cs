using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Managers {
    public class ExceptionManager {
        private string _modVersion;
        private readonly DirectoryManager _errorsDir;
        private readonly string _author;
        private readonly string _mod;

        internal ExceptionManager(string author, string mod, string modVersion) {
            _errorsDir = new DirectoryManager().FetchDirectoryNoParent("modErrors").FetchDirectoryNoParent(mod);
            _modVersion = modVersion;
            _author = author;
            _mod = mod;
        }

        public void HandleException(Exception ex, Dictionary<string, object> extras = null) {
            var filename = DateTime.Now.Ticks;

            _errorsDir.WriteFile($"{filename}.{_modVersion}.error", JsonConvert.SerializeObject(ex, Formatting.Indented), null, true);

            if (extras != null) {
                if (extras.Any()) {
                    _errorsDir.WriteFile($"{filename}.{_modVersion}.data", JsonConvert.SerializeObject(extras, Formatting.Indented), null, true);
                }
            }
        }
    }
}
