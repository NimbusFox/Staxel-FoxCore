using System;
using System.IO;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.TestMod {
    public class TestHook : IFoxModHookV3 {

        public TestHook() {
            var fxCore = new Fox_Core("NimbusFox", "TestMod", "DEV");
            DirectoryTest(fxCore.SaveDirectory);
            DirectoryTest(fxCore.ConfigDirectory);

            fxCore.ExceptionManager.HandleException(new Exception("Test"));
        }

        private void DirectoryTest(DirectoryManager directory) {
            var testFile = "test.bin";

            var testBlob = BlobAllocator.Blob(true);

            testBlob.SetString("test", "yes");

            var testStream = new MemoryStream();

            testBlob.SaveJsonStream(testStream);

            testStream.Seek(0, SeekOrigin.Begin);

            directory.FetchDirectory("Test Dir");
            directory.WriteFileStream(testFile, testStream, true);
            directory.ReadFileStream(testFile);

            directory.WriteFile(testFile, testBlob, true, true);
            directory.ReadFile<Blob>(testFile, true);
            directory.WriteFile(testFile, testBlob, false, true);
            directory.ReadFile<Blob>(testFile);
        }

        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }
        public void UniverseUpdateBefore(Universe universe, Timestep step) { }
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

        public void OnPlayerConnect(Entity entity) { }
        public void OnPlayerDisconnect(Entity entity) { }
    }
}
