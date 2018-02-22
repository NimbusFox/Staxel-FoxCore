using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.FoxCore.Staxel.Builders.Painter {
    class ParticleHostEntityPainter : EntityPainter {
        private EffectRenderer _effectRenderer = Allocator.EffectRenderer.Allocate();

        protected override void Dispose(bool disposing) {
            if (_effectRenderer != null) {
                _effectRenderer.Dispose();
                Allocator.EffectRenderer.Release(ref _effectRenderer);
            }
        }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController,
            EntityUniverseFacade facade,
            int updateSteps) {
            _effectRenderer.RenderUpdate(timestep, entity.Effects, entity, this, facade, entity.Physics.Position);
        }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }
        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController,
            Timestep renderTimestep) { }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            _effectRenderer.Render(entity, this, renderTimestep, graphics, matrix, renderOrigin, renderMode);
        }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) { }
    }
}
