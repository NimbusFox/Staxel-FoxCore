using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Events;
using Plukit.Base;
using NimbusFox.FoxCore.Forms;
using NimbusFox.FoxCore.Managers;
using Staxel;
using Staxel.Behavior;
using Staxel.EntityStorage;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Player;
using Staxel.Server;
using Staxel.Tiles;
using Blob = Plukit.Base.Blob;

namespace NimbusFox {
    internal class CoreHook : IModHookV2 {

        internal static UserManager UserManager;
        internal static Universe Universe;
        internal static ServerMainLoop ServerMainLoop;
        internal static Fox_Core FxCore;
        private static long _cacheTick;

        internal static void AfterLoad(PlayerEntityLogic __instance) {
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
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerSaveBefore(__instance, out var blob);
                    __state.MergeFrom(blob);
                }
            }
        }

        internal static void AfterSave(PlayerEntityLogic __instance, Blob __state) {
            __state.FetchBlob("collections").MergeFrom(__instance.PlayerEntity.Blob.GetBlob("collections"));
            __state.SetBool("dontStare", __instance.PlayerEntity.GetPrivateFieldValue<bool>("NPCsDontStare"));
            __state.SetLong("theftCount", __instance.PlayerEntity.GetPrivateFieldValue<long>("_theftCount"));
            __state.SetLong("theftResetDay", __instance.PlayerEntity.GetPrivateFieldValue<long>("_theftResetDay"));
            __state.SetBool("hasSoldToMerchant", __instance.PlayerEntity.GetPrivateFieldValue<bool>("HasSoldToMerchant"));

            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerSaveAfter(__instance, out var blob);
                    __state.MergeFrom(blob);
                }
            }

            ServerContext.EntityBlobDatabase.Set(__instance.Uid(), __instance.DisplayName(), EntityStorageKey.OtherPlayerData, __state);
            Blob.Deallocate(ref __state);
        }

        internal static void OnConnect(Entity entity) {
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerConnect(entity);
                }
            }
        }

        internal static void OnDisconnect(Entity entity) {
            foreach (var modInstance in GameContext.ModdingController.GetPrivateFieldValue<IEnumerable>("_modHooks")) {
                if (modInstance.GetPrivateFieldValue<object>("_instance") is IFoxModHookV3 mod) {
                    mod.OnPlayerDisconnect(entity);
                }
            }
        }

        public void Dispose() {
            _cacheTick = 0;
        }

        public void GameContextInitializeInit() {
            UserManager = new UserManager();
        }
        public void GameContextInitializeBefore() { }

        public void GameContextInitializeAfter() {
            if (Process.GetCurrentProcess().ProcessName.Contains("ContentBuilder")) {
                var variantLoader = new VariantLoader();

                Application.Run(variantLoader);

                variantLoader.Dispose();
            }

            if (FxCore == null) {
                FxCore = new Fox_Core("NimbusFox", "FoxCore", "V2.1", "nimbusfox.foxcore@nimbusfox.uk");

                FxCore.PatchController.Add(typeof(PlayerEntityLogic), "PersistOtherPlayerData", typeof(CoreHook), nameof(BeforeSave), typeof(CoreHook), nameof(AfterSave));
                FxCore.PatchController.Add(typeof(PlayerEntityLogic), "Construct", null, null, typeof(CoreHook), nameof(AfterLoad));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveAllPlayerDataOnConnect", null, null, typeof(CoreHook), nameof(OnConnect));
                FxCore.PatchController.Add(typeof(PlayerPersistence), "SaveDisconnectingPlayer", null, null, typeof(CoreHook), nameof(OnDisconnect));

                FxCore.ProcessReportingMods();

                FxCore.ExceptionManager.HandleException(new Exception("Test exception", new Exception("Test innerException")), new Dictionary<string, object> { { "test", true } });
            }
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }

        public void GameContextReloadAfter() {
        }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            Universe = universe;

            if (_cacheTick <= DateTime.Now.Ticks) {
                foreach (var player in UserManager.GetPlayerEntities()) {
                    UserManager.AddUpdateEntry(player.PlayerEntityLogic.Uid(), player.PlayerEntityLogic.DisplayName());
                }
                UserManager.CacheCheck();
                _cacheTick = DateTime.Now.AddSeconds(30).Ticks;
            }

            if (ServerMainLoop == null) {
                ServerMainLoop = (ServerMainLoop)typeof(DirectorFacade).GetField("_serverMainLoop",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.GetValue(ServerContext.VillageDirector.UniverseFacade);
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
        public void CleanupOldSession() { }
    }
}
