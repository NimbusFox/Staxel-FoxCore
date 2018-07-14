using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class VectorSquareD {
        public Vector3D Start { get; }
        public Vector3D End { get; }

        public VectorSquareD(Vector3D start, Vector3D end) {
            Helpers.Sort3D(start, end, out var _start, out var _end);
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
    }
}
