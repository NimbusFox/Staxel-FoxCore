using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Managers.Particles;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel.Logic;

namespace Staxel.FoxCore.Managers.Particles {
    public class EntityFollowParticleManager : EntityParticleManager {
        public void Add(Entity entity, string particleCode, int xOffset = 0, int yOffset = 0, int zoffset = 0) {
            var particleHost = new VectorRangeI {
                ParticleCode = particleCode,
                TrackEntity = entity,
                Offset = new Vector3I(xOffset, yOffset, zoffset)
            };

            particles.Add(particleHost);
        }

        public new void DrawParticles() {
            if (LastTick <= DateTime.Now.Ticks) {
                foreach (var vector in Clone()) {
                    if (CoreHook.Universe.TryGetEntity(vector.TrackEntity.Id, out _)) {
                        vector.Start = Converters.From3Dto3I(vector.TrackEntity.Physics.BottomPosition());
                        vector.Start = new Vector3I(vector.Start.X + vector.Offset.Y, vector.Start.Y + vector.Offset.X, vector.Start.Z + vector.Offset.Z);
                    }
                }

                base.DrawParticles();
            }
        }

        public void Remove(Entity entity) {
            var vectors = Clone().Where(x => x.TrackEntity == entity).ToList();

            if (vectors.Any()) {
                foreach (var vector in vectors) {
                    foreach (var target in vector.GetEntities()) {
                        ((ParticleHostEntityLogic)target.Logic).Finish();
                        vector.Entities.Remove(target);
                        target.Dispose();
                    }

                    particles.Remove(vector);
                }
            }
        }
    }
}
