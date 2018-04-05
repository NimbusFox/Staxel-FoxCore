using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class VectorFileData {
        public Vector3I Size { get; set; } = new Vector3I();
        public Dictionary<string, VectorData> Mappings { get; set; } = new Dictionary<string, VectorData>();
    }
}
