using System.Collections.Generic;
using Staxel;
using Staxel.Browser;
using Staxel.Client;
using Staxel.Input;
using Staxel.Items;
using Staxel.Mumble;
using Staxel.Particles;
using Staxel.Peripherals;
using Staxel.Player;
using Staxel.Rendering;
using Staxel.Sky;
using Staxel.Sound;
using Staxel.Steam;
using Staxel.Translation;
using Staxel.Weather;

namespace NimbusFox.FoxCore.Interpreters.Classes {
    internal class CContext {
        public List<ResolutionOption> SupportedResolutions => ClientContext.SupportedResolutions;

        public SteamManager SteamManager => ClientContext.SteamManager;
        public PlayerFacade PlayerFacade => ClientContext.PlayerFacade;
        public LastPlayedCharacterStorage LastPlayedCharacter => ClientContext.LastPlayedCharacter;
        public UserInput UserInput => ClientContext.UserInput;
        public OptionsStorage OptionsStorage => ClientContext.OptionsStorage;
        public BrowserCore BrowserCore => ClientContext.BrowserCore;
        public LocalServerManager LocalServerManager => ClientContext.LocalServerManager;
        public InputSource InputSource => ClientContext.InputSource;
        public ItemRendererManager ItemRendererManager => ClientContext.ItemRendererManager;
        public LanguageDatabase LanguageDatabase => ClientContext.LanguageDatabase;
        public WeatherRenderer WeatherRenderer => ClientContext.WeatherRenderer;
        public LagThresholdEstimator ServerLagEstimator => ClientContext.ServerLagEstimator;
        public SingleplayerWorldDatabase WorldDatabase => ClientContext.WorldDatabase;
        public TextToSpeechDatabase TextToSpeechDatabase => ClientContext.TextToSpeechDatabase;
        public PeripheralsManager PeripheralsManager => ClientContext.PeripheralsManager;
        public MumblePositionalAudioProvider MumblePositionalAudioProvider => ClientContext.MumblePositionalAudioProvider;
        public string[] NightTransitionTipCodes => ClientContext.NightTransitionTipCodes;
        public OverlayController OverlayController => ClientContext.OverlayController;
        public WebOverlayRenderer WebOverlayRenderer => ClientContext.WebOverlayRenderer;
        public ChunkRenderer ChunkRenderer => ClientContext.ChunkRenderer;
        public NameTagRenderer NameTagRenderer => ClientContext.NameTagRenderer;
        public EntityRenderer EntityRenderer => ClientContext.EntityRenderer;
        public OverlayRenderer OverlayRenderer => ClientContext.OverlayRenderer;
        public UniverseRenderer UniverseRenderer => ClientContext.UniverseRenderer;
        public GpuResources GpuResources => ClientContext.GpuResources;
        public SoundManager SoundManager => ClientContext.SoundManager;
        public ParticleManager ParticleManager => ClientContext.ParticleManager;
        public OptionProcessor OptionProcessor => ClientContext.OptionProcessor;
        public SkyBoxRenderer SkyBoxRenderer => ClientContext.SkyBoxRenderer;
        public int ReloadRevision => ClientContext.ReloadRevision;

        public void CleanupOldSession() {
            ClientContext.CleanupOldSession();
        }

        public void ClearCaches() {
            ClientContext.ClearCaches();
        }

        public void Deinitialize(bool reload = false) {
            ClientContext.Deinitialize(reload);
        }

        public void Initialize(bool skipResourceInitializations, bool disableSteam, bool reload = false) {
            ClientContext.Initialize(skipResourceInitializations, disableSteam, reload);
        }

        public void Reload() {
            ClientContext.Reload();
        }

        public void ResourceInitializations() {
            ClientContext.ResourceIntializations();
        }
    }
}
