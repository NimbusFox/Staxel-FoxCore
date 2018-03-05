using System;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorCubeI {
        public AreaI X { get; }
        public AreaI Y { get; }
        public AreaI Z { get; }

        public VectorCubeI(Vector3I start, Vector3I end) {
            X = new AreaI(start.X, end.X);

            Y = new AreaI(start.Y, end.Y);

            Z = new AreaI(start.Z, end.Z);
        }

        public bool IsInside(Vector3I position) {
            return X.Start < position.X
                   && Y.Start < position.Y
                   && Z.Start < position.Z
                   && X.End > position.X
                   && Y.End > position.Y
                   && Z.End > position.Z;
        }

        public bool IsInside(Vector3D position) {
            return X.Start < position.X
                   && Y.Start < position.Y
                   && Z.Start < position.Z
                   && X.End > position.X
                   && Y.End > position.Y
                   && Z.End > position.Z;
        }

        public long GetTileCount() {
            return (X.End - X.Start) * (Z.End - Z.Start) * (Y.End - Y.Start);
        }

        public Vector3I GetStart() {
            return new Vector3I(X.Start, Y.Start, Z.Start);
        }

        public Vector3I GetEnd() {
            return new Vector3I(X.End, Y.End, Z.End);
        }
    }
}
