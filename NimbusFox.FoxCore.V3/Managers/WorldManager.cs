using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.V3.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.Managers {
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
