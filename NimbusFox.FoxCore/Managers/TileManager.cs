using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NimbusFox;
using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel.FoxCore.Classes;
using Staxel.Logic;
using Staxel.Tiles;

namespace Staxel.FoxCore.Managers {
    public class TileManager {
        private static DirectoryManager _fileManager;
        private static Dictionary<string, DictionaryData> _data;
        private const string DataFile = "Tiles.json";

        [Obsolete("For debug items only. Do not use in production")]
        public TileManager(Dictionary<string, DictionaryData> data) {
            _data = data;
        }

        internal TileManager() {
            _fileManager = new DirectoryManager("NimbusFox", "FoxCore");

            if (!_fileManager.FileExists(DataFile)) {
                _data = new Dictionary<string, DictionaryData>();
            } else {
                var pause = true;

                _fileManager.ReadFile<Dictionary<string, DictionaryData>>(DataFile, data => {
                    _data = data;
                    pause = false;
                }, true);

                while (pause) { }
            }

            var tiles = GameContext.TileDatabase.AllMaterials().ToList();

            foreach (var tile in tiles) {
                var bits = tile.Code.Split('.');
                int index;
                var name = GetName(bits, out index).ToLower();
                var variantCode = bits.Length - 1 < index ? "" : bits[index].ToLower() == name ? "" : bits[index].ToLower();

                variantCode = Regex.Replace(variantCode, name, "", RegexOptions.IgnoreCase);

                if (!_data.ContainsKey(name)) {
                    _data.Add(name, new DictionaryData());
                    _data[name].SubVariantKey = name;
                }

                _data[name] = GetData(bits, name, index, tile, _data[name], variantCode);
            }

            Logger.WriteLine($"FoxCore: Writing {tiles.Count} dictionary words");

            _fileManager.WriteFile(DataFile, _data, null, true);
        }

        private DictionaryData GetData(IReadOnlyList<string> bits, string name, int index, TileConfiguration tile, DictionaryData parent, string variantCode = null) {
            if (variantCode == null) {
                variantCode = bits[index];
            }

            variantCode = Regex.Replace(variantCode, name, "", RegexOptions.IgnoreCase);
            variantCode = Regex.Replace(variantCode, "_", "");

            if (!parent.Variants.ContainsKey(variantCode) && !parent.SubVariants.Any(x => x.SubVariantKey == variantCode) &&
                !parent.SubVariants.Any(x => x.Variants.ContainsKey(variantCode))) {
                parent.Variants.Add(variantCode, tile.Code);

                parent.Variants = parent.Variants.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                return parent;
            }

            DictionaryData variant;

            if (!parent.SubVariants.Any(x => x.SubVariantKey == variantCode)) {
                variant = new DictionaryData();
                variant.SubVariantKey = variantCode;

                if (parent.Variants.ContainsKey(variantCode)) {
                    var vCode = parent.Variants[variantCode].Split('.').Last();

                    vCode = Regex.Replace(vCode, variantCode, "", RegexOptions.IgnoreCase);
                    vCode = Regex.Replace(vCode, "_", "");

                    variant.Variants.Add(vCode, parent.Variants[variantCode]);
                    parent.Variants.Remove(variantCode);
                    variant.Variants = variant.Variants.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                }

                parent.SubVariants.Add(variant);
            } else {
                variant = parent.SubVariants.First(x => x.SubVariantKey == variantCode);
            }

            if (index + 1 <= bits.Count - 1) {
                GetData(bits, variantCode, index + 1, tile, variant);
            }

            return parent;
        }

        private static string GetName(string[] bits, out int index) {
            if (bits.Length < 4) {
                index = bits.Length - 1;
                return bits.Last();
            }

            if (bits[2].ToLower() == "roofing") {
                index = 3;
                return "Roof";
            }

            if (bits[2].ToLower() == "door") {
                index = 3;
                return "Door";
            }

            if (bits[2].ToLower() == "dungeon") {
                index = 3;
                return "Dungeon";
            }

            if (bits[2].ToLower() == "bobbleheads") {
                index = 3;
                return "BobbleHead";
            }

            if (bits[2].ToLower() == "totems") {
                index = 3;
                return "Totem";
            }

            if (bits[2].ToLower() == "fairyvillage") {
                index = 3;
                return "Fairy";
            }

            if (bits[2].ToLower() == "merchantstalls") {
                index = 3;
                return "Merchant";
            }

            if (bits[2].ToLower() == "carts") {
                index = 3;
                return "MerchantCart";
            }

            if (bits[2].ToLower() == "grass") {
                index = 3;
                return "Grass";
            }

            if (bits[3].ToLower().Contains("grasslayer")) {
                index = 4;
                return "Grass";
            }

            if (bits[2].ToLower() == "balloon") {
                index = 3;
                return "Balloon";
            }

            if (bits[2].ToLower() == "tree") {
                index = 3;
                return "Tree";
            }

            if (bits[2].ToLower() == "bush") {
                index = 3;
                return "Bush";
            }

            if (bits[2].ToLower() == "patisserie") {
                index = 3;
                return "Patisserie";
            }

            if (bits[3].ToLower().Contains("piano")) {
                index = 4;
                return "Piano";
            }

            if (bits[2].ToLower() == "farmanimals") {
                index = 3;
                return "Animal";
            }

            if (bits[2].ToLower() == "flower") {
                index = 3;
                return "Flower";
            }

            if (bits[2].ToLower() == "museum") {
                index = 3;
                return "Museum";
            }

            if (bits[2].ToLower() == "weed") {
                index = 3;
                return "Weed";
            }

            if (bits[2].ToLower() == "wicker") {
                index = 3;
                return "Wicker";
            }

            if (bits[2].ToLower() == "retrotest") {
                index = 3;
                return "RetroTest";
            }

            if (bits[2].ToLower() == "wood") {
                index = 3;
                return "Wood";
            }

            if (bits[2].ToLower() == "merchanttotem") {
                index = 3;
                return "MerchantTotem";
            }

            if (bits[2].ToLower() == "crop") {
                index = 3;
                return "Crop";
            }

            index = 4;
            return bits[3];
        }

        public bool IsValidTile(string tileCode) {
            return GameContext.TileDatabase.AllMaterials()
                .Any(x => string.Equals(x.Code, tileCode, StringComparison.CurrentCultureIgnoreCase));
        }

        public Tile GetTile(string tileCode) {
            if (IsValidTile(tileCode)) {
                return GameContext.TileDatabase.AllMaterials().First(x =>
                    string.Equals(x.Code, tileCode, StringComparison.CurrentCultureIgnoreCase)).MakeTile();
            }

            return new Tile();
        }

        public string GetTileCode(string input) {
            var regex = new Regex("([^:|#]+)");

            var bits = regex.Matches(input.ToLower());

            if (bits.Count > 0) {
                if (_data.ContainsKey(bits[0].Value)) {
                    var current = _data[bits[0].Value];

                    if (bits.Count > 1) {
                        for (var i = 1; i < bits.Count; i++) {
                            if (current.Variants.Any()) {
                                if (current.Variants.Keys.Select(x => x.ToLower()).Contains(bits[i].Value)) {
                                    return current.Variants[current.Variants.Keys.First(x => x.ToLower() == bits[i].Value)];
                                }
                            }

                            if (current.SubVariants != null) {
                                if (current.SubVariants.Any(x => x.SubVariantKey.ToLower() == bits[i].Value)) {
                                    current = current.SubVariants.First(x => x.SubVariantKey.ToLower() == bits[i].Value);

                                    if (i + 1 >= bits.Count) {
                                        return GetDefault(current).Variants.First().Value;
                                    }

                                    continue;
                                }
                            }

                            return GetDefault(current).Variants.First().Value;
                        }
                    } else {
                        var thisOne = GetDefault(current);
                        return thisOne.Variants.First().Value;
                    }
                }
            }

            return input;
        }

        private DictionaryData GetDefault(DictionaryData parent) {
            while (true) {
                if (parent.Variants.Any()) {
                    return parent;
                }
                foreach (var variant in parent.SubVariants) {
                    return GetDefault(variant);
                }
            }
        }
    }
}
