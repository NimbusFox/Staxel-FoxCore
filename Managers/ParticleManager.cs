using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class ParticleManager : IDisposable {
        internal string Identifier { get; }

        internal List<VectorRangeI> particles = new List<VectorRangeI>();

        public ParticleManager() {
            Identifier = Guid.NewGuid().ToString();
        }

        public virtual void DrawParticles() {
            if (!CoreHook.Particles.ContainsKey(Identifier)) {
                CoreHook.Particles.Add(Identifier, new List<ParticleStore>());
            }

            if (!CoreHook.Particles[Identifier].Any()) {
                var output = new List<ParticleStore>();
                foreach (var range in Clone()) {
                    output.AddRange(GetRange(range, range.End));
                }
                CoreHook.Particles[Identifier].AddRange(output);
            }
        }

        public List<ParticleStore> GetRange(VectorRangeI range, Vector3I top) {
            var output = new List<ParticleStore>();
            for (var y = range.Start.Y - 1; y <= top.Y; y++) {
                for (var x = range.Start.X; x <= top.X; x++) {
                    for (var z = range.Start.Z; z <= top.Z; z++) {
                        var render = z == range.Start.Z || z == top.Z
                                                        || x == range.Start.X || x == top.X || x == top.X - 1;

                        if (render) {
                            output.Add(new ParticleStore(new Vector3D(x, y, z), range.Entity, range.ParticleCode));
                        }
                    }
                }
            }

            return output;
        }

        public List<ParticleStore> GetRange(VectorRangeI range, Vector3D top) {
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

        public void Add(Vector3I start, Vector3I end, string particleCode, Entity entity) {
            particles.Add(new VectorRangeI {
                Start = start,
                End = end,
                ParticleCode = particleCode,
                Entity = entity
            });
        }

        public void Add(Vector3D start, Vector3D end, string particleCode, Entity entity) {
            Add(Converters.From3Dto3I(start), Converters.From3Dto3I(end), particleCode, entity);
        }

        public void Remove(Vector3I start) {
            var remove = GetStartVectors(start);

            foreach (var item in remove) {
                particles.Remove(item);
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
