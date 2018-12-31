using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NimbusFox.FoxCore.V3.Events;
using NimbusFox.FoxCore.V3.Events.Builders;
using NimbusFox.FoxCore.V3.Patches;
using NimbusFox.FoxCore.V3.UI.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Effects;
using Staxel.Input;
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

        public event Action<GraphicsDevice> LoadUIContent;

        private Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();
        private Dictionary<string, UiBackground> _backgrounds = new Dictionary<string, UiBackground>();
        private Dictionary<string, UiTexture2D> _images = new Dictionary<string, UiTexture2D>();

        static FoxUIHook() {
            OverlayRendererPatches.Initialize();
        }

        public FoxUIHook() {
            Instance = this;

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

                foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uiBackground")) {
                    using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                        var blob = BlobAllocator.Blob(true);

                        blob.LoadJsonStream(stream);

                        blob.GetString("code");
                        blob.GetString("topLeft");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("topLeft")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("topLeft"));
                        }
                        blob.GetString("topMiddle");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("topMiddle")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("topMiddle"));
                        }
                        blob.GetString("topRight");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("topRight")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("topRight"));
                        }
                        blob.GetString("middleLeft");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("middleLeft")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("middleLeft"));
                        }
                        blob.GetString("middleMiddle");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("middleMiddle")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("middleMiddle"));
                        }
                        blob.GetString("middleRight");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("middleRight")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("middleRight"));
                        }
                        blob.GetString("bottomLeft");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("bottomLeft")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("bottomLeft"));
                        }
                        blob.GetString("bottomMiddle");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("bottomMiddle")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("bottomMiddle"));
                        }
                        blob.GetString("bottomRight");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("bottomRight")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("bottomRight"));
                        }
                    }
                }

                foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uiPicture")) {
                    using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                        var blob = BlobAllocator.Blob(true);

                        blob.LoadJsonStream(stream);

                        blob.GetString("code");
                        blob.GetString("picture");
                        if (!File.Exists(Path.Combine(GameContext.ContentLoader.RootDirectory, blob.GetString("picture")))) {
                            throw new FileNotFoundException("Could not find file: " + blob.GetString("picture"));
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
        public void GameContextInitializeBefore() {
            Windows.Clear();
        }
        public void GameContextInitializeAfter() {
        }
        public void GameContextDeinitialize() { }

        public void GameContextReloadBefore() {
        }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
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

        public void ClientContextInitializeInit() {
        }

        public void ClientContextInitializeBefore() {
            Windows.Clear();
        }

        public void ClientContextInitializeAfter() {

        }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }

        public void CleanupOldSession() {
            Windows.Clear();
        }
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            return true;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            return true;
        }

        internal void LoadContent(GraphicsDevice graphics) {
            _fonts.Clear();
            _backgrounds.Clear();
            _images.Clear();

            foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uifont")) {
                using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                    stream.Seek(0L, SeekOrigin.Begin);
                    var blob = BlobAllocator.Blob(true);

                    blob.LoadJsonStream(stream);

                    if (Process.GetCurrentProcess().ProcessName.Contains("Staxel.Client")) {
                        _fonts.Add(blob.GetString("code"), ContentManager.Load<SpriteFont>(blob.GetString("xnb")));
                    }
                }
            }

            foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uiBackground")) {
                using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                    stream.Seek(0L, SeekOrigin.Begin);
                    var blob = BlobAllocator.Blob(true);

                    blob.LoadJsonStream(stream);

                    _backgrounds.Add(blob.GetString("code"), new UiBackground(graphics, blob));
                }
            }

            foreach (var asset in GameContext.AssetBundleManager.FindByExtension("uiPicture")) {
                using (var stream = GameContext.ContentLoader.ReadStream(asset)) {
                    stream.Seek(0L, SeekOrigin.Begin);
                    var blob = BlobAllocator.Blob(true);

                    blob.LoadJsonStream(stream);

                    var image = new UiTexture2D();
                    image.SetTexture(context => 
                        Texture2D.FromStream(context.Graphics.GraphicsDevice,
                            GameContext.ContentLoader.ReadStream(blob.GetString("picture"))));

                    _images.Add(blob.GetString("code"), image);
                }
            }

            LoadUIContent?.Invoke(graphics);
        }

        public SpriteFont GetFont(string code = Constants.Fonts.MyFirstCrush24) {
            return !_fonts.ContainsKey(code) ? null : _fonts[code];
        }

        public UiBackground GetBackground(string code) {
            return !_backgrounds.ContainsKey(code) ? null : _backgrounds[code];
        }

        public UiTexture2D GetPicture(string code) {
            return !_images.ContainsKey(code) ? null : _images[code];
        }
    }
}
