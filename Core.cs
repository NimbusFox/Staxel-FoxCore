﻿using System;
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

        public static TInterface ResolveOptionalDependency<TInterface>(string key) {
            var assembly = Assembly.GetAssembly(typeof(FxCore));
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
                                    Console.WriteLine("FOUND ONE");
                                    return (TInterface)Activator.CreateInstance(module.ReflectedType);
                                }
                            }
                        }
                    }
                }
            }

            return default(TInterface);
        }

        public static List<TInterface> GetDependencies<TInterface>(string key) {
            var output = new List<TInterface>();

            var assembly = Assembly.GetAssembly(typeof(FxCore));
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
                                    Console.WriteLine("FOUND ONE");
                                    output.Add((TInterface)Activator.CreateInstance(module.ReflectedType));
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }
    }
}
