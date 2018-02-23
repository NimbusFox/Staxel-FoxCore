using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class WorldManager {
        public Universe Universe {
            get { return CoreHook.Universe; }
        }

        public World World {
            get { return Universe.World; }
        }

        public Lyst<Entity> GetPlayerEntities() {
            var output = new Lyst<Entity>();

            Universe.GetPlayers(output);

            return output;
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

        public VectorSquareI GetSquareI(Vector3I start, Vector3I end) {
            return new VectorSquareI(start, end);
        }

        public VectorSquareD GetSquareD(Vector3D start, Vector3D end) {
            return new VectorSquareD(start, end);
        }

        public VectorCubeI GetCubeI(Vector3I start, Vector3I end) {
            return new VectorCubeI(start, end);
        }

        public VectorCubeD GetCubeD(Vector3D start, Vector3D end) {
            return new VectorCubeD(start, end);
        } 
    }
}
