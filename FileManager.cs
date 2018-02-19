using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Staxel;

namespace NimbusFox.FoxCore {
    public class FileManager {
        private readonly string StreamLocation;
        private readonly string LocalContentLocation;

        public FileManager(string author, string mod) {
            StreamLocation = $"Mods\\{author}\\{mod}\\";
            LocalContentLocation = GameContext.ContentLoader.LocalContentDirectory + StreamLocation;

            if (!Directory.Exists(LocalContentLocation)) {
                Directory.CreateDirectory(LocalContentLocation);
            }
        }

        public void WriteFile<T>(string fileName, T data) {
            var bf = new BinaryFormatter();
            var stream = new MemoryStream();
            bf.Serialize(stream, data);
            stream.Seek(0L, SeekOrigin.Begin);
            GameContext.ContentLoader.WriteLocalStream(StreamLocation + fileName, stream);
        }

        public T ReadFile<T>(string filename) where T : new() {
            if (FileExists(filename)) {
                var bf = new BinaryFormatter();
                var stream = GameContext.ContentLoader.ReadLocalStream(LocalContentLocation + filename);
                stream.Seek(0L, SeekOrigin.Begin);
                return (T) bf.Deserialize(stream);
            }

            return new T();
        }

        public bool FileExists(string filename) {
            return File.Exists(LocalContentLocation + filename);
        }
    }
}
