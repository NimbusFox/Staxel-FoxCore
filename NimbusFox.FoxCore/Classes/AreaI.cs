using System;

namespace NimbusFox.FoxCore.Classes {
    [Serializable]
    [Obsolete]
    public class AreaI {
        public int Start { get; set; }
        public int End { get; set; }

        public AreaI() { }

        public AreaI(int start, int end) {
            Start = start >= end ? end : start;
            End = Start == start ? end : start;
        }
    }
}
