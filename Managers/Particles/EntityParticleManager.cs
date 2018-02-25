using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Staxel.Builders;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers.Particles {
    public class EntityParticleManager : ParticleManager {
        public Guid Add(Vector3I start, Entity trackEntity, string particleCode) {
            var particleHost = new VectorRangeI {
                Start = start,
                ParticleCode = particleCode,
                TrackEntity = trackEntity
            };

            particles.Add(particleHost);
            return particleHost.UID;
        }

        public Guid Add(Vector3D start, Entity trackEntity, string particleCode) {
            return Add(Converters.From3Dto3I(start), trackEntity, particleCode);
        }

        public new void DrawParticles() {
            if (LastTick <= DateTime.Now.Ticks) {
                foreach (var vector in Clone()) {
                    foreach (var entity in vector.GetUnrenderedEntities()) {
                        CoreHook.Universe.AddEntity(entity.Key);
                        entity.Key.Bind(CoreHook.Universe);
                        vector.Entities[entity.Key] = true;
                    }

                    if (!CoreHook.Universe.TryGetEntity(vector.TrackEntity.Id, out _)) {
                        foreach (var entity in vector.GetEntities()) {
                            vector.Entities.Remove(entity);
                            entity.Dispose();
                        }
                        particles.Remove(vector);
                        continue;
                    }

                    if (vector.TrackEntityLastPos != Converters.From3Dto3I(vector.TrackEntity.Physics.Position)) {
                        foreach (var entity in vector.GetEntities()) {
                            ((ParticleHostEntityLogic)entity?.Logic)?.Finish();
                            vector.Entities.Remove(entity);
                        }
                    }

                    foreach (var entity in vector.GetEntities()) {
                        if (entity.Logic == null) {
                            vector.Entities.Remove(entity);
                        } else {
                            if (((ParticleHostEntityLogic)entity.Logic).CanDispose) {
                                vector.Entities.Remove(entity);
                            }
                        }
                    }

                    if (vector.TrackEntityLastPos != Converters.From3Dto3I(vector.TrackEntity.Physics.Position) || !vector.Entities.Any()) {
                        var pos = vector.TrackEntity.Physics.BottomPosition();
                        var newEntities = GetRange(vector, new Vector3D(pos.X + vector.Offset.X, pos.Y + vector.Offset.Y, pos.Z + vector.Offset.Z));

                        if (!vector.Entities.Any()) {
                            foreach (var entity in newEntities) {
                                vector.Entities.Add(entity, false);
                            }
                        } else {
                            var current = vector.GetEntities();
                            foreach (var entity in newEntities) {
                                if (!current.Any(x => {
                                    var item = Converters.From3Dto3I(x.Physics.Position);
                                    var item2 = Converters.From3Dto3I(entity.Physics.Position);
                                    return item.X == item2.X && item.Y == item2.Y && item.Z == item2.Z && !((ParticleHostEntityLogic)entity.Logic)._done;
                                })) {
                                    vector.Entities.Add(entity, false);
                                }
                            }
                        }

                        vector.TrackEntityLastPos = Converters.From3Dto3I(vector.TrackEntity.Physics.Position);
                    }
                }

                LastTick = DateTime.Now.Ticks + 100;
            }
        }
    }
}
