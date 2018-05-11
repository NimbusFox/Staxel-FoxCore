namespace NimbusFox.FoxCore.Classes {
    public class CodeEntry {
        public readonly string Code;
        public readonly uint Rotation;

        public CodeEntry(string code, uint rotation) {
            Code = code;
            Rotation = rotation;
        }

        public override bool Equals(object obj) {
            var codeEntry = (CodeEntry)obj;
            if ((int)Rotation == (int)codeEntry.Rotation)
                return Code == codeEntry.Code;
            return false;
        }

        public override int GetHashCode() {
            return Code.GetHashCode();
        }
    }
}
