using BlobbyVolleyWorld.Maps;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public abstract class BallStateComponent : StateComponent
    {
        public abstract bool IsMapCollisionEnabled { get; }
        public abstract bool IsPlayerCollisionEnabled { get; }
        public abstract bool IsGravityEnabled { get; }
        
        protected BallMovementComponent BallMovementComponent { get; private set; }
        protected MapGeometryComponent MapGeometryComponent { get; private set; }
        
        void Awake()
        {
            BallMovementComponent = GetComponent<BallMovementComponent>();
            MapGeometryComponent = FindObjectOfType<MapGeometryComponent>();
        }

        public abstract void OnPlayerCollision();
        
        public abstract void OnGroundCollision();
        
        public abstract void OnWallCollision();
        
        public abstract void OnNetEdgeCollision();
    }
}