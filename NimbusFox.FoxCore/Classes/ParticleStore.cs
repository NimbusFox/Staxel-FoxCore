using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Classes {
    public class ParticleStore {
        public Vector3D Target { get; set; }
        public Entity Entity { get; set; }
        public string ParticleCode { get; set; }

        public ParticleStore(Vector3D target, Entity entity, string particleCode) {
            Target = target;
            Entity = entity;
            ParticleCode = particleCode;
        }
    }
}
