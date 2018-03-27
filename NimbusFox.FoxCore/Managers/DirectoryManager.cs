using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.FoxCore.Classes;

namespace Staxel.FoxCore.Managers {
    public class DirectoryManager {
        private string _localContentLocation;
        private DirectoryManager _parent { get; set; }

        public DirectoryManager Parent => _parent ?? this;

        internal DirectoryManager(string author, string mod) {
            var streamLocation = Path.Combine("Mods", author, mod);
            _localContentLocation = Path.Combine(GameContext.ContentLoader.LocalContentDirectory, streamLocation);

            if (!Directory.Exists(_localContentLocation)) {
                Directory.CreateDirectory(_localContentLocation);
            }
        }

        private DirectoryManager() { }

        public DirectoryManager FetchDirectory(string name) {
            var dir = new DirectoryManager {
                _localContentLocation = Path.Combine(_localContentLocation, name),
                _parent = this
            };

            CreateDirectory(name);

            return dir;
        }

        public void CreateDirectory(string name) {
            if (!DirectoryExists(name)) {
                Directory.CreateDirectory(Path.Combine(_localContentLocation, name));
            }
        }

        public IReadOnlyList<string> Directories => new DirectoryInfo(_localContentLocation).GetDirectories().Select(dir => dir.Name).ToList();

        public IReadOnlyList<string> Files => new DirectoryInfo(_localContentLocation).GetFiles().Select(file => file.Name).ToList();

        public bool FileExists(string name) {
            return Files.Contains(name);
        }

        public bool DirectoryExists(string name) {
            return Directories.Contains(name);
        }

        public static Blob SerializeObject<T>(T data) {
            var blob = BlobAllocator.AcquireAllocator().NewBlob(true);
            blob.ObjectToBlob(null, data);
            return blob;
        }

        public void WriteFile<T>(string fileName, T data, Action onFinish = null, bool outputAsText = false) {
            new Thread(() => {
                var stream = new MemoryStream();
                var output = SerializeObject(data);
                stream.Seek(0L, SeekOrigin.Begin);
                if (!outputAsText) {
                    stream.WriteBlob(output);
                } else {
                    output.SaveJsonStream(stream);
                }
                stream.Seek(0L, SeekOrigin.Begin);
                GameContext.ContentLoader.WriteLocalStream(Path.Combine(_localContentLocation, fileName), stream);
                onFinish?.Invoke();
            }).Start();
        }

        public void WriteFileStream(string filename, Stream stream) {
            new Thread(() => {
                stream.Seek(0L, SeekOrigin.Begin);
                GameContext.ContentLoader.WriteLocalStream(Path.Combine(_localContentLocation, filename), stream);
            }).Start();
        }

        public void ReadFile<T>(string filename, Action<T> onLoad, bool inputIsText = false) {
            new Thread(() => {
                if (FileExists(filename)) {
                    var stream =
                        GameContext.ContentLoader.ReadLocalStream(Path.Combine(_localContentLocation, filename));
                    Blob input;
                    if (!inputIsText) {
                        input = stream.ReadBlob();
                    } else {
                        input = BlobAllocator.AcquireAllocator().NewBlob(false);
                        var sr = new StreamReader(stream);
                        input.ReadJson(sr.ReadToEnd());
                    }

                    stream.Seek(0L, SeekOrigin.Begin);
                    if (typeof(T) == input.GetType()) {
                        onLoad((T) (object) input);
                    }

                    onLoad(input.BlobToObject(null, (T) Activator.CreateInstance(typeof(T))));
                }

                onLoad((T) Activator.CreateInstance(typeof(T)));
            }).Start();
        }

        public void ReadFileStream(string filename, Action<Stream> onLoad, bool required = false) {
            new Thread(() => {
                var stream =
                    GameContext.ContentLoader.ReadLocalStream(Path.Combine(_localContentLocation, filename), required);
                stream.Seek(0L, SeekOrigin.Begin);
                onLoad(stream);
            }).Start();
        }

        public void DeleteFile(string name) {
            if (FileExists(name)) {
                File.Delete(Path.Combine(_localContentLocation, name));
            }
        }

        public void DeleteDirectory(string name, bool recursive) {
            if (DirectoryExists(name)) {
                Directory.Delete(Path.Combine(_localContentLocation, name), recursive);
            }
        }
    }
}
