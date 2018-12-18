using System.Collections;
using System.Collections.Generic;
using NimbusFox.FoxCore.V3.Interfaces;
using NimbusFox.FoxCore.V3.Managers;
using NimbusFox.FoxCore.V3.Patches;
using Plukit.Base;
using Staxel;
using Staxel.EntityStorage;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Server;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.V3.Classes {
    internal class CoreHook : IFoxModHook {

        internal static UserManager UserManager;
        internal static Universe Universe;
        internal static ServerMainLoop ServerMainLoop;
        internal static FoxCore FxCore;
        internal static Entity _settingEntity;

        internal static void AfterLoad(PlayerEntityLogic __instance) {
            if (__instance == null) {
                return;
            }
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
                    var blob = BlobAllocator.Blob(true);
                    blob.AssignFrom(__instance.PlayerEntity.Blob);
                    mod.OnPlayerLoadAfter(blob);
                    Blob.Deallocate(ref blob);
                }
            }
        }

        public CoreHook() {
            if (FxCore == null) {
                FxCore = new FoxCore("NimbusFox", "FoxCore", "V3");

                FxCore.PatchController.Add(typeof(PlayerEntityLogic), "PersistOtherPlayerData", typeof(CoreHook), nameof(BeforeSave), typeof(CoreHook), nameof(AfterSave));
                FxCore.PatchController.Add(typeof(PlayerEntityLogic), "Construct", null, null, typeof(CoreHook), nameof(AfterLoad));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveAllPlayerDataOnConnect", null, null, typeof(CoreHook), nameof(OnConnect));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveDisconnectingPlayer", null, null, typeof(CoreHook), nameof(OnDisconnect));
                ClientServerConnectionPatches.InitPatches();
                ChatControllerPatches.InitPatches();
            }
        }

        internal static void BeforeSave(PlayerEntityLogic __instance, Blob __state) {
            __state = BlobAllocator.Blob(true);
            if (__instance == null) {
                return;
            }
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
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
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
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
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
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
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
                    mod.OnPlayerDisconnect(entity);
                }
            }

            Disconnects.Remove(entity);
        }

        public void Dispose() {
            _settingEntity?.Dispose();
            _settingEntity = null;
        }

        public void GameContextInitializeInit() {
        }
        public void GameContextInitializeBefore() {

        }

        public void GameContextInitializeAfter() {
            
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }

        public void GameContextReloadAfter() {
        }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            Universe = universe;
            if (universe.Server) {
                if (UserManager == null) {
                    UserManager = new UserManager();
                }

                if (ServerMainLoop == null) {
                    ServerMainLoop =
                        ServerContext.VillageDirector?.UniverseFacade?
                            .GetPrivateFieldValue<ServerMainLoop>("_serverMainLoop");
                }
            }
        }

        public void UniverseUpdateAfter() {
            if (Universe.Server) {
                var blob = BlobAllocator.Blob(true);
                var storeBlob = blob.FetchBlob("store");
                var hasVal = false;
                foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                    if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
                        if (!storeBlob.Contains(mod.GetType().FullName)) {
                            var current = storeBlob.FetchBlob(mod.GetType().FullName);
                            mod.Store(current);
                            var val = current?.IsEmpty();
                            if (val == null) {
                                storeBlob.Delete(mod.GetType().FullName);
                                continue;
                            }

                            if (val.Value) {
                                storeBlob.Delete(mod.GetType().FullName);
                                continue;
                            }

                            hasVal = true;
                        }
                    }
                }

                if (hasVal) {
                    FxCore.MessageAllPlayers(blob.ToString());
                }

                Blob.Deallocate(ref blob);
            }
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

        }
        public void OnPlayerDisconnect(Entity entity) { }
        public void Store(Blob blob) { }
        public void Restore(Blob blob) { }

        public static void StartRestore(Blob blob) {
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHook mod) {
                    if (blob.Contains(mod.GetType().FullName)) {
                        mod.Restore(blob.GetBlob(mod.GetType().FullName));
                    }
                }
            }
        }
    }
}
