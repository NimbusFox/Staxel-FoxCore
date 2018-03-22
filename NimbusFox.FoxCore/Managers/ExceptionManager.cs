using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Staxel;

namespace NimbusFox.FoxCore.Managers {
    public class ExceptionManager : FileManager {
        private string _modVersion;
        private readonly string ErrorsDir;

        internal ExceptionManager(string author, string mod, string modVersion) : base(author, mod) {
            ErrorsDir = Path.Combine(LocalContentLocation, "Errors");
            _modVersion = modVersion;
        }

        public void HandleException(Exception ex, Dictionary<string, object> extras = null) {
            if (!Directory.Exists(ErrorsDir)) {
                Directory.CreateDirectory(ErrorsDir);
            }

            var filename = DateTime.Now.Ticks;

            File.AppendAllText(Path.Combine(ErrorsDir, $"{filename}.{_modVersion}.error"), SerializeObject(ex));
            if (extras != null) {
                if (extras.Any()) {
                    foreach (var file in extras) {
                        File.AppendAllText(Path.Combine(ErrorsDir, $"{filename}.{_modVersion}.{file.Key}.data"), SerializeObject(file.Value));
                    }
                }
            }
        }
    }
}
