using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NimbusFox.FoxCore.V3.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Input;
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

        public static bool VectorContains(Vector2 start, Vector2 end, Vector2 location) {
            return start.X < location.X &&
                   end.X > location.X &&
                   start.Y < location.Y &&
                   end.Y > location.Y;
        }

        public static Vector2I VectorLocation(Vector2I start, Vector2I end, Vector2I location) {
            if (!VectorContains(start.ToVector2F().ToVector2(), end.ToVector2F().ToVector2(), location.ToVector2F().ToVector2())) {
                return new Vector2I(-1, -1);
            }

            return location - start;
        }

        public static Vector2I VectorLocation(Vector2 start, Vector2 end, Vector2 location) {
            return VectorLocation(new Vector2I((int) Math.Round(start.X), (int) Math.Round(start.Y)),
                new Vector2I((int) Math.Round(end.X), (int) Math.Round(end.Y)),
                new Vector2I((int) Math.Round(location.X), (int) Math.Round(location.Y)));
        }

        public static Color GetColorByCoordinate(Texture2D image, Vector2I location) {
            var data = new Color[image.Width * image.Height];
            image.GetData(data);

            return data[(image.Width * location.Y) + location.X];
        }

        public static void VectorLoop(Vector3I start, Vector3I end, Action<int, int, int> coordFunction) {
            VectorLoop(new VectorCubeI(start, end), coordFunction);
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

        public static List<ScanCode> GetAllKeysPressed() {
            var keys = new List<ScanCode>();

            ClientContext.UserInput.EnableDebugKeys = true;

            foreach (ScanCode enu in Enum.GetValues(typeof(ScanCode))) {
                if (ClientContext.InputSource.IsDebugKeyDown(enu)) {
                    keys.Add(enu);
                }
            }

            ClientContext.UserInput.EnableDebugKeys = false;

            return keys;
        }

        private static string _clipboard = "";
        public static string GetClipboardText() {
            var t = new Thread(GetClipboard);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            while (t.IsAlive) {
            }
            return _clipboard;
        }

        [STAThread]
        private static void GetClipboard() {
            if (Clipboard.ContainsText()) {
                _clipboard = Clipboard.GetText(TextDataFormat.UnicodeText);
            }
        }

        public static Texture2D GetTexture(DeviceContext graphics, uint width = 25, uint height = 25) {
            var square = new Texture2D(graphics.Graphics.GraphicsDevice, (int)width, (int)height);

            var colors = new Color[25 * 25];

            for (var i = 0; i < colors.Length; i++) {
                colors[i] = Color.White;
            }

            square.SetData(colors);

            return square;
        }
    }
}
