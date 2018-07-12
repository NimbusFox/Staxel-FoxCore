namespace NimbusFox.FoxCore.Interpreters {
    /*public class CoreIntMods : IModHookV3 {
        internal static List<CoreInt> IntMods = new List<CoreInt>();
        internal static bool AllowDebug = true;
        internal static readonly Dictionary<string, CommandBuilder> Commands = new Dictionary<string, CommandBuilder>();
        private static bool ScriptsLoaded;

        public CoreIntMods() {
            new Thread(() => {
                while (ServerContext.CommandsManager == null) {

                }

                ServerContextInitializeAfter();
            }).Start(); 
        }

        public static void ServerContextInitializeAfter() {
            if (ServerContext.CommandsManager != null) {
                var commandDic = (Dictionary<string, ICommandBuilder>)ServerContext.CommandsManager.GetType()
                    .GetField("_commandBuilders", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    ?.GetValue(ServerContext.CommandsManager);

                if (commandDic != null) {
                    foreach (var command in Commands) {
                        var commandC = new ScriptCommand {
                            Kind = command.Key,
                            Usage = command.Value.Usage,
                            Public = command.Value.Public,
                            Command = (string[] array, Blob blob, ClientServerConnection server, ICommandsApi api,
                                out object[] objectArray) => {
                                var output = command.Value.Execute(array, blob, server, api);

                                objectArray = output.ResponseParams;
                                return output.TranslationCode;
                            }
                        };
                        commandDic.Add(command.Key, commandC);
                    }
                }
            }
        }

        public void Dispose() {
            foreach (var mod in IntMods) {
                mod.Dispose();
            }

            Commands.Clear();
            AllowDebug = true;
            ScriptsLoaded = false;
        }

        public void GameContextInitializeInit() {
            foreach (var mod in IntMods) {
                try {
                    mod.GameContextInitializeInit();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }

        public void GameContextInitializeBefore() {
            foreach (var mod in IntMods) {
                try {
                    mod.GameContextInitializeBefore();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }

        internal static void ReloadMods(bool reload = false) {
            foreach (var mod in IntMods) {
                mod.Dispose();
            }

            if (ServerContext.CommandsManager != null) {
                var commandDic = (Dictionary<string, ICommandBuilder>) ServerContext.CommandsManager.GetType()
                    .GetField("_commandBuilders",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(ServerContext.CommandsManager);

                if (commandDic != null) {
                    foreach (var command in Commands) {
                        commandDic.Remove(command.Key);
                    }
                }
            }

            Commands.Clear();

            IntMods.Clear();
            AllowDebug = true;
            
            var modDir = new DirectoryManager().FetchDirectory("mods");

            var debugMods = new List<string>();

            foreach (var dir in modDir.Directories.Select(dir => modDir.FetchDirectory(dir))) {
                if (dir.FileExists("fxCode.json")) {
                    var wait = true;
                    dir.ReadFile<Blob>("fxCode.json", data => {
                        var debug = false;
                        if (!data.Contains("debug")) {
                            AllowDebug = false;
                        } else {
                            debug = data.GetBool("debug", false);
                            if (debug) {
                                debugMods.Add(dir.Folder);
                            } else {
                                AllowDebug = false;
                            }
                        }

                        if (data.Contains("language")) {
                            try {
                                CoreInt currentMod;
                                switch (data.GetString("language", null)?.ToLower()) {
                                    case "js":
                                    case "javascript":
                                        currentMod = new JSInt(dir, debug);
                                        IntMods.Add(currentMod);
                                        break;
                                    case "lua":
                                        currentMod = new LuaInt(dir, debug);
                                        IntMods.Add(currentMod);
                                        break;
                                }
                            } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                                if (debug) {
                                    Application.Run(new ExceptionViewer(new Exception($"{dir.Folder} was not loaded due to the following error:\n\nParse Error: {ex.Message}", ex), dir.Folder));
                                }
                            }
                        }

                        wait = false;
                    }, true);

                    while (wait) { }
                }
            }

            if (!AllowDebug && debugMods.Any() && !reload) {
                Application.Run(new ExceptionViewer(
                    new Exception(
                        $"You are currently running non compiled scripts which can be dangerous. Please get in touch with the mod creators to get their scripts compiled.\n\nThe following mods are not compiled:\n\n{string.Join("\n", debugMods)}"),
                    "Fox Core"));
            }

            foreach (var mod in IntMods) {
                try {
                    mod.GameContextInitializeAfter();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }

            if (reload) {
                ServerContextInitializeAfter();
            }
        }

        public void GameContextInitializeAfter() {
            if (!ScriptsLoaded) {

                ReloadMods();

                ScriptsLoaded = true;
            } else {
                foreach (var mod in IntMods) {
                    try {
                        mod.GameContextInitializeAfter();
                    } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                        mod.ExceptionManager.HandleException(ex);

                        if (mod.Debug) {
                            Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                        }
                    }
                }
            }

        }
        public void GameContextDeinitialize() {
            foreach (var mod in IntMods) {
                try {
                    mod.GameContextDeinitialize();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void GameContextReloadBefore() {
            foreach (var mod in IntMods) {
                try {
                    mod.GameContextReloadBefore();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void GameContextReloadAfter() {
            foreach (var mod in IntMods) {
                try {
                    mod.GameContextReloadAfter();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            foreach (var mod in IntMods) {
                try {
                    mod.UniverseUpdateBefore(universe, step);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void UniverseUpdateAfter() {
            foreach (var mod in IntMods) {
                try {
                    mod.UniverseUpdateAfter();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            var result = true;
            foreach (var mod in IntMods) {
                try {
                    result &= mod.CanPlaceTile(entity, location, tile, accessFlags);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
            return result;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            var result = true;
            foreach (var mod in IntMods) {
                try {
                    result &= mod.CanReplaceTile(entity, location, tile, accessFlags);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
            return result;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            var result = true;
            foreach (var mod in IntMods) {
                try {
                    result &= mod.CanRemoveTile(entity, location, accessFlags);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
            return result;
        }

        public void ClientContextInitializeInit() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextInitializeInit();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void ClientContextInitializeBefore() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextInitializeBefore();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void ClientContextInitializeAfter() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextInitializeAfter();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void ClientContextDeinitialize() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextDeinitialize();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void ClientContextReloadBefore() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextReloadBefore();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void ClientContextReloadAfter() {
            foreach (var mod in IntMods) {
                try {
                    mod.ClientContextReloadAfter();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public void CleanupOldSession() {
            foreach (var mod in IntMods) {
                try {
                    mod.CleanupOldSession();
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
        }
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            var result = true;
            foreach (var mod in IntMods) {
                try {
                    result &= mod.CanInteractWithTile(entity, location, tile);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
            return result;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            var result = true;
            foreach (var mod in IntMods) {
                try {
                    result &= mod.CanInteractWithEntity(entity, lookingAtEntity);
                } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
                    mod.ExceptionManager.HandleException(ex);

                    if (mod.Debug) {
                        Application.Run(new ExceptionViewer(ex, mod.Directory.Folder));
                    }
                }
            }
            return result;
        }
    }*/
}
