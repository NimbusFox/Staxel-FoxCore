using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Client.Staxel.Builders;
using NimbusFox.FoxCore.Client.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers.Particles {
    public class ParticleManager : IDisposable {
        internal long LastTick;

        internal List<VectorRangeI> particles = new List<VectorRangeI>();

        public void DrawParticles() {
            if (LastTick <= DateTime.Now.Ticks) {
                foreach (var vector in Clone()) {
                    foreach (var entity in vector.GetUnrenderedEntities()) {
                        CoreHook.Universe.AddEntity(entity.Key);
                        entity.Key.Bind(CoreHook.Universe);
                        vector.Entities[entity.Key] = true;
                    }
                }

                LastTick = DateTime.Now.Ticks + 100;
            }
        }

        public List<Entity> GetRange(VectorRangeI range, Vector3I top) {
            var output = new List<Entity>();
            var cube = new VectorCubeI(range.Start, top);
            Fox_Core.VectorLoop(range.Start, top, (x, y, z) => {
                var renderCount = 0;

                renderCount += y == cube.Y.Start || y == cube.Y.End ? 1 : 0;

                renderCount += z == cube.Z.Start || z == cube.Z.End ? 1 : 0;

                renderCount += x == cube.X.Start || x == cube.X.Start - 1 || x == cube.X.End ? 1 : 0;

                if (renderCount > 1) {
                    var entity = new Entity(CoreHook.Universe.AllocateNewEntityId(), false, ParticleHostEntityBuilder.KindCode, true);
                    entity.Physics.ForcedPosition(new Vector3D(x, y, z));
                    ((ParticleHostEntityLogic)entity.Logic).SetParticleCode(range.ParticleCode);
                    ((ParticleHostEntityLogic)entity.Logic).SetLocation(new Vector3D(x, y, z));
                    output.Add(entity);
                }

            });

            return output;
        }

        public List<Entity> GetRange(VectorRangeI range, Vector3D top) {
            return GetRange(range, top.From3Dto3I());
        }

        internal List<VectorRangeI> Clone() {
            return new List<VectorRangeI>(particles);
        }

        public bool ContainsWithinVector(Vector3I target) {
            var tempContainer = Clone();

            return tempContainer.Any(cube => new VectorCubeI(cube.Start, cube.End).IsInside(target));
        }

        public bool ContainsWithinVector(Vector3D target) {
            return ContainsWithinVector(target.From3Dto3I());
        }

        public bool ContainsStartVector(Vector3I target) {
            return Clone().Any(x => x.Start.Z == target.Z && x.Start.Y == target.Y && x.Start.X == target.X);
        }

        public bool ContainsStartVector(Vector3D target) {
            return ContainsStartVector(target.From3Dto3I());
        }

        public List<VectorRangeI> GetStartVectors(Vector3I target) {
            return Clone().Where(x => x.Start.Z == target.Z && x.Start.Y == target.Y && x.Start.X == target.X).ToList();
        }

        public List<VectorRangeI> GetStartVectors(Vector3D target) {
            return GetStartVectors(target.From3Dto3I());
        }

        /// <summary>
        /// Bug: Do not make the cuboid taller than 20 else unexpected renderings can occur
        /// </summary>
        public Guid Add(Vector3I start, Vector3I end, string particleCode) {
            var range = new VectorRangeI {
                Start = start,
                End = end,
                ParticleCode = particleCode
            };

            var entities = GetRange(range, end);

            foreach (var entity in entities) {
                range.Entities.Add(entity, false);
            }

            particles.Add(range);

            return range.UID;
        }

        /// <summary>
        /// Bug: Do not make the cuboid taller than 20 else unexpected renderings can occur
        /// </summary>
        public Guid Add(Vector3D start, Vector3D end, string particleCode) {
            return Add(start.From3Dto3I(), end.From3Dto3I(), particleCode);
        }

        public void Remove(Guid UID) {
            var target = Clone().FirstOrDefault(x => x.UID == UID);
            if (target != null) {
                foreach (var entity in target.Entities) {
                    ((ParticleHostEntityLogic) entity.Key.Logic).Finish();
                }
                particles.Remove(target);

            }
        }

        public void Dispose() {
            foreach (var item in Clone()) {
                Remove(item.UID);
            }

            foreach (var item in Clone()) {
                foreach (var entity in item.Entities) {
                    if (!((ParticleHostEntityLogic) entity.Key.Logic).IsDisposed) {
                        ((ParticleHostEntityLogic)entity.Key.Logic).Dispose();
                    }
                }
            }
            particles.Clear();
        }
    }
}
