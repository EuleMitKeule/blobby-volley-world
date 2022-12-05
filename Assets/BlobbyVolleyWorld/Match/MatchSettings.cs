using System;
using BlobbyVolleyWorld.Maps;
using Sirenix.Serialization;

namespace BlobbyVolleyWorld.Match
{
    [Serializable]
    public class MatchSettings
    {
        [OdinSerialize] 
        public Map Map { get; private set; }
        
        [OdinSerialize]
        public GameMode GameMode { get; private set; }
        
        [OdinSerialize]
        public JumpMode JumpMode { get; private set; }
        
        [OdinSerialize]
        public PlayerMode PlayerMode { get; private set; }
        
        [OdinSerialize]
        public bool IsJumpOverNet { get; private set; }

        [OdinSerialize] 
        public float Speed { get; private set; }

        public MatchSettings(
            Map map, 
            GameMode gameMode, 
            JumpMode jumpMode, 
            PlayerMode playerMode, 
            bool isJumpOverNet, 
            float speed)
        {
            Map = map;
            GameMode = gameMode;
            JumpMode = jumpMode;
            PlayerMode = playerMode;
            IsJumpOverNet = isJumpOverNet;
            Speed = speed;
        }
        
        public MatchSettings WithMap(Map map) => 
            new (map, GameMode, JumpMode, PlayerMode, IsJumpOverNet, Speed);

        public MatchSettings WithGameMode(GameMode gameMode) => 
            new (Map, gameMode, JumpMode, PlayerMode, IsJumpOverNet, Speed);
        
        public MatchSettings WithJumpMode(JumpMode jumpMode) =>
            new (Map, GameMode, jumpMode, PlayerMode, IsJumpOverNet, Speed);
        
        public MatchSettings WithPlayerMode(PlayerMode playerMode) =>
            new (Map, GameMode, JumpMode, playerMode, IsJumpOverNet, Speed);
        
        public MatchSettings WithIsJumpOverNet(bool isJumpOverNet) =>
            new (Map, GameMode, JumpMode, PlayerMode, isJumpOverNet, Speed);
        
        public MatchSettings WithSpeed(float speed) =>
            new (Map, GameMode, JumpMode, PlayerMode, IsJumpOverNet, speed);

        public static MatchSettings Default => new(
            Maps.Map.Gym,
            GameMode.Standard,
            JumpMode.Standard,
            PlayerMode.Single,
            false,
            1f);
    }
}