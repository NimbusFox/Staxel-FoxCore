using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Managers;
using NimbusFox.FoxCore.Managers.Particles;
using Plukit.Base;
using Staxel.Logic;
using WorldManager = NimbusFox.FoxCore.Managers.WorldManager;

namespace NimbusFox.FoxCore {
    public class Fox_Core {
        public ExceptionManager ExceptionManager { get; }
        public WorldManager WorldManager { get; }
        [Obsolete("Can be buggy at times so will be removed in a future version")]
        public ParticleManager ParticleManager { get; }
        [Obsolete("Can be buggy at times so will be removed in a future version")]
        public EntityParticleManager EntityParticleManager { get; }
        [Obsolete("Can be buggy at times so will be removed in a future version")]
        public EntityFollowParticleManager EntityFollowParticleManager { get; }
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public UserManager UserManager => CoreHook.UserManager;
        public DirectoryManager SaveDirectory { get; }
        public DirectoryManager ModDirectory { get; }
        public DirectoryManager ModsDirectory { get; }
        public DirectoryManager ConfigDirectory { get; }
        public DirectoryManager ContentDirectory { get; }

        public bool IsLocalServer => CoreHook.ServerMainLoop != null && CoreHook.ServerMainLoop.isLocal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <param name="mod">Must match your mod directory name</param>
        /// <param name="modVersion"></param>
        public Fox_Core(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(mod, modVersion);
            WorldManager = new WorldManager();
            ParticleManager = new ParticleManager();
            EntityParticleManager = new EntityParticleManager();
            EntityFollowParticleManager = new EntityFollowParticleManager();
            SaveDirectory = new DirectoryManager(author, mod);
            ModDirectory = new DirectoryManager(mod) {ContentFolder = true};
            ModsDirectory = new DirectoryManager {ContentFolder = true};
            ModsDirectory = ModsDirectory.FetchDirectoryNoParent("mods");
            ConfigDirectory = new DirectoryManager().FetchDirectoryNoParent("config").FetchDirectoryNoParent(mod);
            ContentDirectory = new DirectoryManager {ContentFolder = true};
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

        public void MessageAllPlayers(string languageCode, params object[] textParams) {
            var players = UserManager.GetPlayerEntities();

            foreach (var player in players) {
                CoreHook.ServerMainLoop.MessagePlayer(player.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }

        public void MessagePlayersByUid(IEnumerable<string> uids, string languageCode, params object[] textParams) {
            var players = UserManager.GetPlayerEntities().Where(x => uids.Contains(x.PlayerEntityLogic.Uid()));

            foreach (var player in players) {
                CoreHook.ServerMainLoop.MessagePlayer(player.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }

        public void MessagePlayersByName(IEnumerable<string> names, string languageCode, params object[] textParams) {
            var players = UserManager.GetPlayerEntities().Where(x => names.Contains(x.PlayerEntityLogic.DisplayName()));

            foreach (var player in players) {
                CoreHook.ServerMainLoop.MessagePlayer(player.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }

        public void MessagePlayersByEntity(IEnumerable<Entity> entities, string languageCode, params object[] textParams) {
            foreach (var entity in entities) {
                if (entity.PlayerEntityLogic != null) {
                    CoreHook.ServerMainLoop.MessagePlayer(entity.PlayerEntityLogic.Uid(), languageCode, textParams);
                }
            }
        }

        public void MessagePlayerByUid(string uid, string languageCode, params object[] textParams) {
            var player = UserManager.GetPlayerEntities().FirstOrDefault(x => x.PlayerEntityLogic.Uid() == uid);

            if (player != default(Entity)) {
                CoreHook.ServerMainLoop.MessagePlayer(player.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }

        public void MessagePlayerByName(string name, string languageCode, params object[] textParams) {
            var player = UserManager.GetPlayerEntities().FirstOrDefault(x => x.PlayerEntityLogic.DisplayName() == name);

            if (player != default(Entity)) {
                CoreHook.ServerMainLoop.MessagePlayer(player.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }

        public void MessagePlayerByEntity(Entity entity, string languageCode, params object[] textParams) {
            if (entity.PlayerEntityLogic != null) {
                CoreHook.ServerMainLoop.MessagePlayer(entity.PlayerEntityLogic.Uid(), languageCode, textParams);
            }
        }
    }
}
