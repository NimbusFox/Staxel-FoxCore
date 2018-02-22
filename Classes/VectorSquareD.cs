using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class VectorSquareD {
        private Vector3D Start { get; }
        private Vector3D End { get; }

        public AreaD X { get; }
        public AreaD Z { get; }

        public VectorSquareD(Vector3D start, Vector3D end) {
            Start = start;
            End = end;

            X = new AreaD(start.X, end.X);

            Z = new AreaD(start.Z, end.Z);
        }

        public bool IsInside(Vector3I position) {
            return X.Start <= position.X
                   && Z.Start <= position.Z
                   && X.End >= position.X
                   && Z.End >= position.Z;
        }

        public bool IsInside(Vector3D position) {
            return X.Start <= position.X
                   && Z.Start <= position.Z
                   && X.End >= position.X
                   && Z.End >= position.Z;
        }
    }
}
