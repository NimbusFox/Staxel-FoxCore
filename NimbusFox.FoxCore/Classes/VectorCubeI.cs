using System;
using Plukit.Base;
using Staxel.FoxCore;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorCubeI {
        public Vector3I Start { get; }
        public Vector3I End { get; }

        [Obsolete]
        public AreaI X { get; private set; }
        [Obsolete]
        public AreaI Y { get; private set; }
        [Obsolete]
        public AreaI Z { get; private set; }

        public VectorCubeI(Vector3I start, Vector3I end) {
            Vector3I first;
            Vector3I second;
            Global.Sort3I(start, end, out first, out second);
            Start = first;
            End = second;
            X = new AreaI(start.X, end.X);

            Y = new AreaI(start.Y, end.Y);

            Z = new AreaI(start.Z, end.Z);
        }

        private VectorCubeI() { }

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

        public long GetTileCount() {
            return (End.X - Start.X) * (End.Z - Start.Z) * (End.Y - Start.Y);
        }
    }
}
