using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Staxel;

namespace NimbusFox.FoxCore.Managers {
    [Obsolete("Please use the new DirectoryManager", true)]
    public class FileManager {
        internal readonly string StreamLocation;
        private readonly string LocalContentLocation;

        internal FileManager(string author, string mod) {
            StreamLocation = Path.Combine("Mods", author, mod);
            LocalContentLocation = Path.Combine(GameContext.ContentLoader.LocalContentDirectory, StreamLocation);

            if (!Directory.Exists(LocalContentLocation)) {
                Directory.CreateDirectory(LocalContentLocation);
            }
        }

        public static string SerializeObject<T>(T data) {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static T DeserializeObject<T>(string json, Type type) {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void WriteFile<T>(string fileName, T data, bool outputAsText = false) {
            new Thread(() => {
                var stream = new MemoryStream();
                var output = SerializeObject(data);
                stream.Seek(0L, SeekOrigin.Begin);
                if (!outputAsText) {
                    var bf = new BinaryFormatter();
                    bf.Serialize(stream, output);
                } else {
                    var sw = new StreamWriter(stream);
                    sw.Write(output);
                    sw.Flush();
                }
                stream.Seek(0L, SeekOrigin.Begin);
                GameContext.ContentLoader.WriteLocalStream(Path.Combine(LocalContentLocation, fileName), stream);
            }).Start();
        }

        public void WriteFileStream(string filename, Stream stream) {
            new Thread(() => {
                stream.Seek(0L, SeekOrigin.Begin);
                GameContext.ContentLoader.WriteLocalStream(Path.Combine(LocalContentLocation, filename), stream);
            }).Start();
        }

        public T ReadFile<T>(string filename, bool inputIsText = false) {
            if (FileExists(filename)) {
                var bf = new BinaryFormatter();
                var stream = GameContext.ContentLoader.ReadLocalStream(Path.Combine(LocalContentLocation, filename));
                string input;
                if (!inputIsText) {
                    input = (string)bf.Deserialize(stream);
                } else {
                    var sr = new StreamReader(stream);
                    input = sr.ReadToEnd();
                }
                stream.Seek(0L, SeekOrigin.Begin);
                return DeserializeObject<T>(input, typeof(T));
            }

            return (T) Activator.CreateInstance(typeof(T));
        }

        public Stream ReadFileStream(string filename, bool required = false) {
            var stream = GameContext.ContentLoader.ReadLocalStream(Path.Combine(LocalContentLocation, filename), required);
            stream.Seek(0L, SeekOrigin.Begin);
            return stream;
        }

        public bool FileExists(string filename) {
            return File.Exists(Path.Combine(LocalContentLocation, filename));
        }
    }
}
