using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using NimbusFox.FoxCore.Client.Staxel.Builders.Painter;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Client.Staxel.Builders {
    public class ParticleHostEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2 {

        public EntityPainter Instance() {
            return new ParticleHostEntityPainter();
        }

        public EntityLogic Instance(Entity entity, bool server) {
            return new ParticleHostEntityLogic(entity);
        }

        public void Load() { }

        public string Kind {
            get { return KindCode; }
        }

        public bool IsTileStateEntityKind() {
            return false;
        }

        public static string KindCode {
            get { return "mods.nimbusfox.foxcore.entity.particleHost"; }
        }

        void IEntityPainterBuilder.Load() { }
    }
}
