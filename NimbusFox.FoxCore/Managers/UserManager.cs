using System;
using System.Collections.Generic;
using System.Linq;
using Plukit.Base;
using NimbusFox.FoxCore.Classes;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class UserManager {
        private const string CacheFile = "User.cache";
        private Blob _cache;

        internal UserManager() {
            if (!CoreHook.FxCore.ConfigDirectory.FileExists(CacheFile)) {
                _cache = BlobAllocator.Blob(true);
                Flush();
            } else {
                _cache = CoreHook.FxCore.ConfigDirectory.ReadFile<Blob>(CacheFile);
            }
        }

        private void Flush() {
            var blob = BlobAllocator.Blob(true);
            blob.SetObject("userCache", _cache);
            CoreHook.FxCore.ConfigDirectory.WriteFile(CacheFile, blob, false, true);
            Blob.Deallocate(ref blob);
        }

        private Universe Universe => CoreHook.Universe;

        public Lyst<Entity> GetPlayerEntities() {
            var output = new Lyst<Entity>();

            Universe.GetPlayers(output);

            return output;
        }

        internal void AddUpdateEntry(string uid, string name) {
            if (string.IsNullOrEmpty(GetNameByUid(uid))) {
                var newUser = new UserCache {
                    DisplayName = name,
                    Uid = uid
                };
                _cache.SetObject(uid, newUser);
            } else {
                var user = _cache.GetObject<UserCache>(uid);
                user.DisplayName = name;
                _cache.SetObject(uid, user);
            }

            Flush();
        }

        private List<UserCache> CloneCache() {
            var list = new List<UserCache>();

            foreach (var blob in _cache.KeyValueIteratable) {
                list.Add(_cache.GetObject<UserCache>(blob.Key));
            }

            return list;
        }

        public string GetUidByName(string name) {
            return CloneCache().FirstOrDefault(x => string.Equals(x.DisplayName, name, StringComparison.CurrentCultureIgnoreCase))?.Uid ?? "";
        }

        public string GetNameByUid(string uid) {
            return CloneCache().FirstOrDefault(x => x.Uid == uid)?.DisplayName ?? "";
        }

        public IReadOnlyList<string> GetPlayerNames() {
            return GetPlayerEntities().Select(x => x.PlayerEntityLogic.DisplayName()).ToList();
        }

        public Entity GetPlayerEntityByName(string name) {
            return GetPlayerEntities().FirstOrDefault(x => string.Equals(x.PlayerEntityLogic.DisplayName(), name, StringComparison.CurrentCultureIgnoreCase));
        }

        public Entity GetPlayerEntityByUid(string uid) {
            return GetPlayerEntities().FirstOrDefault(x => x.PlayerEntityLogic.Uid() == uid);
        }

        public Vector3D? GetPlayerVector(string name) {
            var target = GetPlayerEntityByName(name);
            if (target == null) {
                return null;
            }

            return GetEntityVector(target);
        }

        public Vector3D GetEntityVector(Entity entity) {
            return entity.Physics.BottomPosition();
        }

        public bool IsUserOnline(string uid) {
            return GetPlayerEntities().Any(x => x.PlayerEntityLogic.Uid() == uid);
        }
    }
}
