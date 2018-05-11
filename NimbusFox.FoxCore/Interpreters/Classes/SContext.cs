using Staxel;
using System.Collections.Generic;
using Plukit.Base;
using Staxel.Activities;
using Staxel.Behavior;
using Staxel.Commands;
using Staxel.EntityStorage;
using Staxel.Festivals;
using Staxel.Rights;
using Staxel.Server;
using Staxel.Storage;
using Staxel.TileBlobStorage;

namespace NimbusFox.FoxCore.Interpreters.Classes {
    internal class SContext {
        public static ScriptDatabase ScriptDatabase => ServerContext.ScriptDatabase;
        public static StatisticsMonitor Statistics => ServerContext.Statistics;
        public static VillageDirector VillageDirector => ServerContext.VillageDirector;
        public static FestivalDatabase FestivalDatabase => ServerContext.FestivalDatabase;
        public static VillagerDefinitionDatabase VillagerDefinitionDatabase => ServerContext.VillagerDefinitionDatabase;
        public static VillagerLogicDatabase VillagerLogicDatabase => ServerContext.VillagerLogicDatabase;
        public static VillagerJobDatabase VillagerJobDatabase => ServerContext.VillagerJobDatabase;
        public static VillagerPersonalityDatabase VillagerPersonalityDatabase => ServerContext.VillagerPersonalityDatabase;
        public static Dictionary<string, IQuestDefinitionBuilder> QuestDefBuilders => ServerContext.QuestDefBuilders;
        public static int ReloadRevision => ServerContext.ReloadRevision;
        public static DirectorFacade DirectorFacade => ServerContext.DirectorFacade;
        public static EntityBlobDatabase EntityBlobDatabase => ServerContext.EntityBlobDatabase;
        public static TileBlobDatabase TileBlobDatabase => ServerContext.TileBlobDatabase;
        public static ChunkActivityDatabase ChunkActivityDatabase => ServerContext.ChunkActivityDatabase;
        public static CommandsManager CommandsManager => ServerContext.CommandsManager;
        public static RightsManager RightsManager => ServerContext.RightsManager;
        public static BlobStorage BlobStorage => ServerContext.BlobStorage;

        public void Deinitialize() {
            ServerContext.Deinitialize();
        }

        public void Initialize(bool startDatabase, bool revalidate, string storageName) {
            ServerContext.Initialize(startDatabase, revalidate, storageName);
        }
    }
}
