//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
//using Neo.IronLua;
//using NimbusFox.FoxCore.Forms;
//using NimbusFox.FoxCore.Interpreters.Classes;
//using NimbusFox.FoxCore.Managers;
//using Plukit.Base;
//using Staxel.Commands;
//using Staxel.Items;
//using Staxel.Logic;
//using Staxel.Server;
//using Staxel.Tiles;

//namespace NimbusFox.FoxCore.Interpreters {
//    internal class LuaInt : CoreInt {
//        private readonly LuaGlobalPortable _luaEngine;

//        public LuaInt(DirectoryManager directory, bool debug) {
//            Debug = debug;
//            Directory = directory;

//            ExceptionManager = new ExceptionManager(directory.Folder, "null");

//            ParseScripts(".lua.code", directory);

//            _luaEngine = new Lua().CreateEnvironment();

//            Directory._luaEngine = _luaEngine;

//            _luaEngine["log"] = new Action<string>(Logger.WriteLine);
//            _luaEngine["Fox_Core"] =
//                new Func<string, string, string, Fox_Core>((author, mod, version) =>
//                    new Fox_Core(author, mod, version));

//            _luaEngine["GameContext"] = new GContext();
//            _luaEngine["ClientContext"] = new CContext();
//            _luaEngine["ServerContext"] = new SContext();
//            _luaEngine["RegisterCommand"] = new Action<string, LuaTable>(AddCommand);

//            _luaEngine.DoChunk(script, $"{directory.Folder}.lua");

//        }

//        private void AddCommand(string command, LuaTable obj) {
//            try {
//                var commandBuilder = new CommandBuilder();

//                if (obj["execute"] == null) {
//                    throw new LuaRuntimeException("Object must contain a \"execute\" member", null);
//                }

//                commandBuilder.Execute = (bits, blob, connection, api) => {
//                    var bitTable = new LuaTable();

//                    foreach (var bit in bits) {
//                        bitTable.Add(bit);
//                    }

//                    object[] result;
//                    try {
//                        result = obj.CallMember("execute", bitTable, blob, connection, api).Values;
//                    } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
//                        throw new LuaRuntimeException("\"execute\" must be a function", ex);
//                    }

//                    if (!result.Any() || result[0] is LuaTable == false) {
//                        throw new LuaRuntimeException(
//                            "Expected an object containing \"translationCode\" and \"responseParams\" members", null);
//                    }

//                    var output = new CommandOutput();

//                    var resultTable = (LuaTable)result[0];

//                    if (resultTable["responseParams"] == null) {
//                        output.ResponseParams = new object[0];
//                    } else {
//                        var array = resultTable["responseParams"];

//                        if (array is LuaTable == false) {
//                            throw new LuaRuntimeException("Expecting member \"responseParams\" as an array", null);
//                        }

//                        output.ResponseParams = ((LuaTable)array).ArrayList.ToArray();
//                    }

//                    if (resultTable["translationCode"] == null || resultTable["translationCode"] is string == false) {
//                        throw new LuaRuntimeException("Object must contain a \"translationCode\" member as string",
//                            null);
//                    }

//                    output.TranslationCode = (string)resultTable["translationCode"];

//                    return output;
//                };

//                if (obj["usage"] == null || obj["usage"] is string == false) {
//                    throw new LuaRuntimeException("Object must have the member \"usage\" as string", null);
//                }

//                commandBuilder.Usage = (string)obj["usage"];

//                if (obj["public"] == null || obj["public"] is bool == false) {
//                    throw new LuaRuntimeException("Object must have the member \"public\" as bool", null);
//                }

//                commandBuilder.Public = (bool)obj["public"];

//                AddCommand(command, commandBuilder);
//            } catch (LuaRuntimeException ex) when (!System.Diagnostics.Debugger.IsAttached) {
//                var exce = LuaExceptionData.GetData(ex);

//                var exe = new Exception(exce.StackTrace, ex);

//                ExceptionManager.HandleException(exe);

//                if (Debug) {
//                    Application.Run(new ExceptionViewer(exe, Directory.Folder));
//                }
//            } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
//                ExceptionManager.HandleException(ex);

//                if (Debug) {
//                    Application.Run(new ExceptionViewer(ex, Directory.Folder));
//                }
//            }
//        }

//        public override void GameContextInitializeInit() {
//            if (_luaEngine["gameContextInitializeInit"] != null &&
//                _luaEngine["gameContextInitializeInit"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void GameContextInitializeBefore() {
//            if (_luaEngine["gameContextInitializeBefore"] != null &&
//                _luaEngine["gameContextInitializeBefore"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void GameContextInitializeAfter() {
//            if (_luaEngine["gameContextInitializeAfter"] != null &&
//                _luaEngine["gameContextInitializeAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void GameContextDeinitialize() {
//            if (_luaEngine["gameContextDeinitialize"] != null &&
//                _luaEngine["gameContextDeinitialize"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void GameContextReloadBefore() {
//            if (_luaEngine["gameContextReloadBefore"] != null &&
//                _luaEngine["gameContextReloadBefore"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void GameContextReloadAfter() {
//            if (_luaEngine["gameContextReloadAfter"] != null &&
//                _luaEngine["gameContextReloadAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void UniverseUpdateBefore(Universe universe, Timestep step) {
//            if (_luaEngine["universeUpdateBefore"] != null &&
//                _luaEngine["universeUpdateBefore"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void UniverseUpdateAfter() {
//            if (_luaEngine["universeUpdateAfter"] != null &&
//                _luaEngine["universeUpdateAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            if (_luaEngine["canPlaceTile"] != null &&
//                _luaEngine["canPlaceTile"] is LuaMethod f) {
//                var result = _luaEngine.CallMember(f.Name, entity, location, tile, accessFlags).Values.FirstOrDefault();

//                if (result == null || result is bool == false) {
//                    throw new LuaRuntimeException("Was expecting a boolean as the first result", null);
//                }

//                return (bool)result;
//            }

//            return true;
//        }

//        public override bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            if (_luaEngine["canReplaceTile"] != null &&
//                _luaEngine["canReplaceTile"] is LuaMethod f) {
//                var result = _luaEngine.CallMember(f.Name, entity, location, tile, accessFlags).Values.FirstOrDefault();

//                if (result == null || result is bool == false) {
//                    throw new LuaRuntimeException("Was expecting a boolean as the first result", null);
//                }

//                return (bool)result;
//            }

//            return true;
//        }

//        public override bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
//            if (_luaEngine["canRemoveTile"] != null &&
//                _luaEngine["canRemoveTile"] is LuaMethod f) {
//                var result = _luaEngine.CallMember(f.Name, entity, location, accessFlags).Values.FirstOrDefault();

//                if (result == null || result is bool == false) {
//                    throw new LuaRuntimeException("Was expecting a boolean as the first result", null);
//                }

//                return (bool)result;
//            }

//            return true;
//        }

//        public override void ClientContextInitializeInit() {
//            if (_luaEngine["gameContextInitializeInit"] != null &&
//                _luaEngine["gameContextInitializeInit"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void ClientContextInitializeBefore() {
//            if (_luaEngine["gameContextInitializeInit"] != null &&
//                _luaEngine["gameContextInitializeInit"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void ClientContextInitializeAfter() {
//            if (_luaEngine["clientContextInitializeAfter"] != null &&
//                _luaEngine["clientContextInitializeAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void ClientContextDeinitialize() {
//            if (_luaEngine["clientContextDeinitialize"] != null &&
//                _luaEngine["clientContextDeinitialize"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void ClientContextReloadBefore() {
//            if (_luaEngine["clientContextReloadBefore"] != null &&
//                _luaEngine["clientContextReloadBefore"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void ClientContextReloadAfter() {
//            if (_luaEngine["clientContextReloadAfter"] != null &&
//                _luaEngine["clientContextReloadAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override void CleanupOldSession() {
//            if (_luaEngine["clientContextReloadAfter"] != null &&
//                _luaEngine["clientContextReloadAfter"] is LuaMethod f) {
//                _luaEngine.CallMember(f.Name);
//            }
//        }

//        public override bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
//            if (_luaEngine["canInteractWithTile"] != null &&
//                _luaEngine["canInteractWithTile"] is LuaMethod f) {
//                var result = _luaEngine.CallMember(f.Name, entity, location, tile).Values.FirstOrDefault();

//                if (result == null || result is bool == false) {
//                    throw new LuaRuntimeException("Was expecting a boolean as the first result", null);
//                }

//                return (bool)result;
//            }
//            return true;
//        }

//        public override bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
//            if (_luaEngine["canInteractWithEntity"] != null &&
//                _luaEngine["canInteractWithEntity"] is LuaMethod f) {
//                var result = _luaEngine.CallMember(f.Name, entity, lookingAtEntity).Values.FirstOrDefault();

//                if (result == null || result is bool == false) {
//                    throw new LuaRuntimeException("Was expecting a boolean as the first result", null);
//                }

//                return (bool)result;
//            }
//            return true;
//        }
//    }
//}
