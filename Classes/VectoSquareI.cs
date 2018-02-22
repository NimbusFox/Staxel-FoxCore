using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class VectorSquareI {
        private Vector3I Start { get;}
        private Vector3I End { get;}

        public AreaI X { get; }
        public AreaI Z { get; }

        public VectorSquareI(Vector3I start, Vector3I end) {
            Start = start;
            End = end;

            X = new AreaI(start.X, end.X);

            Z = new AreaI(start.Z, end.Z);
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
