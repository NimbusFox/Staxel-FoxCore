namespace NimbusFox.FoxCore.Classes {
    public class AreaI {
        public int Start { get; }
        public int End { get; }

        public AreaI(int start, int end) {
            Start = start > end ? end : start;
            End = end < start ? end : start;
        }
    }
}
