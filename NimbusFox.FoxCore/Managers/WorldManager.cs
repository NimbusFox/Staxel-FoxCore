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
