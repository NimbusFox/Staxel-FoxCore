﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Tiles;
using JsonConvert = NimbusFox.FoxCore.Dependencies.Newtonsoft.Json.JsonConvert;

namespace NimbusFox.FoxCore.Classes {
    public static class Extensions {
        public static Item MakeItem(this Tile tile) {
            return tile.Configuration.MakeItem();
        }

        public static Item MakeItem(this TileConfiguration tile) {
            var itemBlob = BlobAllocator.Blob(true);
            itemBlob.SetString("kind", "staxel.item.Placer");
            itemBlob.SetString("tile", tile.Code);
            var item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);
            Blob.Deallocate(ref itemBlob);

            if (item.IsNull()) {
                return Item.NullItem;
            }

            return item.Item;
        }

        public static void SetObject(this Blob blob, string key, object obj) {
            blob.FetchBlob(key).ReadJson(JsonConvert.SerializeObject(obj));
        }

        public static void SetObject(this Blob blob, object obj) {
            blob.ReadJson(JsonConvert.SerializeObject(obj));
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

                using (var ms = new MemoryStream()) {
                    tempBlob.SaveJsonStream(ms);

                    ms.Seek(0L, SeekOrigin.Begin);

                    var sr = new StreamReader(ms);

                    return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                }
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
                using (var ms = new MemoryStream()) {
                    blob.SaveJsonStream(ms);

                    ms.Seek(0L, SeekOrigin.Begin);

                    var sr = new StreamReader(ms);

                    return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                }
            } catch when (!Debugger.IsAttached) {
                return null;
            }
        }

        internal static T GetPrivatePropertyValue<T>(this object parentObject, string field) {
            return (T)parentObject.GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
                .GetValue(parentObject);
        }

        internal static void SetPrivatePropertyValue(this object parentObject, string field, object value) {
            parentObject.GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(parentObject, value);
        }

        internal static T GetPrivateFieldValue<T>(this object parentObject, string field) {
            return (T)parentObject.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
                .GetValue(parentObject);
        }

        internal static void SetPrivateFieldValue(this object parentObject, string field, object value) {
            parentObject.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(parentObject, value);
        }

        internal static T GetPrivatePropertyValue<T>(this object parentObject, string field, Type type) {
            return (T)type.GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
                .GetValue(parentObject);
        }

        internal static void SetPrivatePropertyValue(this object parentObject, string field, object value, Type type) {
            type.GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(parentObject, value);
        }

        internal static T GetPrivateFieldValue<T>(this object parentObject, string field, Type type) {
            return (T)type.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?
                .GetValue(parentObject);
        }

        internal static void SetPrivateFieldValue(this object parentObject, string field, object value, Type type) {
            type.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(parentObject, value);
        }
    }
}
