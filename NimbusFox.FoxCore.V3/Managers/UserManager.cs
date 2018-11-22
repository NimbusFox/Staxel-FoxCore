using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.V3.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.Managers {
    public class UserManager {
        private const string CacheFile = "User.cache";
        private readonly UserCacheFile _cacheFile;

        internal UserManager() {
            _cacheFile = new UserCacheFile(
                CoreHook.FxCore.ConfigDirectory.ObtainFileStream(CacheFile, FileMode.OpenOrCreate));
        }

        private void Flush() {
            _cacheFile.InteralSave();
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
                _cacheFile.Blob.FromObject(uid, newUser);
            } else {
                var user = _cacheFile.Blob.ToObject<UserCache>(uid);
                user.DisplayName = name;
                _cacheFile.Blob.FromObject(uid, user);
            }

            Flush();
        }

        private List<UserCache> CloneCache() {
            var list = new List<UserCache>();

            foreach (var blob in _cacheFile.Blob.Entries) {
                list.Add(_cacheFile.Blob.ToObject<UserCache>(blob.Key));
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
