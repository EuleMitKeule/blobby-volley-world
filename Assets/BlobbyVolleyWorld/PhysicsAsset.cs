using System.Collections.Generic;
using BlobbyVolleyWorld.Match;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Physics
{
    [CreateAssetMenu(menuName = nameof(PhysicsAsset), fileName = nameof(PhysicsAsset))]
    public class PhysicsAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float Gravity { get; private set; }
        
        [OdinSerialize]
        public float PlayerSpeed { get; private set; }
        
        [OdinSerialize]
        public float JumpSpeed { get; private set; }
        
        [OdinSerialize]
        public float JumpDrift { get; private set; }

        [OdinSerialize]
        public float JumpChargeTime { get; private set; } = 0.04f;
        
        [OdinSerialize]
        public float JumpChargeIncrease { get; private set; }
        
        [OdinSerialize]
        public float JumpChargeMin { get; private set; }
        
        [OdinSerialize]
        public float JumpChargeMax { get; private set; }

        [OdinSerialize]
        public float BallGravity { get; private set; } = 78f;

        [OdinSerialize]
        public float BallSpeed { get; private set; } = 45.4f;

        [OdinSerialize]
        public float BallMaxSpeed { get; private set; } = 52.2f;

        [OdinSerialize]
        public float BallGroundedSpeedThreshold { get; private set; } = 3.5f;

        [OdinSerialize]
        public float BallGroundBounceFactor { get; private set; } = 0.5f;

        [OdinSerialize]
        public float BallAngularVelocityFactor { get; private set; } 
        
        [OdinSerialize]
        public float BallGroundAngularVelocityFactor { get; private set; } = 0.5f;
        
        [OdinSerialize]
        public Dictionary<FieldPosition, Vector2> PlayerSpawnPositions { get; private set; }
        
        [OdinSerialize]
        public Dictionary<Side, Vector2> BallSpawnPositions { get; private set; } = new()
        {
            {Side.Left, new Vector2(-10f, 1f)},
            {Side.Right, new Vector2(10f, 1f)}
        };
    }
}