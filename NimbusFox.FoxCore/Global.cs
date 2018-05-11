using Plukit.Base;

namespace NimbusFox.FoxCore {
    internal static class Global {
        public static void Sort3D(Vector3D first, Vector3D second, out Vector3D start, out Vector3D end) {
            var startx = 0.0;
            var endx = 0.0;
            var starty = 0.0;
            var endy = 0.0;
            var startz = 0.0;
            var endz = 0.0;

            SortDouble(first.X, second.X, out startx, out endx);
            SortDouble(first.Y, second.Y, out starty, out endy);
            SortDouble(first.Z, second.Z, out startz, out endz);

            start = new Vector3D(startx, starty, startz);
            end = new Vector3D(endx, endy, endz);
        }

        public static void Sort3I(Vector3I first, Vector3I second, out Vector3I start, out Vector3I end) {
            var startx = 0;
            var endx = 0;
            var starty = 0;
            var endy = 0;
            var startz = 0;
            var endz = 0;

            SortInt(first.X, second.X, out startx, out endx);
            SortInt(first.Y, second.Y, out starty, out endy);
            SortInt(first.Z, second.Z, out startz, out endz);

            start = new Vector3I(startx, starty, startz);
            end = new Vector3I(endx, endy, endz);
        }

        public static void SortDouble(double first, double second, out double start, out double end) {
            start = first >= second ? second : first;
            end = first == start ? second : first;
        }

        public static void SortInt(int first, int second, out int start, out int end) {
            start = first >= second ? second : first;
            end = first == start ? second : first;
        }
    }
}
