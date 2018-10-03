using System;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    public class VectorCubeI {
        public Vector3I Start { get; private set; }
        public Vector3I End { get; private set; }

        public Vector3I OrigStart { get; }
        public Vector3I OrigEnd { get; }

        public VectorCubeI(Vector3I start, Vector3I end) {
            Helpers.Sort3I(start, end, out var first, out var second);
            Start = first;
            End = second;
            OrigStart = start;
            OrigEnd = end;
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

        public VectorCubeI GetOuterRegions() {
            return new VectorCubeI {
                Start = new Vector3I(Start.X - 1, Start.Y - 1, Start.Z - 1),
                End = new Vector3I(End.X + 1, End.Y + 1, End.Z + 1)
            };
        }
    }
}
