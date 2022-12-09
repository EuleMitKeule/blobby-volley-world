using System;
using BlobbyVolleyWorld.Entities.Physics;
using BlobbyVolleyWorld.Match;
using BlobbyVolleyWorld.UserInterface.Animation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BlobbyVolleyWorld.Game
{
    public abstract class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public PhysicsAsset PhysicsAsset { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public MatchSettings MatchSettings { get; private set; }
        
        public event EventHandler<MatchSettings> MatchSettingsChanged;

        protected virtual void Start()
        {
            MatchSettings = MatchSettings.Default;
            MatchSettingsChanged?.Invoke(this, MatchSettings);
        }

        public void UpdateMatchSettings(MatchSettings matchSettings)
        {
            MatchSettings = matchSettings;
            MatchSettingsChanged?.Invoke(this, MatchSettings);
        }
    }
}