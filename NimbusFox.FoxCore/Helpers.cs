using System;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Tiles;

namespace NimbusFox.FoxCore {
    public static class Helpers {
        public static void Sort3D(Vector3D first, Vector3D second, out Vector3D start, out Vector3D end) {
            var startx = 0.0;
            var endx = 0.0;
            var starty = 0.0;
            var endy = 0.0;
            var startz = 0.0;
            var endz = 0.0;

            SortDouble(first.X, second.X, out startx, out endx);
            SortDouble(first.Y, second.Y, out starty, out endy);
            SortDouble(first.Z, second.Z, out startz, out endz);

            start = new Vector3D(startx, starty, startz);
            end = new Vector3D(endx, endy, endz);
        }

        public static void Sort3I(Vector3I first, Vector3I second, out Vector3I start, out Vector3I end) {
            var startx = 0;
            var endx = 0;
            var starty = 0;
            var endy = 0;
            var startz = 0;
            var endz = 0;

            SortInt(first.X, second.X, out startx, out endx);
            SortInt(first.Y, second.Y, out starty, out endy);
            SortInt(first.Z, second.Z, out startz, out endz);

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

            return config.MakeTile(rotation);
        }

        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
            while (toCheck != null && toCheck != typeof(object)) {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
