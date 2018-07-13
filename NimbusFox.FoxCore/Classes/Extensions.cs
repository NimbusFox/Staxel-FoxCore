﻿using System.IO;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Tiles;

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

                    return JsonConvert.DeserializeObject<T>(ms.ReadAllText());
                }
            } catch {
                return null;
            }
        }
    }
}
