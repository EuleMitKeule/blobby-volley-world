using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public class BallReadyStateComponent : BallStateComponent
    {
        public override bool IsMapCollisionEnabled => false;
        public override bool IsPlayerCollisionEnabled => true;
        public override bool IsGravityEnabled => false;
        
        public override void OnPlayerCollision()
        {
            SetState<BallRunningStateComponent>();
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