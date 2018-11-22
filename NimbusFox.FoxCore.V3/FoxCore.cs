using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.V3.Classes;
using NimbusFox.FoxCore.V3.Managers;
using NimbusFox.FoxCore.V3.Patches;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Server;
using WorldManager = NimbusFox.FoxCore.V3.Managers.WorldManager;

namespace NimbusFox.FoxCore.V3 {
    public class FoxCore {
        public ExceptionManager ExceptionManager { get; }
        public WorldManager WorldManager { get; }
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public UserManager UserManager => CoreHook.UserManager;
        public DirectoryManager SaveDirectory { get; }
        public DirectoryManager ModDirectory { get; }
        public DirectoryManager ModsDirectory { get; }
        public DirectoryManager ConfigDirectory { get; }
        public DirectoryManager ContentDirectory { get; }
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public ServerMainLoop ServerMainLoop => CoreHook.ServerMainLoop;

        public PatchController PatchController => _patchController ??
                                                  (_patchController = new PatchController(_patchControllerId));

        private PatchController _patchController;
        private readonly string _patchControllerId;

        private readonly string _author;
        private readonly string _mod;
        private readonly string _version;

        public bool IsLocalServer => CoreHook.ServerMainLoop != null && CoreHook.ServerMainLoop.isLocal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <param name="mod">Must match your mod directory name</param>
        /// <param name="modVersion"></param>
        public FoxCore(string author, string mod, string modVersion) {
            _author = author;
            _mod = mod;
            _version = modVersion;
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            WorldManager = new WorldManager();
            SaveDirectory = new DirectoryManager(author, mod).FetchDirectoryNoParent(modVersion);
            ModDirectory = new DirectoryManager(mod) { ContentFolder = true };
            ModsDirectory = new DirectoryManager { ContentFolder = true }.FetchDirectoryNoParent("content");
            ModsDirectory = ModsDirectory.FetchDirectoryNoParent("mods");
            ConfigDirectory = new DirectoryManager().FetchDirectoryNoParent("modConfigs").FetchDirectoryNoParent(mod).FetchDirectoryNoParent(modVersion);
            ContentDirectory = new DirectoryManager { ContentFolder = true }.FetchDirectoryNoParent("content");
            _patchControllerId = $"{_author}.{_mod}";
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
