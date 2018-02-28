using System;
using Plukit.Base;

namespace NimbusFox.FoxCore {
    public static class Converters {
        public static Vector3I From3Dto3I(this Vector3D input) {
            return new Vector3I(Convert.ToInt32(input.X), Convert.ToInt32(input.Y), Convert.ToInt32(input.Z));
        }
    }
}
