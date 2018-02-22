using System.Collections.Generic;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Classes {
    public class VectorRangeI {
        public Vector3I Start { get; set; }
        public Vector3I End { get; set; }
        public string ParticleCode { get; set; }
        public Entity TrackEntity { get; set; }
        internal List<Entity> Entities { get; set; }

        public VectorRangeI() {
            Entities = new List<Entity>();
        }

        internal List<Entity> GetEntities() {
            return new List<Entity>(Entities);
        }
    }
}
