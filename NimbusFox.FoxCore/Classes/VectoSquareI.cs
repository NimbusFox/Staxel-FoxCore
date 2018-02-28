using System;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorSquareI {

        public AreaI X { get; set; }
        public AreaI Z { get; set; }

        public VectorSquareI() { }

        public VectorSquareI(Vector3I start, Vector3I end) {
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

        public int GetTileCount() {
            return (X.End - X.Start) * (Z.End - Z.Start);
        }
    }
}
