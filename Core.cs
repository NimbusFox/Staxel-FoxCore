using System;
using NimbusFox.FoxCore.Managers;
using WorldManager = NimbusFox.FoxCore.Managers.WorldManager;

namespace NimbusFox.FoxCore {
    public class FxCore {
        public readonly ExceptionManager ExceptionManager;
        public readonly FileManager FileManager;
        public readonly WorldManager WorldManager;
        public readonly ParticleManager ParticleManager;
        public readonly EntityParticleManager EntityParticleManager;

        public FxCore(string author, string mod, string modVersion) {
            ExceptionManager = new ExceptionManager(author, mod, modVersion);
            FileManager = new FileManager(author, mod);
            WorldManager = new WorldManager();
            ParticleManager = new ParticleManager();
            EntityParticleManager = new EntityParticleManager();
        }
    }
}
