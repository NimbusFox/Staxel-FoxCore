using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NimbusFox.FoxCore.V3.Patches;
using NimbusFox.FoxCore.V3.UI.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.V3 {
    public class FoxUIHook : IModHookV3 {

        public static FoxUIHook Instance;

        internal List<UiWindow> Windows { get; } = new List<UiWindow>();

        internal static PatchController PatchController { get; } = new PatchController("NimbusFox.Fox UI");

        internal ContentManager ContentManager;

        public event Action LoadUIContent;

        private Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();

        static FoxUIHook() {
            OverlayRendererPatches.Initialize();
        }

        public FoxUIHook() {
            Instance = this;

            LoadUIContent += () => {
                foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uifont")) {
                    using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                        var blob = BlobAllocator.Blob(true);

                        blob.LoadJsonStream(stream);

                        if (Process.GetCurrentProcess().ProcessName.Contains("Staxel.Client")) {
                            _fonts.Add(blob.GetString("code"), ContentManager.Load<SpriteFont>(blob.GetString("xnb")));
                        } else {
                            blob.GetString("code");
                            blob.GetString("xnb");
                        }
                    }
                }
            };

            if (Process.GetCurrentProcess().ProcessName.Contains("Staxel.ContentBuilder")) {
                foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uifont")) {
                    using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                        var blob = BlobAllocator.Blob(true);

                        blob.LoadJsonStream(stream);

                        blob.GetString("code");
                        blob.GetString("xnb");

                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("xnb")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("xnb"));
                        }
                    }
                }
            }
        }

        public void Dispose() {
            ContentManager?.Dispose();
        }

        public void GameContextInitializeInit() {
        }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() {
        }
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

        public void ClientContextInitializeInit() {
        }
        public void ClientContextInitializeBefore() { }

        public void ClientContextInitializeAfter() {

        }
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

        internal void LoadContent() {
            LoadUIContent?.Invoke();
        }

        public SpriteFont GetFont(string code = Constants.Fonts.MyFirstCrush24) {
            return !_fonts.ContainsKey(code) ? null : _fonts[code];
        }
    }
}
