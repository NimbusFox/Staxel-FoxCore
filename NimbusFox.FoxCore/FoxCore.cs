using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Events;
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
        public PatchController PatchController { get; }

        private string _author;
        private string _mod;
        private string _version;

        public bool IsLocalServer => CoreHook.ServerMainLoop != null && CoreHook.ServerMainLoop.isLocal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <param name="mod">Must match your mod directory name</param>
        /// <param name="modVersion"></param>
        public Fox_Core(string author, string mod, string modVersion, string patchControllerId = null) {
            _author = author;
            _mod = mod;
            _version = modVersion;
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
            PatchController = new PatchController(patchControllerId ?? $"{author}.{mod}");
            VersionCheck.VersionCheck.Check();
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

        public void LogError(string text) {
            Console.ForegroundColor = ConsoleColor.Red;
            Logger.WriteLine($"[Error]{_author}.{_mod}.{_version}: {text}");
            Console.ResetColor();
        }

        public void Log(string text, ConsoleColor color = default) {
            if (color != default) {
                Console.ForegroundColor = color;
            }
            Logger.WriteLine($"[Info]{_author}.{_mod}.{_version}: {text}");
            Console.ResetColor();
        }
    }
}
