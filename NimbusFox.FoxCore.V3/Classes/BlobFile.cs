using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json.Bson;
using NimbusFox.FoxCore.V3.Classes.FxBlob;
using Plukit.Base;
namespace NimbusFox.FoxCore.V3.Classes {
    public class BlobFile : IDisposable {
        private readonly bool _binary;
        private readonly FileStream _fileStream;
        protected FoxBlob Blob { get; private set; }
        private Timer _timer;
        private readonly BinaryFormatter _bf = new BinaryFormatter();

        protected bool IsStream => _fileStream == null;

        protected BlobFile(FileStream stream, bool binary = false) {
            Blob = new FoxBlob();
            if (stream == null) {
                return;
            }

            _fileStream = stream;
            _binary = binary;
            _fileStream.Seek(0, SeekOrigin.Begin);

            if (_fileStream.Length == 0) {
                ForceSave();
                return;
            }

            try {
                if (binary) {
                    Blob = (FoxBlob)_bf.Deserialize(_fileStream);
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
            if (_fileStream == null) {
                return;
            }
            _fileStream.SetLength(0);
            _fileStream.Position = 0;
            if (_binary) {
                _bf.Serialize(_fileStream, Blob);
            } else {
                using (var ms = new MemoryStream()) {
                    using (var sw = new StreamWriter(ms)) {
                        sw.Write(Blob.ToString());
                        sw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.CopyTo(_fileStream);
                    }
                }
            }
            _fileStream.Flush(true);
        }

        public FoxBlob CopyBlob() {
            return Blob.Clone();
        }

        public Blob ToStaxelBlob() {
            var temp = BlobAllocator.Blob(true);

            temp.ReadJson(Blob.ToString());

            return temp;
        }
    }
}
