using System;
using System.Linq;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.V3 {
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
    }
}
