using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.V3.Classes.FxBlob;
using Plukit.Base;

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
