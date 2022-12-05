using System;
using BlobbyVolleyWorld.Match;
using BlobbyVolleyWorld.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlobbyVolleyWorld
{
    public class ClientGameComponent : GameComponent
    {
        [ShowInInspector]
        [ReadOnly]
        public MatchSettings MatchSettings { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public ClientSettings ClientSettings { get; private set; }
        
        public event EventHandler<MatchSettings> MatchSettingsChanged;
        public event EventHandler<ClientSettings> ClientSettingsChanged;
        
        void Start()
        {
            MatchSettings = MatchSettings.Default;
            ClientSettings = ClientSettings.Default;
            
            MatchSettingsChanged?.Invoke(this, MatchSettings);
            ClientSettingsChanged?.Invoke(this, ClientSettings);
        }
        
        public void UpdateClientSettings(ClientSettings clientSettings)
        {
            ClientSettings = clientSettings;
            ClientSettingsChanged?.Invoke(this, ClientSettings);
            
            Debug.Log($"Client settings updated: {ClientSettings}");
        }

        public void UpdateMatchSettings(MatchSettings matchSettings)
        {
            MatchSettings = matchSettings;
            MatchSettingsChanged?.Invoke(this, MatchSettings);
        }
    }
}