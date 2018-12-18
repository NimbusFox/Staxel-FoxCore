using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Effects;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.Events.Builders {
    public class ModOptionsEventBuilder : IEffectBuilder {
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { }
        public void Load() { }

        public static string KindCode => "nimbusfox.foxcore.effect.modOptions";

        public string Kind() {
            return KindCode;
        }

        public IEffect Instance(Timestep step, Entity entity, EntityPainter painter, EntityUniverseFacade facade, Blob data,
            EffectDefinition definition, EffectMode mode) {
            return new ModOptionsEvent();
        }
    }
}
