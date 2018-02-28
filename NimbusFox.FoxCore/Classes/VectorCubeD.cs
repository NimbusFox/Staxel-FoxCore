using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class VectorCubeD {
        private Vector3D Start { get; }
        private Vector3D End { get; }

        public AreaD X { get; }
        public AreaD Y { get; }
        public AreaD Z { get; }

        public VectorCubeD(Vector3D start, Vector3D end) {
            Start = start;
            End = end;

            X = new AreaD(start.X, end.X);

            Y = new AreaD(start.Y, end.Y);

            Z = new AreaD(start.Z, end.Z);
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
    }
}
