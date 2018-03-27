using System;
using Plukit.Base;
using Staxel.FoxCore;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorCubeD {
        public Vector3D Start { get; private set; }
        public Vector3D End { get; private set; }

        [Obsolete]
        public AreaD X { get; private set; }
        [Obsolete]
        public AreaD Y { get; private set; }
        [Obsolete]
        public AreaD Z { get; private set; }

        public VectorCubeD(Vector3D start, Vector3D end) {
            Vector3D first;
            Vector3D second;
            Global.Sort3D(start, end, out first, out second);
            Start = first;
            End = second;

            X = new AreaD(start.X, end.X);

            Y = new AreaD(start.Y, end.Y);

            Z = new AreaD(start.Z, end.Z);
        }

        public bool IsInside(Vector3I position) {
            return Start.X < position.X
                   && Start.Y < position.Y
                   && Start.Z < position.Z
                   && End.X > position.X
                   && End.Y > position.Y
                   && End.Z > position.Z;
        }

        public bool IsInside(Vector3D position) {
            return Start.X < position.X
                   && Start.Y < position.Y
                   && Start.Z < position.Z
                   && End.X > position.X
                   && End.Y > position.Y
                   && End.Z > position.Z;
        }
    }
}
