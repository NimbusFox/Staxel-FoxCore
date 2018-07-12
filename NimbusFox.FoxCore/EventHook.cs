using System;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.FoxCore.Events;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.FoxCore {
    internal class EventHook : IModHookV2 {

        public EventHook() {
            Controller.Add(typeof(ModdingController), "UniverseUpdateAfter", null, null, this, "ShowMessageAfterUpdate");
        }
        
        public static void ShowMessageAfterUpdate() {
            Logger.WriteLine("Test complete");
            Console.WriteLine(@"Testing#################################################");
        }

        public void Dispose() { }
        public void GameContextInitializeInit() { }

        public void GameContextInitializeBefore() {
        }
        public void GameContextInitializeAfter() { }

        public void GameContextDeinitialize() {
            
        }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }
        public void UniverseUpdateBefore(Universe universe, Timestep step) { }
        
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
