using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel.FoxCore.Classes;

namespace NimbusFox.FoxCore.Tests {
    class Program {
        // Writes blobs to files made from an object
        //static void Main(string[] args) {
        //    var blob = BlobAllocator.AcquireAllocator().NewBlob(true);
        //    var dic = new Dictionary<Guid, List<Guid>>();
        //    for (var i = 0; i < 10; i++) {
        //        var list = new List<Guid>();
        //        for (var j = 0; j < 10; j++) {
        //            list.Add(Guid.NewGuid());
        //        }
        //        dic.Add(Guid.NewGuid(), list);
        //    }

        //    if (!Directory.Exists("./Results")) {
        //        Directory.CreateDirectory("./Results");
        //    }

        //    blob.SetObject("data", dic);

        //    var ms = new MemoryStream();

        //    blob.SaveJsonStream(ms);
        //    ms.Seek(0L, SeekOrigin.Begin);

        //    File.WriteAllText($"./Results/{Guid.NewGuid().ToString()}.txt", ms.ReadAllText());
        //}

        //static void Main(string[] args) {
        //    if (File.Exists("./Results/test.txt")) {
        //        var read = File.ReadAllText("./Results/test.txt");
        //        var blob = BlobAllocator.AcquireAllocator().NewBlob(true);
        //        blob.ReadJson(read);
        //        var dic = new Dictionary<Guid, List<Guid>>();
        //        var obj = blob.BlobToObject("data", dic);

        //        Console.WriteLine(obj.Count);
        //        foreach (var pair in obj) {
        //            Console.WriteLine(pair.Value.Count);
        //        }
        //        Console.ReadKey();
        //    }
        //}

        static void Main(string[] args) {
            var dic = new Dictionary<Guid, List<Guid>>();
            for (var i = 0; i < 10; i++) {
                var list = new List<Guid>();
                for (var j = 0; j < 10; j++) {
                    list.Add(Guid.NewGuid());
                }
                dic.Add(Guid.NewGuid(), list);
            }

            if (!Directory.Exists("./Results")) {
                Directory.CreateDirectory("./Results");
            }

            File.WriteAllText($"./Results/{Guid.NewGuid().ToString()}.txt", FileManager.SerializeObject(dic));
        }
    }
}
