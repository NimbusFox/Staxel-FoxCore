using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;
using Staxel.Voxel;

namespace Staxel.FoxCore.Classes {
    public class VoxelOutput {
        public VoxelObject Voxels { get; internal set; }
        public VectorFileData JsonData { get; internal set; }
    }
}
