using System;
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

        [ShowInInspector]
        [ReadOnly]
        public Side GivingSide { get; set; }
        
        public event EventHandler<Side> GivingSideChanged;
        
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
                var playerObject = Instantiate(PlayerPrefab);
                var playerComponent = playerObject.GetComponent<PlayerComponent>();
                
                playerComponent.SetFieldPosition(fieldPosition);
                
                Players.Add(fieldPosition, playerComponent);
            }
            
            var ballObject = Instantiate(BallPrefab);

            Time.timeScale = GameComponent.MatchSettings.Speed;
            
            GivingSide = Side.Left;
            GivingSideChanged?.Invoke(this, GivingSide);
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