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
        private static FileManager _fileManager;
        private static Dictionary<string, DictionaryData> _data;
        private const string _dataFile = "dictionary.json";

        internal TileManager() {
            _fileManager = new FileManager("NimbusFox", "FoxCore");

            _data = !_fileManager.FileExists(_dataFile) ? new Dictionary<string, DictionaryData>() : _fileManager.ReadFile<Dictionary<string, DictionaryData>>(_dataFile, true);

            var tiles = GameContext.TileDatabase.AllMaterials().ToList();

            var existingVariant = new Dictionary<string, string>();
            foreach (var tile in tiles) {
                var name = tile.Code.Remove(0, tile.Code.LastIndexOf('.') + 1);
                var nvName = Regex.Replace(name, @"[\d-]", "") != "" ? Regex.Replace(name, @"[\d-]", "") : name;
                var nv = name.Replace(nvName, "");
                var variantCode = tile.Code.Replace($".{name}", "");

                if (!existingVariant.ContainsKey(variantCode)) {
                    existingVariant.Add(variantCode, nvName);
                }

                if (!_data.ContainsKey(nvName)) {
                    _data.Add(nvName, new DictionaryData());
                }

                var current = _data[nvName];

                if (!current.Variants.ContainsKey(nv)) {
                    current.Variants.Add(nv, tile.Code);
                }

                var list = current.Variants.ToList();
                list.Sort((pair1, pair2) => string.Compare(pair1.Key, pair2.Key, StringComparison.Ordinal));
                current.Variants = list.ToDictionary(x => x.Key, x => x.Value);
            }

            var dataList = _data.ToList();
            dataList.Sort((pair1, pair2) => string.Compare(pair1.Key, pair2.Key, StringComparison.Ordinal));
            _data = dataList.ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine($"Writing {_data.Sum(x => x.Value.Variants.Values.Count)} dictionary words");

            _fileManager.WriteFile(_dataFile, _data, true);
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
    }
}
