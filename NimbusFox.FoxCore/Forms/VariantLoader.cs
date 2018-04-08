using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plukit.Base;
using Staxel.Core;
using Staxel.FoxCore.Managers;
using Color = Microsoft.Xna.Framework.Color;

namespace Staxel.FoxCore.Forms {
    public partial class VariantLoader : Form {
        private Dictionary<string, Dictionary<Color, Color>> _colors;
        public VariantLoader() {
            InitializeComponent();
        }

        private void CycleGetPalettes(DirectoryManager dir) {
            foreach (var subDir in dir.Directories) {
                CycleGetPalettes(dir.FetchDirectory(subDir));
            }

            if (dir.FileExists("palettes.json")) {
                var wait = true;
                dir.ReadFile<Blob>("palettes.json", blob => {
                    foreach (var palette in blob.KeyValueIteratable) {
                        var current = blob.GetBlob(palette.Key);

                        if (!_colors.ContainsKey(palette.Key)) {
                            _colors.Add(palette.Key, new Dictionary<Microsoft.Xna.Framework.Color, Color>());
                        }

                        foreach (var colorKey in current.KeyValueIteratable.Keys) {
                            var colorK = ColorMath.ParseString(colorKey);
                            var colorV = ColorMath.ParseString(current.GetString(colorKey));
                            if (!_colors[palette.Key].ContainsKey(colorK)) {
                                _colors[palette.Key].Add(colorK, colorV);
                            }
                            Application.DoEvents();
                        }
                        Application.DoEvents();
                    }
                    wait = false;
                }, true);
                while (wait) {
                    Application.DoEvents();
                }
            }
        }

        private void CycleGenerate(DirectoryManager dir) {
            foreach (var subDir in dir.Directories) {
                CycleGenerate(dir.FetchDirectory(subDir));
            }

            if (dir.FileExists("variations.json")) {
                var wait = true;
                dir.ReadFile<Blob>("variations.json", blob => {
                    var total = 0;

                    if (blob.Contains("Tiles")) {
                        total += blob.FetchBlob("Tiles").KeyValueIteratable.Count;
                    }

                    if (blob.Contains("Items")) {
                        total += blob.FetchBlob("Items").KeyValueIteratable.Count;
                    }

                    prgrssTileItems.Maximum = total;

                    var index = 0;

                    if (blob.Contains("Tiles")) {
                        var tiles = blob.GetBlob("Tiles");

                        foreach (var tile in tiles.KeyValueIteratable.Keys) {
                            index++;
                            Application.DoEvents();

                            prgrssTileItems.Value = index;
                            lblTileItem.Text = $@"{tile} ({index} of {total})";

                            if (GameContext.TileDatabase.AllMaterials().Any(x =>
                                string.Equals(x.Code, tile, StringComparison.CurrentCultureIgnoreCase))) {

                                var baseTile = GameContext.TileDatabase.GetTileConfiguration(tile).MakeTile();

                                var currentTile = tiles.GetBlob(tile);

                                foreach (var generate in currentTile.KeyValueIteratable) {
                                    var current = currentTile.GetBlob(generate.Key);
                                    var palettes = current.GetStringList("Palettes");

                                    var iindex = 0;

                                    prgrssPalettes.Maximum = palettes.Length;

                                    var overrideBlob = current.FetchBlob("Overrides");

                                    foreach (var palette in palettes) {
                                        iindex++;

                                        prgrssPalettes.Value = iindex;
                                        lblPalette.Text = $@"{palette} ({iindex} of {palettes.Length})";
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
                            index++;
                            Application.DoEvents();

                            prgrssTileItems.Value = index;
                            lblTileItem.Text = $@"{item} ({index} of {total})";

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

                                    var iindex = 0;

                                    prgrssPalettes.Maximum = palettes.Length;

                                    var overrideBlob = variant.FetchBlob("Overrides");

                                    foreach (var palette in palettes) {
                                        iindex++;

                                        prgrssPalettes.Value = iindex;
                                        lblPalette.Text = $@"{palette} ({iindex} of {palettes.Length})";

                                        if (_colors.ContainsKey(palette)) {
                                            var paletteDir = dir.FetchDirectory(palette);

                                            VariantManager.CreateItem(_colors[palette], kind, string.Format(newVariant, palette), item, paletteDir, overrideBlob);
                                        }
                                    }
                                }

                                itemWait = false;
                            }, true);

                            while (itemWait) {
                                Application.DoEvents();
                            }
                        }
                    }

                    wait = false;
                }, true);
                while (wait) {
                    Application.DoEvents();
                }
            }
        }

        private void VariantLoader_Shown(object sender, EventArgs e) {
            _colors = new Dictionary<string, Dictionary<Color, Color>>();
            var modDir = new DirectoryManager().FetchDirectory("mods");

            prgrssMods.Maximum = modDir.Directories.Count;

            foreach (var mod in modDir.Directories) {
                prgrssMods.Value = modDir.Directories.ToList().IndexOf(mod) + 1;
                lblMod.Text = $@"{mod} ({prgrssMods.Value} of {modDir.Directories.Count})";
                _colors.Clear();
                Application.DoEvents();

                lblTileItem.Text = @"Fetching Palettes";
                lblPalette.Text = @"Fetching Palettes";
                prgrssTileItems.Style = ProgressBarStyle.Continuous;
                prgrssPalettes.Style = ProgressBarStyle.Continuous;
                Application.DoEvents();
                CycleGetPalettes(modDir.FetchDirectory(mod));

                prgrssTileItems.Style = ProgressBarStyle.Blocks;
                prgrssPalettes.Style = ProgressBarStyle.Blocks;
                Application.DoEvents();

                CycleGenerate(modDir.FetchDirectory(mod));
            }

            Close();
        }
    }
}
