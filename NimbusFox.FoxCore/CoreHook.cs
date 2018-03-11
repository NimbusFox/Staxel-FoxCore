﻿using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Client.Staxel.Builders;
using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using NimbusFox.FoxCore.VersionCheck;
using Plukit.Base;
using Staxel.Collections;
using Staxel.Effects;
using Staxel.FoxCore.Managers;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox {
    internal class CoreHook : IModHookV2 {

        internal static UserManager UserManager;
        internal static Universe Universe;
        internal static TileManager TileManager;
        private static long CacheTick;

        public void Dispose() {
            TileManager = null;
            CacheTick = 0;
        }

        public void GameContextInitializeInit() {
            UserManager = new UserManager();
        }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            Universe = universe;
            if (TileManager == null) {
                TileManager = new TileManager();
            }

            if (CacheTick <= DateTime.Now.Ticks) {
                foreach (var player in UserManager.GetPlayerEntities()) {
                    UserManager.AddUpdateEntry(player.PlayerEntityLogic.Uid(), player.PlayerEntityLogic.DisplayName());
                }
                UserManager.CacheCheck();
                CacheTick = DateTime.Now.AddSeconds(30).Ticks;
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
