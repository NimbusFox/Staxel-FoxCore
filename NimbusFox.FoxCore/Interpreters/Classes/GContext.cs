using System.Collections.Generic;
using Plukit.Base;
using Simplex;
using Staxel;
using Staxel.Achievements;
using Staxel.Behavior.Conversation;
using Staxel.Behavior.Conversation.DialogueCommands;
using Staxel.Characters;
using Staxel.Core;
using Staxel.Crafting;
using Staxel.Docks;
using Staxel.Draw;
using Staxel.Effects;
using Staxel.EntityActions;
using Staxel.Expressions;
using Staxel.FarmAnimals;
using Staxel.FarmAnimals.Pets;
using Staxel.Farming;
using Staxel.Gatherables;
using Staxel.Gfx;
using Staxel.Items;
using Staxel.Journal;
using Staxel.Lighting;
using Staxel.Logic;
using Staxel.Mail;
using Staxel.Merchants;
using Staxel.Modding;
using Staxel.Notifications;
using Staxel.Particles;
using Staxel.Rendering;
using Staxel.Rendering.Palettes;
using Staxel.Sound;
using Staxel.Tiles;
using Staxel.TileStates;
using Staxel.TileStates.Totems;
using Staxel.Treasures;
using Staxel.ValueImage;
using Staxel.Weather;
using Staxel.WorldGen.Biomes;
using Staxel.WorldGen.Structures;

namespace NimbusFox.FoxCore.Interpreters.Classes {
    internal class GContext {
        public DeterministicRandom RandomSource {
            get => GameContext.RandomSource;
            set => GameContext.RandomSource = value;
        }

        public DebugGraphics DebugGraphics {
            get => GameContext.DebugGraphics;
            set => GameContext.DebugGraphics = value;
        }


        public TileConfigurationHolder[] TileMapping {
            get => GameContext.TileMapping;
            set => GameContext.TileMapping = value;
        }

        public bool[] SolidTileMapping {
            get => GameContext.SolidTileMapping;
            set => GameContext.SolidTileMapping = value;
        }

        public bool Revalidate {
            get => GameContext.Revalidate;
            set => GameContext.Revalidate = value;
        }

        public bool StoreBundle {
            get => GameContext.StoreBundle;
            set => GameContext.StoreBundle = value;
        }

        public int ReloadRevision {
            get => GameContext.ReloadRevision;
            set => GameContext.ReloadRevision = value;
        }

        public bool ResourcesInitialised {
            get => GameContext.ResourcesInitialised;
            set => GameContext.ResourcesInitialised = value;
        }

        public AssetBundleManager AssetBundleManager => GameContext.AssetBundleManager;

        public ContentLoader ContentLoader => GameContext.ContentLoader;

        public ResourceManager Resources => GameContext.Resources;

        public WorkerManager Worker => GameContext.Worker;

        public EntityFactory EntityFactory => GameContext.EntityFactory;

        public GraphicalContextFlag GraphicalContextFlag => GameContext.GraphicalContextFlag;

        public ColorCorrectionDatabase ColorCorrectionDatabase => GameContext.ColorCorrectionDatabase;

        public ParticleDatabase ParticleDatabase => GameContext.ParticleDatabase;

        public DockDatabase DockDatabase => GameContext.DockDatabase;

        public TileDatabase TileDatabase => GameContext.TileDatabase;

        public ItemDatabase ItemDatabase => GameContext.ItemDatabase;

        public CategoryDatabase CategoryDatabase => GameContext.CategoryDatabase;

        public AchievementDatabase AchievementDatabase => GameContext.AchievementDatabase;

        public AnimationDatabase AnimationDatabase => GameContext.AnimationDatabase;

        public SoundDatabase SoundDatabase => GameContext.SoundDatabase;

        public SystemStatus SystemStatus => GameContext.SystemStatus;

        public PlantDatabase PlantDatabase => GameContext.PlantDatabase;

        public ReactionDatabase ReactionDatabase => GameContext.ReactionDatabase;

        public RecipeDatabase RecipeDatabase => GameContext.RecipeDatabase;

        public FarmingDatabase FarmingDatabase => GameContext.FarmingDatabase;

        public GatheringDatabase GatheringDatabase => GameContext.GatheringDatabase;

        public EmoteDatabase EmoteDatabase => GameContext.EmoteDatabase;

        public LightingWorker LightingWorker => GameContext.LightingWorker;

        public ExpressionDatabase ExpressionDatabase => GameContext.ExpressionDatabase;

        public EntityActionDatabase EntityActionDatabase => GameContext.EntityActionDatabase;

        public EffectDatabase EffectDatabase => GameContext.EffectDatabase;

        public PaletteDatabase PaletteDatabase => GameContext.PaletteDatabase;

        public CharacterDesignDatabase CharacterDesignDatabase => GameContext.CharacterDesignDatabase;

        public TileStateDatabase TileStateDatabase => GameContext.TileStateDatabase;

        public TreasureDatabase TreasureDatabase => GameContext.TreasureDatabase;

        public ModdingController ModdingController => GameContext.ModdingController;

        public FarmAnimalBehaviourDatabase FarmAnimalBehaviourDatabase => GameContext.FarmAnimalBehaviourDatabase;

        public FarmAnimalDatabase FarmAnimalDatabase => GameContext.FarmAnimalDatabase;

        public MerchantDatabase MerchantDatabase => GameContext.MerchantDatabase;

        public WeatherDatabase WeatherDatabase => GameContext.WeatherDatabase;

        public NotificationDatabase NotificationDatabase => GameContext.NotificationDatabase;

        public PetDatabase PetDatabase => GameContext.PetDatabase;

        public TotemDatabase TotemDatabase => GameContext.TotemDatabase;

        public DialogueCommandsManager DialogueCommandsManager => GameContext.DialogueCommandsManager;

        public DialogueDatabase DialogueDatabase => GameContext.DialogueDatabase;

        public ValueImageDatabase ValueImageDatabase => GameContext.ValueImageDatabase;

        public MailDatabase MailDatabase => GameContext.MailDatabase;

        public JournalQuestDatabase JournalQuestDatabase => GameContext.JournalQuestDatabase;

        public Dictionary<string, IComponentBuilder> GeneralComponentBuilders => GameContext.GeneralComponentBuilders;

        public Dictionary<string, IItemComponentBuilder> ItemComponentBuilders => GameContext.ItemComponentBuilders;

        public Dictionary<string, ITileComponentBuilder> TileComponentBuilders => GameContext.TileComponentBuilders;

        public Dictionary<string, IWeatherComponentBuilder> WeatherComponentBuilders =>
            GameContext.WeatherComponentBuilders;

        public void ContentPath(string storageName, out string fullContentRootPath, out string fullStoragePath) {
            GameContext.ContentPath(storageName, out fullContentRootPath, out fullStoragePath);
        }

        public void Initialize(GameContextInitFlags flags, string storageName) {
            GameContext.Initialize(flags, storageName);
        }

        public void ResourceInitializations(bool revalidate, bool client, bool storeBundle) {
            GameContext.ResourceInitializations(revalidate, client, storeBundle);
        }

        public void Deinitialize(bool reload = false) {
            GameContext.Deinitialize(reload);
        }
    }
}
