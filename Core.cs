using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NimbusFox.FoxCore.Managers;
using NimbusFox.FoxCore.Managers.Particles;
using Staxel.FoxCore.Managers.Particles;
using WorldManager = NimbusFox.FoxCore.Managers.WorldManager;

namespace NimbusFox.FoxCore {
    public class FxCore {
        public readonly ExceptionManager ExceptionManager;
        public readonly FileManager FileManager;
        public readonly WorldManager WorldManager;
        public readonly ParticleManager ParticleManager;
        public readonly EntityParticleManager EntityParticleManager;
        public readonly EntityFollowParticleManager EntityFollowParticleManager;

        public FxCore(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            FileManager = new FileManager(author, mod);
            WorldManager = new WorldManager();
            ParticleManager = new ParticleManager();
            EntityParticleManager = new EntityParticleManager();
            EntityFollowParticleManager = new EntityFollowParticleManager();
        }

        public static T ResolveOptionalDependency<T>(string key) {
            var dir = Assembly.GetExecutingAssembly().Location;
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.mod")) {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file.FullName));

                if (data.Any(x => x.Key.ToLower() == "fxdependencykeys")) {
                    var current = data[data.First(x => x.Key.ToLower() == "fxdependencykeys").Key];

                    if (current is string[] keys) {
                        if (keys.Any()) {
                            if (keys.Any(x => string.Equals(x, key, StringComparison.CurrentCultureIgnoreCase))) {
                                var item = Assembly.LoadFile(file.FullName.Replace(".mod", ".dll"));
                                foreach (var module in item.GetModules()) {
                                    if (module.GetType() is T) {
                                        return (T)Activator.CreateInstance(module.GetType());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return default(T);
        }
    }
}
