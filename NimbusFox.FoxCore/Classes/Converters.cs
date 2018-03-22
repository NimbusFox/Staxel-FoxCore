using System;
using Plukit.Base;
using Staxel.Core;
using Staxel.FoxCore;
using Staxel.FoxCore.Enums;

namespace NimbusFox.FoxCore {
    public static class Converters {
        public static Vector3I From3Dto3I(this Vector3D input) {
            return new Vector3I(Convert.ToInt32(input.X), Convert.ToInt32(input.Y), Convert.ToInt32(input.Z));
        }

        public static Compass GetDirection(this Heading input) {
            if (input.X < 0.75 && input.X > -0.75) {
                return Compass.NORTH;
            }

            if (input.X < -0.75 && input.X > -2.25) {
                return Compass.EAST;
            }

            if (input.X < 2.25 && input.X > 0.75) {
                return Compass.WEST;
            }

            return Compass.SOUTH;
        }
    }
}
