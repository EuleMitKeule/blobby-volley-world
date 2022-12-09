using System.Collections.Generic;
using BlobbyVolleyWorld.Entities;
using BlobbyVolleyWorld.Game;
using BlobbyVolleyWorld.Match.States;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Match
{
    [RequireComponent(typeof(MatchTimeComponent))]
    public class MatchComponent : StateContextComponent<MatchStateComponent>
    {
        [OdinSerialize]
        [Required]
        public GameObject PlayerPrefab { get; set; }
        
        [OdinSerialize]
        [Required]
        public GameObject BallPrefab { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        Dictionary<FieldPosition, PlayerComponent> Players { get; set; } = new();

        MatchTimeComponent MatchTimeComponent { get; set; }
        
        GameComponent GameComponent { get; set; }
        
        protected override void Awake()
        {
            base.Awake();

            GameComponent = FindObjectOfType<GameComponent>();
            MatchTimeComponent = GetComponent<MatchTimeComponent>();
            
            MatchTimeComponent.TimeChanged += OnTimeChanged;
        }

        public void InitializeMatch()
        {
            foreach (var fieldPosition in GameComponent.MatchSettings.PlayerMode.ToFieldPositions())
            {
                var playerPosition = GameComponent.PhysicsAsset.PlayerSpawnPositions[fieldPosition];
                var playerObject = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                var playerComponent = playerObject.GetComponent<PlayerComponent>();
                
                playerComponent.SetFieldPosition(fieldPosition);
                
                Players.Add(fieldPosition, playerComponent);
            }
            
            var ballObject = Instantiate(BallPrefab);
            var ballComponent = ballObject.GetComponent<BallMovementComponent>();

            Time.timeScale = GameComponent.MatchSettings.Speed;
        }

        public void StartMatch()
        {
            SetState<MatchReadyStateComponent>();
        }

        void OnTimeChanged(object sender, float time)
        {
            StateComponent.OnTimeChanged(sender, time);
        }
    }
}