using System;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes {
    [Serializable]
    public class VectorCubeD {
        public Vector3D Start { get; private set; }
        public Vector3D End { get; private set; }

        public VectorCubeD(Vector3D start, Vector3D end) {
            Helpers.Sort3D(start, end, out var first, out var second);
            Start = first;
            End = second;
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
