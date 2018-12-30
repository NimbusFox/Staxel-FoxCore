using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;
using Staxel;
using Staxel.Input;
using Staxel.Items;
using Staxel.Tiles;

namespace NimbusFox.FoxCore.V3 {
    public static class Extensions {
        public static Item MakeItem(this Tile tile) {
            return tile.Configuration.MakeItem();
        }

        public static Item MakeItem(this TileConfiguration tile, string kind = "staxel.item.placer") {
            var itemBlob = BlobAllocator.Blob(true);
            itemBlob.SetString("kind", kind);
            itemBlob.SetString("tile", tile.Code);
            var item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);
            Blob.Deallocate(ref itemBlob);

            if (item.IsNull()) {
                return Item.NullItem;
            }

            return item.Item;
        }

        public static void SetObject(this Blob blob, string key, object obj) {
            blob.FetchBlob(key).SetObject(obj);
        }

        public static void SetObject(this Blob blob, object obj) {
            var tempBlob = BlobAllocator.Blob(true);
            tempBlob.ReadJson(JsonConvert.SerializeObject(obj));
            blob.MergeFrom(tempBlob);
            Blob.Deallocate(ref tempBlob);
        }

        public static T GetObject<T>(this Blob blob, string key, T _default) where T : class {
            var value = blob.GetObject<T>(key);

            if (value == null) {
                return _default;
            }

            return value;
        }

        public static T GetObject<T>(this Blob blob, string key) where T : class {
            if (!blob.Contains(key)) {
                return null;
            }

            try {
                var tempBlob = blob.FetchBlob(key);

                return JsonConvert.DeserializeObject<T>(tempBlob.ToString());
            } catch when (!Debugger.IsAttached) {
                return null;
            }
        }

        public static T GetObject<T>(this Blob blob, T _default) where T : class {
            var value = blob.GetObject<T>();

            if (value == null) {
                return _default;
            }

            return value;
        }

        public static T GetObject<T>(this Blob blob) where T : class {
            try {
                return JsonConvert.DeserializeObject<T>(blob.ToString());
            } catch when (!Debugger.IsAttached) {
                return null;
            }
        }

        public static T GetPrivatePropertyValue<T>(this object parentObject, string field) {
            return (T)AccessTools.Property(parentObject.GetType(), field)?.GetValue(parentObject);
        }

        public static void SetPrivatePropertyValue(this object parentObject, string field, object value) {
            AccessTools.Property(parentObject.GetType(), field)?.SetValue(parentObject, value);
        }

        public static T GetPrivateFieldValue<T>(this object parentObject, string field) {
            return (T)AccessTools.Field(parentObject.GetType(), field)?.GetValue(parentObject);
        }

        public static void SetPrivateFieldValue(this object parentObject, string field, object value) {
            AccessTools.Field(parentObject.GetType(), field)?.SetValue(parentObject, value);
        }

        public static T GetPrivatePropertyValue<T>(this object parentObject, string field, Type type) {
            return (T)AccessTools.Property(type, field)?.GetValue(parentObject);
        }

        public static void SetPrivatePropertyValue(this object parentObject, string field, object value, Type type) {
            AccessTools.Property(type, field)?.SetValue(parentObject, value);
        }

        public static T GetPrivateFieldValue<T>(this object parentObject, string field, Type type) {
            return (T)AccessTools.Field(type, field)?.GetValue(parentObject);
        }

        public static void SetPrivateFieldValue(this object parentObject, string field, object value, Type type) {
            AccessTools.Field(type, field)?.SetValue(parentObject, value);
        }

        public static void RunPrivateVoid(this object parentObject, string method) {
            AccessTools.Method(parentObject.GetType(), method).Invoke(parentObject, new object[0]);
        }

        public static Vector2 Vector2(this MouseState mousestate) {
            return new Vector2(mousestate.X, mousestate.Y);
        }

        public static char? GetFirstPressedKey(this IEnumerable<ScanCode> keys) {
            foreach (var key in keys) {
                var text = Enum.GetName(typeof(ScanCode), key);
                if (text?.Length <= 2 && text.Length > 0) {
                    var ch = text.Length == 2 && text[0] == 'D' ? text[1] : text[0];
                    if (char.IsLetterOrDigit(ch) || char.IsSymbol(ch)) {
                        return ch;
                    }
                }
            }

            return null;
        }

        public static ScanCode? GetDirectionKey(this IEnumerable<ScanCode> keys) {
            foreach (var key in keys) {
                if (key == ScanCode.Left ||
                    key == ScanCode.Right ||
                    key == ScanCode.Up ||
                    key == ScanCode.Down) {
                    return key;
                }
            }

            return null;
        }
    }
}
