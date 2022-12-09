using System;
using BlobbyVolleyWorld.Maps;
using BlobbyVolleyWorld.Match;
using BlobbyVolleyWorld.Settings;
using BlobbyVolleyWorld.UserInterface.Animation;
using Sirenix.OdinInspector;

namespace BlobbyVolleyWorld.Game
{
    public class ClientGameComponent : GameComponent
    {
        [ShowInInspector]
        [ReadOnly]
        public ClientSettings ClientSettings { get; private set; }
        
        public event EventHandler<ClientSettings> ClientSettingsChanged;
        
        MapComponent MapComponent { get; set; }
        MatchComponent MatchComponent { get; set; }
        BlackoutAnimationComponent BlackoutAnimationComponent { get; set; }

        void Awake()
        {
            MapComponent = FindObjectOfType<MapComponent>();
            MatchComponent = FindObjectOfType<MatchComponent>();
            BlackoutAnimationComponent = FindObjectOfType<BlackoutAnimationComponent>();
        }

        protected override void Start()
        {
            base.Start();
            
            ClientSettings = ClientSettings.Default;
            
            ClientSettingsChanged?.Invoke(this, ClientSettings);
        }

        public void StartLocalGame()
        {
            BlackoutAnimationComponent.BlackoutFinished += OnBlackoutFinished;
            BlackoutAnimationComponent.Blackout();
        }
        
        public void UpdateClientSettings(ClientSettings clientSettings)
        {
            ClientSettings = clientSettings;
            ClientSettingsChanged?.Invoke(this, ClientSettings);
        }

        void OnBlackoutFinished(object sender, EventArgs e)
        {
            BlackoutAnimationComponent.BlackoutFinished -= OnBlackoutFinished;
            MapComponent.SetMap(MatchSettings.Map);
            MatchComponent.InitializeMatch();
            BlackoutAnimationComponent.WhiteoutFinished += OnWhiteoutFinished;
            BlackoutAnimationComponent.Whiteout();
        }

        void OnWhiteoutFinished(object sender, EventArgs e)
        {
            BlackoutAnimationComponent.WhiteoutFinished -= OnWhiteoutFinished;
            MatchComponent.StartMatch();
        }
    }
}