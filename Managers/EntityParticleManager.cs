using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.FoxCore.Managers {
    public class EntityParticleManager : ParticleManager {

        public override void DrawParticles() {
            if (!CoreHook.Particles.ContainsKey(Identifier)) {
                CoreHook.Particles.Add(Identifier, new List<ParticleStore>());
            }

            if (!CoreHook.Particles[Identifier].Any()) {
                var output = new List<ParticleStore>();
                var data = Clone();
                var uEntity = data.Select(x => x.Entity).Distinct();
                var playersLyst = new Lyst<Entity>();
                CoreHook.Universe.GetPlayers(playersLyst);
                var players = playersLyst.Where(x =>
                    uEntity.Any(y => y.PlayerEntityLogic.Uid() == x.PlayerEntityLogic.Uid()));

                foreach (var range in data.Where(x => players.Any(y => y.PlayerEntityLogic.Uid() == x.Entity.PlayerEntityLogic.Uid()))) {
                    var current = players.First(x => x.PlayerEntityLogic.Uid() == range.Entity.PlayerEntityLogic.Uid())
                        .Physics.Position;

                    current.Y -= 1;
                    output.AddRange(GetRange(range, current));
                }
                CoreHook.Particles[Identifier].AddRange(output);
            }
        }
    }
}
