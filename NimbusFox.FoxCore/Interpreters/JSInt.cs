//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
//using Jint;
//using Jint.Native;
//using Jint.Parser;
//using Jint.Runtime;
//using Jint.Runtime.Interop;
//using NimbusFox.FoxCore.Forms;
//using NimbusFox.FoxCore.Interpreters.Classes;
//using NimbusFox.FoxCore.Managers;
//using Plukit.Base;
//using Staxel.Items;
//using Staxel.Logic;
//using Staxel.Tiles;

//namespace NimbusFox.FoxCore.Interpreters {
//    internal class JSInt : CoreInt {
//        private readonly Engine _jsEngine;
//        public JSInt(DirectoryManager directory, bool debug) {
//            Debug = debug;

//            Directory = directory;

//            ExceptionManager = new ExceptionManager(directory.Folder, "null");

//            _jsEngine = new Engine(clr => {
//                clr.CatchClrExceptions(exception => {
//                    ExceptionManager.HandleException(exception);
//                    return true;
//                });

//                clr.LimitRecursion(10);
//            });

//            ParseScripts(".js.code", directory);

//            _jsEngine.SetValue("log", new Action<string>(Logger.WriteLine));
//            _jsEngine.SetValue("Fox_Core", TypeReference.CreateTypeReference(_jsEngine, typeof(Fox_Core)));
//            _jsEngine.SetValue("GameContext", new GContext());
//            _jsEngine.SetValue("ClientContext", new CContext());
//            _jsEngine.SetValue("ServerContext", new SContext());
//            _jsEngine.SetValue("RegisterCommand", new Action<string, JsValue>(AddCommand));

//            _jsEngine.Execute(script);

//            Directory._jsEngine = _jsEngine;
//        }

//        private void AddCommand(string command, JsValue obj) {
//            var location = _jsEngine.GetLastSyntaxNode().Location.Start.Line - 1;
//            try {
//                var commandBuilder = new CommandBuilder();

//                if (!obj.IsObject()) {
//                    throw new JavaScriptException(_jsEngine.Error, "2nd Argument must be an object at line " + location);
//                }

//                var dataObj = obj.AsObject();

//                if (!dataObj.HasProperty("execute")) {
//                    throw new JavaScriptException(_jsEngine.Error,
//                        "Object must have the property \"execute\" as function at line " + location);
//                }

//                if (dataObj.Get("execute").IsUndefined() || !dataObj.Get("execute").IsObject()) {
//                    throw new JavaScriptException(_jsEngine.Error, "execute must have a value of function at line " + location);
//                }

//                commandBuilder.Execute = (bits, blob, connection, api) => {
//                    var jsBits = JsValue.FromObject(_jsEngine, bits);
//                    var jsBlob = JsValue.FromObject(_jsEngine, blob);
//                    var jsConnection = JsValue.FromObject(_jsEngine, connection);
//                    var jsApi = JsValue.FromObject(_jsEngine, api);
//                    var output = dataObj.Get("execute").Invoke(jsBits, jsBlob, jsConnection, jsApi);

//                    if (!output.IsObject()) {
//                        throw new JavaScriptException(_jsEngine.Error,
//                            "execute must return an object containing \"translationCode\" and \"resonseParams\" properties at line " + location);
//                    }

//                    var cOutput = new CommandOutput();

//                    var oObj = output.AsObject();

//                    if (oObj.Get("responseParams").IsUndefined() || !oObj.Get("responseParams").IsArray()) {
//                        cOutput.ResponseParams = new object[0];
//                    } else {
//                        cOutput.ResponseParams = oObj.Get("responseParams").AsArray().GetOwnProperties()
//                            .Select(property => property.Value.Value.ToObject()).ToArray();
//                    }

//                    if (oObj.Get("translationCode").IsUndefined()) {
//                        throw new JavaScriptException(_jsEngine.Error,
//                            "execute must return an object with a \"translationCode\" at line " + location);
//                    }

//                    cOutput.TranslationCode = oObj.Get("translationCode").AsString();

//                    return cOutput;
//                };

//                if (dataObj.Get("usage").IsUndefined() || !dataObj.Get("usage").IsString()) {
//                    throw new JavaScriptException(_jsEngine.Error, "Object must have the property \"usage\" as string at line " + location);
//                }

//                commandBuilder.Usage = dataObj.Get("usage").AsString();

//                if (dataObj.Get("public").IsUndefined() || !dataObj.Get("public").IsBoolean()) {
                    
//                    throw new JavaScriptException(_jsEngine.Error, "Object must have the property \"public\" as bool at line " + location);
//                }

//                commandBuilder.Public = dataObj.Get("public").AsBoolean();

//                AddCommand(command, commandBuilder);
//            } catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached) {
//                ExceptionManager.HandleException(ex);

//                if (Debug) {
//                    Application.Run(new ExceptionViewer(ex, Directory.Folder));
//                }
//            }
//        }

//        public override void GameContextInitializeInit() {
//            var function = _jsEngine.GetValue("gameContextInitializeInit");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void GameContextInitializeBefore() {
//            var function = _jsEngine.GetValue("gameContextInitializeBefore");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void GameContextInitializeAfter() {
//            var function = _jsEngine.GetValue("gameContextInitializeAfter");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void GameContextDeinitialize() {
//            var function = _jsEngine.GetValue("gameContextDeinitialize");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void GameContextReloadBefore() {
//            var function = _jsEngine.GetValue("gameContextReloadBefore");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void GameContextReloadAfter() {
//            var function = _jsEngine.GetValue("gameContextReloadAfter");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void UniverseUpdateBefore(Universe universe, Timestep step) {
//            var function = _jsEngine.GetValue("universeUpdateBefore");

//            if (!function.IsUndefined()) {
//                function.Invoke(JsValue.FromObject(_jsEngine, universe), JsValue.FromObject(_jsEngine, step));
//            }
//        }

//        public override void UniverseUpdateAfter() {
//            var function = _jsEngine.GetValue("universeUpdateAfter");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            var function = _jsEngine.GetValue("canPlaceTile");

//            if (!function.IsUndefined()) {
//                var result = function.Invoke(
//                    JsValue.FromObject(_jsEngine, entity),
//                    JsValue.FromObject(_jsEngine, location), 
//                    JsValue.FromObject(_jsEngine, tile),
//                    JsValue.FromObject(_jsEngine, accessFlags));

//                if (result.IsBoolean()) {
//                    return result.AsBoolean();
//                }
//            }
//            return true;
//        }

//        public override bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            var function = _jsEngine.GetValue("canReplaceTile");

//            if (!function.IsUndefined()) {
//                var result = function.Invoke(
//                    JsValue.FromObject(_jsEngine, entity),
//                    JsValue.FromObject(_jsEngine, location),
//                    JsValue.FromObject(_jsEngine, tile),
//                    JsValue.FromObject(_jsEngine, accessFlags));

//                if (result.IsBoolean()) {
//                    return result.AsBoolean();
//                }
//            }
//            return true;
//        }

//        public override bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
//            var function = _jsEngine.GetValue("canRemoveTile");

//            if (!function.IsUndefined()) {
//                var result = function.Invoke(
//                    JsValue.FromObject(_jsEngine, entity),
//                    JsValue.FromObject(_jsEngine, location),
//                    JsValue.FromObject(_jsEngine, accessFlags));

//                if (result.IsBoolean()) {
//                    return result.AsBoolean();
//                }
//            }
//            return true;
//        }

//        public override void ClientContextInitializeInit() {
//            var function = _jsEngine.GetValue("clientContextInitializeInit");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void ClientContextInitializeBefore() {
//            var function = _jsEngine.GetValue("clientContextInitializeBefore");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void ClientContextInitializeAfter() {
//            var function = _jsEngine.GetValue("clientContextInitializeAfter");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void ClientContextDeinitialize() {
//            var function = _jsEngine.GetValue("clientContextDeinitialize");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void ClientContextReloadBefore() {
//            var function = _jsEngine.GetValue("clientContextReloadBefore");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void ClientContextReloadAfter() {
//            var function = _jsEngine.GetValue("clientContextReloadAfter");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override void CleanupOldSession() {
//            var function = _jsEngine.GetValue("cleanupOldSession");

//            if (!function.IsUndefined()) {
//                function.Invoke();
//            }
//        }

//        public override bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
//            var function = _jsEngine.GetValue("canInteractWithTile");

//            if (!function.IsUndefined()) {
//                var result = function.Invoke(
//                    JsValue.FromObject(_jsEngine, entity),
//                    JsValue.FromObject(_jsEngine, location),
//                    JsValue.FromObject(_jsEngine, tile));

//                if (result.IsBoolean()) {
//                    return result.AsBoolean();
//                }
//            }
//            return true;
//        }

//        public override bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
//            var function = _jsEngine.GetValue("canInteractWithEntity");

//            if (!function.IsUndefined()) {
//                var result = function.Invoke(
//                    JsValue.FromObject(_jsEngine, entity),
//                    JsValue.FromObject(_jsEngine, lookingAtEntity));

//                if (result.IsBoolean()) {
//                    return result.AsBoolean();
//                }
//            }
//            return true;
//        }
//    }
//}
