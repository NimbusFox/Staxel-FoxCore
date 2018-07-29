using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class BlobFile : IDisposable {
        private bool _binary = false;
        private FileStream _fileStream;
        protected Blob Blob;
        private Timer _timer;

        public BlobFile(FileStream stream, bool binary = false) {
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
                    using (var ms = new MemoryStream()) {
                        _fileStream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        Blob.Read(ms);
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
                    Blob.WriteFull(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(_fileStream);
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