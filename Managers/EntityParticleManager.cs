using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Staxel.Builders;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class EntityParticleManager : ParticleManager {
        public VectorRangeI Add(Vector3I start, Entity trackEntity, string particleCode) {
            var particleHost = new VectorRangeI {
                Start = start,
                ParticleCode = particleCode,
                TrackEntity = trackEntity
            };

            particles.Add(particleHost);
            return particleHost;
        }

        public VectorRangeI Add(Vector3D start, Entity trackEntity, string particleCode) {
            return Add(Converters.From3Dto3I(start), trackEntity, particleCode);
        }

        public new void DrawParticles() {
            foreach (var vector in Clone()) {
                try {
                    foreach (var entity in vector.GetEntities()) {
                        if (entity.Logic == null) {
                            vector.Entities.Remove(entity);
                        } else {
                            if (((ParticleHostEntityLogic) entity.Logic).CanDispose) {
                                vector.Entities.Remove(entity);
                            }
                        }
                    }
                } catch {
                    vector.Entities.Clear();
                }

                if (!CoreHook.Universe.TryGetEntity(vector.TrackEntity.Id, out _)) {
                    foreach (var entity in vector.GetEntities()) {
                        entity.Dispose();
                    }
                    particles.Remove(vector);
                    continue;
                }

                var newEntities = GetRange(vector, vector.TrackEntity.Physics.Position);

                if (!vector.Entities.Any()) {
                    vector.Entities.AddRange(newEntities);
                } else {
                    var current = vector.GetEntities();
                    foreach (var entity in newEntities) {
                        if (!current.Any(x => {
                            var item = Converters.From3Dto3I(x.Physics.Position);
                            var item2 = Converters.From3Dto3I(entity.Physics.Position);
                            return item.X == item2.X && item.Y == item2.Y && item.Z == item2.Z;
                        })) {
                            vector.Entities.Add(entity);
                        }
                    }
                }


                foreach (var entity in vector.GetEntities().Where(x => !CoreHook.Universe.TryGetEntity(x.Id, out _))) {
                    CoreHook.Universe.AddEntity(entity);
                    ((ParticleHostEntityLogic)entity.Logic).SetTargetable();
                    entity.Bind(CoreHook.Universe);
                }
            }
        }
    }
}
