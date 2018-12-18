using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using NimbusFox.FoxCore.Forms;
using NimbusFox.FoxCore.Managers;
using NimbusFox.FoxCore.Patches;
using Staxel;
using Staxel.Client;
using Staxel.EntityStorage;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Server;
using Staxel.Tiles;
using Blob = Plukit.Base.Blob;

namespace NimbusFox {
    internal class CoreHook : IFoxModHookV3 {

        internal static UserManager UserManager;
        internal static Universe Universe;
        internal static ServerMainLoop ServerMainLoop;
        internal static Fox_Core FxCore;
        private static long _cacheTick;
        internal static Entity _settingEntity;

        internal static void AfterLoad(PlayerEntityLogic __instance) {
            if (__instance == null) {
                return;
            }
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    var blob = BlobAllocator.Blob(true);
                    blob.AssignFrom(__instance.PlayerEntity.Blob);
                    mod.OnPlayerLoadAfter(blob);
                    Blob.Deallocate(ref blob);
                }
            }
        }

        internal static void BeforeSave(PlayerEntityLogic __instance, Blob __state) {
            __state = BlobAllocator.Blob(true);
            if (__instance == null) {
                return;
            }
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerSaveBefore(__instance, out var blob);
                    if (blob != null) {
                        __state.MergeFrom(blob);
                    }
                }
            }

        }

        internal static void AfterSave(PlayerEntityLogic __instance, Blob __state) {
            if (__instance == null) {
                return;
            }
            __state.FetchBlob("collections").MergeFrom(__instance.PlayerEntity.Blob.GetBlob("collections"));
            __state.SetBool("dontStare", __instance.PlayerEntity.GetPrivateFieldValue<bool>("NPCsDontStare"));
            __state.SetLong("theftCount", __instance.PlayerEntity.GetPrivateFieldValue<long>("_theftCount"));
            __state.SetLong("theftResetDay", __instance.PlayerEntity.GetPrivateFieldValue<long>("_theftResetDay"));
            __state.SetBool("hasSoldToMerchant", __instance.PlayerEntity.GetPrivateFieldValue<bool>("HasSoldToMerchant"));

            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerSaveAfter(__instance, out var blob);
                    if (blob != null) {
                        __state.MergeFrom(blob);
                    }
                }
            }

            ServerContext.EntityBlobDatabase.Set(__instance.Uid(), __instance.DisplayName(), EntityStorageKey.OtherPlayerData, __state);
            Blob.Deallocate(ref __state);
        }

        internal static void OnConnect(Entity entity) {
            if (entity == null) {
                return;
            }
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerConnect(entity);
                }
            }
        }

        private static readonly List<Entity> Disconnects = new List<Entity>();

        internal static void OnDisconnect(Entity entity) {
            if (entity == null) {
                return;
            }
            if (!Disconnects.Contains(entity)) {
                Disconnects.Add(entity);
                return;
            }

            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerDisconnect(entity);
                }
            }

            Disconnects.Remove(entity);
        }

        public void Dispose() {
            _cacheTick = 0;
            _settingEntity?.Dispose();
            _settingEntity = null;
        }

        public void GameContextInitializeInit() {
        }
        public void GameContextInitializeBefore() {
            
        }

        public void GameContextInitializeAfter() {
            if (Process.GetCurrentProcess().ProcessName.Contains("ContentBuilder") && !Process.GetCurrentProcess().StartInfo.Arguments.Contains("--stopVariantCheck")) {
                var variantLoader = new VariantLoader();

                Application.Run(variantLoader);

                variantLoader.Dispose();
            }

            if (FxCore == null) {
                FxCore = new Fox_Core("NimbusFox", "FoxCore", "V2.1");

                //FxCore.PatchController.Add(typeof(PlayerEntityLogic), "PersistOtherPlayerData", typeof(CoreHook), nameof(BeforeSave), typeof(CoreHook), nameof(AfterSave));
                //FxCore.PatchController.Add(typeof(PlayerEntityLogic), "Construct", null, null, typeof(CoreHook), nameof(AfterLoad));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveAllPlayerDataOnConnect", null, null, typeof(CoreHook), nameof(OnConnect));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveDisconnectingPlayer", null, null, typeof(CoreHook), nameof(OnDisconnect));
                FxCore.PatchController.Add(typeof(ChatController), "ReceiveConsoleResponse",
                    typeof(ChatControllerPatches), nameof(ChatControllerPatches.ReceiveConsoleResponse));
                
                UserManager = new UserManager();
            }
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }

        public void GameContextReloadAfter() {
        }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            Universe = universe;
            if (universe.Server) {
                if (_cacheTick <= DateTime.Now.Ticks) {
                    foreach (var player in UserManager.GetPlayerEntities()) {
                        UserManager.AddUpdateEntry(player.PlayerEntityLogic.Uid(), player.PlayerEntityLogic.DisplayName());
                    }
                    _cacheTick = DateTime.Now.AddSeconds(30).Ticks;
                }

                if (ServerMainLoop == null) {
                    ServerMainLoop =
                        ServerContext.VillageDirector?.UniverseFacade?
                            .GetPrivateFieldValue<ServerMainLoop>("_serverMainLoop");
                }

                if (SettingsManager.UpdateList.Count > 0) {
                    var blob = BlobAllocator.Blob(true);

                    var settings = blob.FetchBlob("settings");

                    foreach (var item in SettingsManager.UpdateList) {
                        settings.FetchBlob(item).MergeFrom(SettingsManager.ModsSettings[item]);
                    }

                    using (var ms = new MemoryStream()) {
                        blob.Write(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        FxCore.MessageAllPlayers(blob.ToString());
                    }

                    Blob.Deallocate(ref blob);

                    SettingsManager.UpdateList.Clear();
                }
            }
        }

        public void UniverseUpdateAfter() {
        }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return true;
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }

        public void CleanupOldSession() {
            _settingEntity?.Dispose();
            _settingEntity = null;
        }

        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            return true;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            return true;
        }

        public void OnPlayerLoadAfter(Blob blob) { }
        public void OnPlayerSaveBefore(PlayerEntityLogic logic, out Blob saveBlob) {
            saveBlob = null;
        }

        public void OnPlayerSaveAfter(PlayerEntityLogic logic, out Blob saveBlob) {
            saveBlob = null;
        }

        public void OnPlayerConnect(Entity entity) {
            var blob = BlobAllocator.Blob(true);

            var settings = blob.FetchBlob("settings");

            foreach (var setting in SettingsManager.ModsSettings) {
                settings.FetchBlob(setting.Key).MergeFrom(setting.Value);
            }

            if (settings.KeyValueIteratable.Count > 0) {
                FxCore.MessagePlayerByEntity(entity, blob.ToString());
            }

            Blob.Deallocate(ref blob);
        }
        public void OnPlayerDisconnect(Entity entity) { }
    }
}
