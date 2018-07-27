using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Events;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Server;
using WorldManager = NimbusFox.FoxCore.Managers.WorldManager;

namespace NimbusFox.FoxCore {
    public class Fox_Core {
        public ExceptionManager ExceptionManager { get; }
        public WorldManager WorldManager { get; }
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public UserManager UserManager => CoreHook.UserManager;
        public DirectoryManager SaveDirectory { get; }
        public DirectoryManager ModDirectory { get; }
        public DirectoryManager ModsDirectory { get; }
        public DirectoryManager ConfigDirectory { get; }
        public DirectoryManager ContentDirectory { get; }
        public ServerMainLoop ServerMainLoop => CoreHook.ServerMainLoop;

        public PatchController PatchController => _patchController ??
                                                  (_patchController = new PatchController(_patchControllerId));

        private PatchController _patchController;
        private readonly string _patchControllerId;

        private readonly string _author;
        private readonly string _mod;
        private readonly string _version;

        private readonly List<string> _reportingMods = new List<string>();

        public bool IsLocalServer => CoreHook.ServerMainLoop != null && CoreHook.ServerMainLoop.isLocal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <param name="mod">Must match your mod directory name</param>
        /// <param name="modVersion"></param>
        /// <param name="errorEmail">Only add an email if you are fine with the email being public. Be sure to whitelist foxcore@nimbusfox.uk</param>
        public Fox_Core(string author, string mod, string modVersion, string errorEmail = null) {
            _author = author;
            _mod = mod;
            _version = modVersion;
            ExceptionManager = new ExceptionManager(author, mod, modVersion, errorEmail);
            WorldManager = new WorldManager();
            SaveDirectory = new DirectoryManager(author, mod);
            ModDirectory = new DirectoryManager(mod) {ContentFolder = true};
            ModsDirectory = new DirectoryManager {ContentFolder = true};
            ModsDirectory = ModsDirectory.FetchDirectoryNoParent("mods");
            ConfigDirectory = new DirectoryManager().FetchDirectoryNoParent("config").FetchDirectoryNoParent(mod);
            ContentDirectory = new DirectoryManager {ContentFolder = true};
            _patchControllerId = $"{_author}.{_mod}";
            VersionCheck.VersionCheck.Check();

            if (errorEmail != null) {
                if (CoreHook.FxCore == null) {
                    _reportingMods.Add($"{author}.{mod}.{modVersion}");
                    return;
                }

                ProcessReportingMod($"{author}.{mod}.{modVersion}");
            }
        }

        internal void ProcessReportingMods() {
            foreach (var mod in _reportingMods) {
                ProcessReportingMod(mod);
            }
        }

        internal void ProcessReportingMod(string mod) {
            var report = false;
            var message =
                $"{mod} would like the right to submit crash errors to the mod developer's email. Only errors and the data that was given by the mod developer will be sent to their email. Please note this can contain data that I (NimbusFox) cannot maintain nor guarantee to not be identifiable. Please be wary of this as you decide if the mod can send error logs.";
            var answer =
                $"Allow {mod} to report errors?: Y/N (Even if N is selected. Errors can be found in content/modErrors)";
            var windowAnswer = $"Allow {mod} to report errors? (Even if No is selected. Errors can be found in content/modErrors)";
            if (!CoreHook.FxCore.ConfigDirectory.FetchDirectory("errorReports").FileExists($"{mod}.config")) {
                try {
                    var messageBox =
                        MessageBox.Show(
                            $@"{message}{Environment.NewLine}{Environment.NewLine}{windowAnswer}",
                            @"Allow mod error reports", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2);

                    report = messageBox == DialogResult.Yes;
                } catch {
                    Console.WriteLine();
                    CoreHook.FxCore.Log(message, ConsoleColor.Yellow);
                    Console.WriteLine();
                    CoreHook.FxCore.Log(answer, ConsoleColor.Yellow);

                    var input = "";

                    while (input?.ToLower() != "x" && input?.ToLower() != "y") {
                        input = Console.ReadLine();

                        if (input?.ToLower() != "x" && input?.ToLower() != "y") {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(@"Please put either x or y");
                            Console.ResetColor();
                        }
                    }

                    report = input.ToLower() == "y";
                }
            } else {
                report = CoreHook.FxCore.ConfigDirectory.FetchDirectory("errorReports")
                    .ReadFile<Blob>($"{mod}.config", true).GetBool("report", false);
            }

            var blob = BlobAllocator.Blob(true);
            blob.SetBool("report", report);
            CoreHook.FxCore.ConfigDirectory.FetchDirectory("errorReports").WriteFile($"{mod}.config", blob, true, true);
            Blob.Deallocate(ref blob);
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
