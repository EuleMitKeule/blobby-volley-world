using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public class BallRunningTennisStateComponent : BallStateComponent
    {
        public override bool IsMapCollisionEnabled => true;
        public override bool IsPlayerCollisionEnabled => true;
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