using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using NimbusFox.FoxCore.Staxel.Builders.Painter;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Staxel.Builders {
    public class ParticleHostEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder {

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

        public static string KindCode {
            get { return "mods.nimbusfox.foxcore.entity.particleHost"; }
        }

        void IEntityPainterBuilder.Load() { }
    }
}
