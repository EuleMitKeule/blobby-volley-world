#nullable enable
using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public class BallStoppedStateComponent : BallStateComponent
    {
        public override bool IsMapCollisionEnabled => true;
        public override bool IsPlayerCollisionEnabled => false;
        public override bool IsGravityEnabled => true;
        
        public override void OnPlayerCollision()
        {
            
        }

        public override void OnGroundCollision()
        {
            
        }

        public override void OnWallCollision()
        {
            
        }

        public override void OnNetEdgeCollision()
        {
            
        }
    }
}