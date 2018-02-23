using System;
using System.Collections.Generic;
using System.Linq;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Classes {
    public class VectorRangeI {
        public Vector3I Start { get; internal set; }
        public Vector3I End { get; internal set; }
        internal string ParticleCode { get; set; }
        internal Entity TrackEntity { get; set; }
        internal Vector3I TrackEntityLastPos { get; set; }
        internal Dictionary<Entity, bool> Entities { get; set; }
        internal Vector3I Offset { get; set; }

        public VectorRangeI() {
            Entities = new Dictionary<Entity, bool>();
            Offset = new Vector3I(0, 0, 0);
        }

        internal List<Entity> GetEntities() {
            return new List<Entity>(Entities.Keys);
        }

        internal Dictionary<Entity, bool> GetUnrenderedEntities() {
            var min = (int)Math.Ceiling((decimal) Entities.Count / 100);
            var items = new Dictionary<Entity, bool>(Entities).Reverse().Where(x => !x.Value).Take(min < 100 ? 100 : min);
            return items.ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
