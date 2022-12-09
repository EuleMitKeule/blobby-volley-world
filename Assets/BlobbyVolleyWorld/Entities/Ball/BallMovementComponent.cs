using BlobbyVolleyWorld.Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities
{
    public class BallMovementComponent : StateContextComponent<BallStateComponent>
    {
        [ShowInInspector]
        [ReadOnly]
        Vector2 Velocity { get; set; }
        
        GameComponent GameComponent { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            GameComponent = FindObjectOfType<GameComponent>();
        }
        
        void FixedUpdate()
        {
            Velocity += Vector2.up * (Time.fixedDeltaTime * GameComponent.PhysicsAsset.BallGravity);
            
            var clampedVelocity
        }
    }

    public abstract class BallStateComponent : StateComponent
    {
        
    }

    public class BallReadyStateComponent : BallStateComponent
    {
        
    }
}