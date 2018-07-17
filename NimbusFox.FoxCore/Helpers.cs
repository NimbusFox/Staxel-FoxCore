using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Tiles;

namespace NimbusFox.FoxCore {
    public static class Helpers {
        public static void Sort3D(Vector3D first, Vector3D second, out Vector3D start, out Vector3D end) {
            SortDouble(first.X, second.X, out var startx, out var endx);
            SortDouble(first.Y, second.Y, out var starty, out var endy);
            SortDouble(first.Z, second.Z, out var startz, out var endz);

            start = new Vector3D(startx, starty, startz);
            end = new Vector3D(endx, endy, endz);
        }

        public static void Sort3I(Vector3I first, Vector3I second, out Vector3I start, out Vector3I end) {
            SortInt(first.X, second.X, out var startx, out var endx);
            SortInt(first.Y, second.Y, out var starty, out var endy);
            SortInt(first.Z, second.Z, out var startz, out var endz);

            start = new Vector3I(startx, starty, startz);
            end = new Vector3I(endx, endy, endz);
        }

        public static void SortDouble(double first, double second, out double start, out double end) {
            start = first >= second ? second : first;
            end = first == start ? second : first;
        }

        public static void SortInt(int first, int second, out int start, out int end) {
            start = first >= second ? second : first;
            end = first == start ? second : first;
        }

        public static Item MakeItem(string code) {
            var tile = GameContext.TileDatabase.AllMaterials().FirstOrDefault(x => x.Code == code);

            if (tile != default(TileConfiguration)) {
                return tile.MakeItem();
            }

            var itemBlob = BlobAllocator.Blob(true);
            itemBlob.SetString("code", code);

            var item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);
            Blob.Deallocate(ref itemBlob);

            if (item.IsNull()) {
                return Item.NullItem;
            }

            return item.Item;
        }

        public static T MakeItem<T>(string code) where T : Item {
            var item = MakeItem(code);

            if (item is T newItem) {
                return newItem;
            }

            return null;
        }

        public static Tile MakeTile(string code, uint rotation = 0) {
            var config = GameContext.TileDatabase.AllMaterials().FirstOrDefault(x => x.Code == code);

            if (config == default(TileConfiguration)) {
                throw new Exception("Unknown tile code: " + code);
            }

            return config.MakeTile(config.BuildRotationVariant(rotation));
        }

        public static TInterface ResolveOptionalDependency<TInterface>(string key) {
            var collection = GetDependencies<TInterface>(key);

            return collection.FirstOrDefault();
        }

        public static List<TInterface> GetDependencies<TInterface>(string key) {
            var output = new List<TInterface>();
            var assembly = Assembly.GetAssembly(typeof(Fox_Core));
            var dir = assembly.Location.Substring(0, assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));
            foreach (var file in new DirectoryInfo(dir).GetFiles("*.mod")) {
                var data = BlobAllocator.AcquireAllocator().NewBlob(true);
                data.ReadJson(File.ReadAllText(file.FullName));
                if (data.Contains("fxdependencykeys")) {
                    var current = data.GetStringList("fxdependencykeys");
                    if (current.Any(x => string.Equals(x, key, StringComparison.CurrentCultureIgnoreCase))) {
                        var item = Assembly.Load(Assembly.LoadFile(file.FullName.Replace(".mod", ".dll")).GetName());
                        foreach (var module in item.DefinedTypes) {
                            if (module.GetInterfaces().Contains(typeof(TInterface))) {
                                output.Add((TInterface)Activator.CreateInstance(module));
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static void VectorLoop(Vector3I start, Vector3I end, Action<int, int, int> coordFunction) {
            VectorLoop(new VectorCubeI(start, end), coordFunction);
        }

        public static void VectorLoop(Vector3D start, Vector3D end, Action<int, int, int> coordFunction) {
            VectorLoop(start.From3Dto3I(), end.From3Dto3I(), coordFunction);
        }

        public static void VectorLoop(VectorCubeI region, Action<int, int, int> coordFunction) {
            for (var y = region.Start.Y; y <= region.End.Y; y++) {
                for (var z = region.Start.Z; z <= region.End.Z; z++) {
                    for (var x = region.Start.X; x <= region.End.X; x++) {
                        coordFunction(x, y, z);
                    }
                }
            }
        }

        public static void VectorLoop(VectorCubeD region, Action<int, int, int> coordFunction) {
            VectorLoop(region.Start, region.End, coordFunction);
        }
    }
}
