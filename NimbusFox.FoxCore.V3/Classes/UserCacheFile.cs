using System.IO;
using NimbusFox.FoxCore.V3.Classes.FxBlob;

namespace NimbusFox.FoxCore.V3.Classes {
    internal class UserCacheFile : BlobFile {
        internal new FoxBlob Blob => base.Blob;
        internal UserCacheFile(FileStream stream) : base(stream, true) {
        }

        internal void InteralSave() {
            Save();
        }
    }
}
