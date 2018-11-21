using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes {
    public class BlobFile : IDisposable {
        private readonly bool _binary;
        private readonly FileStream _fileStream;
        protected readonly Blob Blob;
        private Timer _timer;
        private readonly BinaryFormatter _bf = new BinaryFormatter();

        protected BlobFile(FileStream stream, bool binary = false) {
            if (stream == null) {
                return;
            }

            _fileStream = stream;
            _binary = binary;
            _fileStream.Seek(0, SeekOrigin.Begin);

            Blob = BlobAllocator.Blob(true);

            if (_fileStream.Length == 0) {
                ForceSave();
                return;
            }

            try {
                if (binary) {
                    var bytes = Convert.FromBase64String((string)_bf.Deserialize(_fileStream));

                    using (var ms = new MemoryStream(bytes)) {
                        ms.Seek(0, SeekOrigin.Begin);
                        Blob.ReadJson((string)_bf.Deserialize(ms));
                    }
                } else {
                    Blob.ReadJson(_fileStream.ReadAllText());
                }

                stream.Seek(0, SeekOrigin.Begin);

                File.WriteAllBytes(stream.Name + ".bak", stream.ReadAllBytes());
            } catch {
                Logger.WriteLine($"{stream.Name} was corrupt. Default values will be loaded");
            }
        }

        public void Load(Blob blob) {
            Blob.MergeFrom(blob);
            Save();
        }

        public void Dispose() {
            ForceSave();
            _timer?.Stop();
            _timer?.Dispose();
            _fileStream?.Dispose();
        }

        protected void Save() {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = new Timer(5000) { AutoReset = false };
            _timer.Start();
            _timer.Elapsed += (sender, args) => {
                ForceSave();
            };
        }

        protected void ForceSave() {
            _fileStream.SetLength(0);
            _fileStream.Position = 0;
            if (_binary) {
                using (var ms = new MemoryStream()) {
                    _bf.Serialize(ms, Blob.ToString());
                    ms.Seek(0, SeekOrigin.Begin);
                    _bf.Serialize(_fileStream, Convert.ToBase64String(ms.ToArray()));
                }
            } else {
                Blob.SaveJsonStream(_fileStream);
            }
            _fileStream.Flush(true);
        }

        public Blob CopyBlob() {
            var tempBlob = BlobAllocator.Blob(true);

            tempBlob.AssignFrom(Blob);

            return tempBlob;
        }
    }
}
