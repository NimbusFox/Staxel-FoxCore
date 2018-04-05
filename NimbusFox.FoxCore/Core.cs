using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        [Obsolete("Please use the new DirectoryManager")]
        public readonly FileManager FileManager;
        public readonly WorldManager WorldManager;
        public readonly ParticleManager ParticleManager;
        public readonly EntityParticleManager EntityParticleManager;
        public readonly EntityFollowParticleManager EntityFollowParticleManager;
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public UserManager UserManager => CoreHook.UserManager;
        public readonly DirectoryManager DirectoryManager;
        internal readonly DirectoryManager ModDirectory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <param name="mod">Must match your mod directory name</param>
        /// <param name="modVersion"></param>
        public Fox_Core(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            FileManager = new FileManager(author, mod);
            WorldManager = new WorldManager();
            ParticleManager = new ParticleManager();
            EntityParticleManager = new EntityParticleManager();
            EntityFollowParticleManager = new EntityFollowParticleManager();
            DirectoryManager = new DirectoryManager(author, mod);
            ModDirectory = new DirectoryManager(mod);
            VersionCheck.VersionCheck.Check();
        }

        public static TInterface ResolveOptionalDependency<TInterface>(string key) {
            var collection = GetDependencies<TInterface>(key);

            return collection.FirstOrDefault();
        }

        public static List<TInterface> GetDependencies<TInterface>(string key) {
            var output = new List<TInterface>();
            var assembly = Assembly.GetAssembly(typeof(Fox_Core));
            var dir = assembly.Location.Substring(0, assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.mod")) {
                var data = BlobAllocator.AcquireAllocator().NewBlob(true);
                data.ReadJson(File.ReadAllText(file.FullName));
                if (data.Contains("fxdependencykeys")) {
                    var current = data.GetStringList("fxdependencykeys");
                    if (current.Any(x => string.Equals(x, key, StringComparison.CurrentCultureIgnoreCase))) {
                        var item = Assembly.Load(Assembly.LoadFile(file.FullName.Replace(".mod", ".dll")).GetName());
                        foreach (var module in item.DefinedTypes) {
                            if (module.GetInterfaces().Contains(typeof(TInterface))) {
                                output.Add((TInterface)Activator.CreateInstance(module));
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static void VectorLoop(Vector3I start, Vector3I end, Action<int, int, int> coordFunction) {
            var region = new VectorCubeI(start, end);

            for (var y = region.Start.Y; y <= region.End.Y; y++) {
                for (var z = region.Start.Z; z <= region.End.Z; z++) {
                    for (var x = region.Start.X; x <= region.End.X; x++) {
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
