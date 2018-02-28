using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Staxel;
using System;

namespace NimbusFox.FoxCore.Managers {
    public class FileManager {
        private readonly string StreamLocation;
        private readonly string LocalContentLocation;

        internal FileManager(string author, string mod) {
            StreamLocation = $"Mods\\{author}\\{mod}\\";
            LocalContentLocation = GameContext.ContentLoader.LocalContentDirectory + StreamLocation;

            if (!Directory.Exists(LocalContentLocation)) {
                Directory.CreateDirectory(LocalContentLocation);
            }
        }

        public void WriteFile<T>(string fileName, T data, bool outputAsText = false) {
            var stream = new MemoryStream();
            var output = JsonConvert.SerializeObject(data, Formatting.Indented);
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
            GameContext.ContentLoader.WriteLocalStream(StreamLocation + fileName, stream);
        }

        public T ReadFile<T>(string filename, bool inputIsText = false) where T : new() {
            if (FileExists(filename)) {
                var bf = new BinaryFormatter();
                var stream = GameContext.ContentLoader.ReadLocalStream(StreamLocation + filename);
                try {
                    string input;
                    if (!inputIsText) {
                        input = (string)bf.Deserialize(stream);
                    } else {
                        var sr = new StreamReader(stream);
                        input = sr.ReadToEnd();
                    }
                    stream.Seek(0L, SeekOrigin.Begin);
                    return JsonConvert.DeserializeObject<T>(input);
                } catch {
                    stream.Seek(0L, SeekOrigin.Begin);
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(bf.Deserialize(stream)));
                }
            }

            return new T();
        }

        public bool FileExists(string filename) {
            return File.Exists(LocalContentLocation + filename);
        }
    }
}
