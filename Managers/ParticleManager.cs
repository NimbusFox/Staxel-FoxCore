using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Staxel.Builders;
using NimbusFox.FoxCore.Staxel.Builders.Logic;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class ParticleManager : IDisposable {
        internal string Identifier { get; }

        internal List<VectorRangeI> particles = new List<VectorRangeI>();

        public ParticleManager() {
            Identifier = Guid.NewGuid().ToString();
        }

        public void DrawParticles() {
            foreach (var vector in Clone()) {
                foreach (var entity in vector.GetEntities()) {
                    if (((ParticleHostEntityLogic) entity.Logic).CanDispose) {
                        vector.Entities.Remove(entity);
                    }
                }

                var newEntities = GetRange(vector, vector.End).Where(x =>
                    vector.GetEntities().Any(y => !vector.GetEntities().Any() || y.Physics.Position != x.Physics.Position));

                vector.Entities.AddRange(newEntities);

                foreach (var entity in vector.GetEntities().Where(x => !CoreHook.Universe.TryGetEntity(x.Id, out _))) {
                    CoreHook.Universe.AddEntity(entity);
                }
            }
        }

        public List<Entity> GetRange(VectorRangeI range, Vector3I top) {
            var output = new List<Entity>();
            var cube = new VectorCubeI(range.Start, top);
            for (var y = cube.Y.Start - 1; y <= cube.Y.End; y++) {
                for (var x = cube.X.Start; x <= cube.X.End; x++) {
                    for (var z = cube.Z.Start; z <= cube.Z.End; z++) {
                        var render = z == cube.Z.Start || z == cube.Z.End
                                                        || x == cube.X.Start || x == cube.X.Start - 1 || x == cube.X.End || x == cube.X.End - 1;

                        if (render) {
                            var entity = new Entity(CoreHook.Universe.AllocateNewEntityId(), false, ParticleHostEntityBuilder.KindCode, true);
                            entity.Physics.ForcedPosition(new Vector3D(x, y, z));
                            ((ParticleHostEntityLogic) entity.Logic).SetParticleCode(range.ParticleCode);
                            ((ParticleHostEntityLogic)entity.Logic).SetLocation(new Vector3D(x, y, z));
                            output.Add(entity);
                        }
                    }
                }
            }

            return output;
        }

        public List<Entity> GetRange(VectorRangeI range, Vector3D top) {
            return GetRange(range, Converters.From3Dto3I(top));
        }

        internal List<VectorRangeI> Clone() {
            return new List<VectorRangeI>(particles);
        }

        public bool ContainsWithinVector(Vector3I target) {
            var tempContainer = Clone();

            return tempContainer.Any(cube => new VectorCubeI(cube.Start, cube.End).IsInside(target));
        }

        public bool ContainsWithinVector(Vector3D target) {
            return ContainsWithinVector(Converters.From3Dto3I(target));
        }

        public bool ContainsStartVector(Vector3I target) {
            return Clone().Any(x => x.Start.Z == target.Z && x.Start.Y == target.Y && x.Start.X == target.X);
        }

        public bool ContainsStartVector(Vector3D target) {
            return ContainsStartVector(Converters.From3Dto3I(target));
        }

        public List<VectorRangeI> GetStartVectors(Vector3I target) {
            return Clone().Where(x => x.Start.Z == target.Z && x.Start.Y == target.Y && x.Start.X == target.X).ToList();
        }

        public List<VectorRangeI> GetStartVectors(Vector3D target) {
            return GetStartVectors(Converters.From3Dto3I(target));
        }

        public void Add(Vector3I start, Vector3I end, string particleCode) {
            particles.Add(new VectorRangeI {
                Start = start,
                End = end,
                ParticleCode = particleCode
            });
        }

        public void Add(Vector3D start, Vector3D end, string particleCode) {
            Add(Converters.From3Dto3I(start), Converters.From3Dto3I(end), particleCode);
        }

        public void Remove(Vector3I start) {
            var remove = GetStartVectors(start);

            if (remove.Any()) {
                particles.Remove(remove.First());
            }
        }

        public void Remove(Vector3D start) {
            Remove(Converters.From3Dto3I(start));
        }

        public void Dispose() {
            particles.Clear();
        }
    }
}
