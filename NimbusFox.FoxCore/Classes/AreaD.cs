using System;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    [Obsolete]
    public class AreaD {
        public double Start { get; }
        public double End { get; }

        public AreaD(double start, double end) {
            Start = start >= end ? end : start;
            End = Start != start ? end : start;
        }
    }
}
