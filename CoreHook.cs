using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel.Effects;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.FoxCore {
    internal class CoreHook : IModHookV2 {
        internal static Universe Universe;
        internal static readonly Dictionary<string, List<ParticleStore>> Particles = new Dictionary<string, List<ParticleStore>>();
        
        public void Dispose() {
            Particles.Clear();
        }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            Universe = universe;

            var _particles = new Dictionary<string, List<ParticleStore>>(Particles);

            if (_particles.Any()) {
                    foreach (var item in _particles.Where(x => x.Value.Any())) {
                        var uniqueEntities = item.Value.Select(x => x.Entity).Distinct();

                        foreach (var entity in new List<Entity>(uniqueEntities)) {
                            var current = item.Value.First(x => x.Entity == entity);
                            BaseEffects.EmitParticles(current.Entity, current.Target, current.ParticleCode);
                            Particles[item.Key].Remove(current);
                        }
                    }
            }
        }
        public void UniverseUpdateAfter() { }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return true;
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }
        public void CleanupOldSession() { }
    }
}
