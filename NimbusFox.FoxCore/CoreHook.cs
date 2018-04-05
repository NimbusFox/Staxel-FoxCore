using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore.Client.Staxel.Builders;
using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using NimbusFox.FoxCore.VersionCheck;
using Plukit.Base;
using Staxel;
using Staxel.Collections;
using Staxel.Core;
using Staxel.Effects;
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
        private static Dictionary<string, Dictionary<Color, Color>> _colors;

        public void Dispose() {
            _cacheTick = 0;
        }

        public void GameContextInitializeInit() {
            UserManager = new UserManager();
        }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() {
            if (Process.GetCurrentProcess().ProcessName.Contains("ContentBuilder")) {
                _colors = new Dictionary<string, Dictionary<Color, Color>>();
                var modDir = new DirectoryManager().FetchDirectory("mods");

                foreach (var mod in modDir.Directories) {
                    _colors.Clear();
                    CycleGetPalettes(modDir.FetchDirectory(mod));

                    CycleGenerate(modDir.FetchDirectory(mod));
                }

                _colors.Clear();
            }
        }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }

        public void GameContextReloadAfter() {
        }

        private static void CycleGetPalettes(DirectoryManager dir) {
            foreach (var subDir in dir.Directories) {
                CycleGetPalettes(dir.FetchDirectory(subDir));
            }

            if (dir.FileExists("palettes.json")) {
                var wait = true;
                dir.ReadFile<Blob>("palettes.json", blob => {
                    foreach (var palette in blob.KeyValueIteratable) {
                        var current = blob.GetBlob(palette.Key);

                        if (!_colors.ContainsKey(palette.Key)) {
                            _colors.Add(palette.Key, new Dictionary<Color, Color>());
                        }

                        foreach (var colorKey in current.KeyValueIteratable.Keys) {
                            var colorK = ColorMath.ParseString(colorKey);
                            var colorV = ColorMath.ParseString(current.GetString(colorKey));
                            if (!_colors[palette.Key].ContainsKey(colorK)) {
                                _colors[palette.Key].Add(colorK, colorV);
                            }
                        }
                    }
                    wait = false;
                }, true);
                while (wait) { }
            }
        }

        private static void CycleGenerate(DirectoryManager dir) {
            foreach (var subDir in dir.Directories) {
                CycleGenerate(dir.FetchDirectory(subDir));
            }

            if (dir.FileExists("variations.json")) {
                var wait = true;
                dir.ReadFile<Blob>("variations.json", blob => {
                    if (blob.Contains("Tiles")) {
                        var tiles = blob.GetBlob("Tiles");

                        foreach (var tile in tiles.KeyValueIteratable.Keys) {
                            if (GameContext.TileDatabase.AllMaterials().Any(x =>
                                string.Equals(x.Code, tile, StringComparison.CurrentCultureIgnoreCase))) {

                                var baseTile = GameContext.TileDatabase.GetTileConfiguration(tile).MakeTile();

                                var currentTile = tiles.GetBlob(tile);

                                foreach (var generate in currentTile.KeyValueIteratable) {
                                    var current = currentTile.GetBlob(generate.Key);
                                    var palettes = current.GetStringList("Palettes");

                                    var overrideBlob = current.FetchBlob("Overrides");

                                    foreach (var palette in palettes) {
                                        if (_colors.ContainsKey(palette)) {
                                            var paletteDir = dir.FetchDirectory(palette);

                                            VariantManager.CreateTile(baseTile, _colors[palette], string.Format(generate.Key, palette), paletteDir, overrideBlob);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (blob.Contains("Items")) {
                        var items = blob.GetBlob("Items");

                        foreach (var item in items.KeyValueIteratable.Keys) {
                            var paths = item.Split('/').ToList();

                            var file = paths.Last();

                            paths.Remove(paths.Last());

                            var error = false;

                            var currentDir = dir.TopLevel;

                            foreach (var path in paths) {
                                if (currentDir.DirectoryExists(path)) {
                                    currentDir = currentDir.FetchDirectory(path);
                                } else {
                                    error = true;
                                    break;
                                }
                            }

                            if (error) {
                                continue;
                            }

                            if (!currentDir.FileExists(file)) {
                                continue;
                            }

                            var itemWait = true;

                            currentDir.ReadFile<Blob>(file, blobData => {
                                var kind = blobData.GetString("kind");

                                var itemVariations = items.GetBlob(item);

                                foreach (var newVariant in itemVariations.KeyValueIteratable.Keys) {
                                    var variant = itemVariations.GetBlob(newVariant);

                                    var palettes = variant.GetStringList("Palettes");

                                    var overrideBlob = variant.FetchBlob("Overrides");

                                    foreach (var palette in palettes) {
                                        if (_colors.ContainsKey(palette)) {
                                            var paletteDir = dir.FetchDirectory(palette);

                                            VariantManager.CreateItem(_colors[palette], kind, string.Format(newVariant, palette), item, paletteDir, overrideBlob);
                                        }
                                    }
                                }

                                itemWait = false;
                            }, true);

                            while (itemWait) { }
                        }
                    }

                    wait = false;
                }, true);
                while (wait) { }
            }
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
