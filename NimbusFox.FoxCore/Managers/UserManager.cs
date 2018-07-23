using System;
using System.Collections.Generic;
using System.Linq;
using Plukit.Base;
using NimbusFox.FoxCore.Classes;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class UserManager {
        private static readonly DirectoryManager FileManager = new DirectoryManager("NimbusFox", "FoxCore");
        private const string CacheFile = "User.cache";
        private List<UserCache> _cache;

        internal UserManager() {
            if (!FileManager.FileExists(CacheFile)) {
                _cache = new List<UserCache>();
                Flush();
            } else {
                var wait = true;
                FileManager.ReadFile<List<UserCache>>(CacheFile, (data) => {
                    _cache = data;
                    wait = false;
                });
                while (wait) { }
            }
        }

        private List<UserCache> CloneCache() {
            return new List<UserCache>(_cache);
        }

        private void Flush() {
            //FileManager.WriteFile(CacheFile, _cache);
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
                    Uid = uid,
                    Expires = DateTime.Now.AddDays(3)
                };
                _cache.Add(newUser);
            } else {
                var user = CloneCache().First(x => x.Uid == uid);
                _cache.Remove(user);
                user.DisplayName = name;
                user.Expires = DateTime.Now.AddDays(3);
                _cache.Add(user);
            }

            Flush();
        }

        internal void CacheCheck() {
            foreach (var user in CloneCache()) {
                if (user.Expires.Ticks <= DateTime.Now.Ticks) {
                    _cache.Remove(user);
                }
            }
        }

        public string GetUidByName(string name) {
            return CloneCache().FirstOrDefault(x => string.Equals(x.DisplayName, name, StringComparison.CurrentCultureIgnoreCase))?.Uid;
        }

        public string GetNameByUid(string uid) {
            return CloneCache().FirstOrDefault(x => x.Uid == uid)?.DisplayName;
        }

        public IReadOnlyList<string> GetPlayerNames() {
            return GetPlayerEntities().Select(x => x.PlayerEntityLogic.DisplayName()).ToList();
        }

        public Entity GetPlayerEntityByName(string name) {
            return GetPlayerEntities().FirstOrDefault(x => x.PlayerEntityLogic.DisplayName() == name);
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
    }
}
