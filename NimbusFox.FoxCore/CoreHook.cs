using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore.Client.Staxel.Builders;
using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using NimbusFox.FoxCore.VersionCheck;
using Plukit.Base;
using Staxel;
using Staxel.Collections;
using Staxel.Core;
using Staxel.Effects;
using Staxel.FoxCore.Forms;
using Staxel.FoxCore.Managers;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox {
    public class CoreHook : IModHookV2 {

        internal static UserManager UserManager;
        public static Universe Universe;
        private static long _cacheTick;

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
        }
        public void UniverseUpdateAfter() { }
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
