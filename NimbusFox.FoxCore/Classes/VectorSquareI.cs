using System;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorSquareI {
        public Vector3I Start { get; }
        public Vector3I End { get; }

        public VectorSquareI() { }

        public VectorSquareI(Vector3I start, Vector3I end) {
            Helpers.Sort3I(start, end, out var _start, out var _end);

            Start = _start;
            End = _end;
        }

        public bool IsInside(Vector3I position) {
            return Start.X <= position.X 
                   && Start.Z <= position.Z 
                   && End.X >= position.X 
                   && End.Z >= position.Z;
        }

        public bool IsInside(Vector3D position) {
            return Start.X <= position.X
                   && Start.Z <= position.Z
                   && End.X >= position.X
                   && End.Z >= position.Z;
        }

        public int GetTileCount() {
            return (End.X - Start.X) * (End.Z - Start.Z);
        }
    }
}
