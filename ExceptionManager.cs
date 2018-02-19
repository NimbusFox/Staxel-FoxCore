using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Staxel;

namespace NimbusFox.FoxCore {
    public class ExceptionManager {
        private string _modVersion;
        private readonly string StreamLocation;
        private readonly string LocalContentLocation;
        private readonly string ErrorsDir;

        public ExceptionManager(string author, string mod, string modVersion) {
            StreamLocation = $"Mods\\{author}\\{mod}\\";
            LocalContentLocation = GameContext.ContentLoader.LocalContentDirectory + StreamLocation;
            ErrorsDir = LocalContentLocation + "Errors\\";
            _modVersion = modVersion;
        }

        public void HandleException(Exception ex, Dictionary<string, object> extras = null) {
            if (!Directory.Exists(ErrorsDir)) {
                Directory.CreateDirectory(ErrorsDir);
            }

            var filename = DateTime.Now.Ticks;

            File.AppendAllText(ErrorsDir + $"{filename}.{_modVersion}.error", JsonConvert.SerializeObject(ex, Formatting.Indented));
            if (extras != null) {
                if (extras.Any()) {
                    foreach (var file in extras) {
                        File.AppendAllText(ErrorsDir + $"{filename}.{_modVersion}.{file.Key}.data", JsonConvert.SerializeObject(file.Value, Formatting.Indented));
                    }
                }
            }
        }
    }
}
