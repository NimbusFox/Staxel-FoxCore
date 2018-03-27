using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Staxel;
using Staxel.FoxCore.Managers;

namespace NimbusFox.FoxCore.Managers {
    public class ExceptionManager {
        private string _modVersion;
        private readonly DirectoryManager ErrorsDir;

        internal ExceptionManager(string author, string mod, string modVersion) {
            ErrorsDir = new DirectoryManager(author, mod).FetchDirectory("Errors");
            _modVersion = modVersion;
        }

        public void HandleException(Exception ex, Dictionary<string, object> extras = null) {
            var filename = DateTime.Now.Ticks;

            var dic = new Dictionary<string, string> {
                {"Message", ex.Message },
                {"Stack", ex.StackTrace },
                {"Inner", ex.InnerException?.Message }
            };

            ErrorsDir.WriteFile($"{filename}.{_modVersion}.error", dic, null, true);

            if (extras != null) {
                if (extras.Any()) {
                    foreach (var file in extras) {
                        ErrorsDir.WriteFile($"{filename}.{_modVersion}.{file.Key}.data", file.Value, null, true);
                    }
                }
            }
        }
    }
}
