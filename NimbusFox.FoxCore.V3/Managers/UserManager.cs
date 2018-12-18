using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.V3.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.Managers {
    public class UserManager {

        internal UserManager() {
        }

        private Universe Universe => CoreHook.Universe;

        public Lyst<Entity> GetPlayerEntities() {
            var output = new Lyst<Entity>();

            Universe.GetPlayers(output);

            return output;
        }

        public string GetUidByName(string name) {
            return ServerContext.RightsManager.TryGetUIDByUsername(name, out var uid) ? uid : null;
        }

        public string GetNameByUid(string uid) {
            return GetPlayerEntityByUid(uid).PlayerEntityLogic.DisplayName();
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
