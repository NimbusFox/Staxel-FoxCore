using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Managers;
using NimbusFox.FoxCore.Managers.Particles;
using Plukit.Base;
using Staxel.FoxCore.Managers;
using Staxel.FoxCore.Managers.Particles;
using WorldManager = NimbusFox.FoxCore.Managers.WorldManager;

namespace NimbusFox.FoxCore {
    public class Fox_Core {
        public readonly ExceptionManager ExceptionManager;
        public readonly FileManager FileManager;
        public readonly WorldManager WorldManager;
        public readonly ParticleManager ParticleManager;
        public readonly EntityParticleManager EntityParticleManager;
        public readonly EntityFollowParticleManager EntityFollowParticleManager;
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public UserManager UserManager => CoreHook.UserManager;
        public TileManager TileManager => CoreHook.TileManager;

        public Fox_Core(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            FileManager = new FileManager(author, mod);
            WorldManager = new WorldManager();
            ParticleManager = new ParticleManager();
            EntityParticleManager = new EntityParticleManager();
            EntityFollowParticleManager = new EntityFollowParticleManager();
            VersionCheck.VersionCheck.Check();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public TInterface ResolveOptionalDependency<TInterface>(string key) {
            var assembly = Assembly.GetAssembly(typeof(Fox_Core));
            var dir = assembly.Location.Substring(0, assembly.Location.LastIndexOf("\\", StringComparison.Ordinal));
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.mod")) {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file.FullName));
                if (data.Any(x => x.Key.ToLower() == "fxdependencykeys")) {
                    var current = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(data[data.First(x => x.Key.ToLower() == "fxdependencykeys").Key]));
                    if (current.Any()) {
                        if (current.Any(x => string.Equals(x, key, StringComparison.CurrentCultureIgnoreCase))) {
                            var item = Assembly.LoadFile(file.FullName.Replace(".mod", ".dll"));
                            foreach (var module in item.DefinedTypes) {
                                if (module.ReflectedType != null && module.ReflectedType.GetInterfaces().Contains(typeof(TInterface))) {
                                    return (TInterface)Activator.CreateInstance(module.ReflectedType);
                                }
                            }
                        }
                    }
                }
            }

            return default(TInterface);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public List<TInterface> GetDependencies<TInterface>(string key) {
            var output = new List<TInterface>();

            var assembly = Assembly.GetAssembly(typeof(Fox_Core));
            var dir = assembly.Location.Substring(0, assembly.Location.LastIndexOf("\\", StringComparison.Ordinal));
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.mod")) {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file.FullName));
                if (data.Any(x => x.Key.ToLower() == "fxdependencykeys")) {
                    var current = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(data[data.First(x => x.Key.ToLower() == "fxdependencykeys").Key]));
                    if (current.Any()) {
                        if (current.Any(x => string.Equals(x, key, StringComparison.CurrentCultureIgnoreCase))) {
                            var item = Assembly.LoadFile(file.FullName.Replace(".mod", ".dll"));
                            foreach (var module in item.DefinedTypes) {
                                if (module.ReflectedType != null && module.ReflectedType.GetInterfaces().Contains(typeof(TInterface))) {
                                    output.Add((TInterface)Activator.CreateInstance(module.ReflectedType));
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static void VectorLoop(Vector3I start, Vector3I end, Action<int, int, int> coordFunction) {
            var region = new VectorCubeI(start, end);

            for (var x = region.X.Start; x <= region.X.End; x++) {
                for (var y = region.Y.Start; y <= region.Y.End; y++) {
                    for (var z = region.Z.Start; z <= region.Z.End; z++) {
                        coordFunction(x, y, z);
                    }
                }
            }
        }

        public static void VectorLoop(Vector3D start, Vector3D end, Action<int, int, int> coordFunction) {
            VectorLoop(start.From3Dto3I(), end.From3Dto3I(), coordFunction);
        }
    }
}
