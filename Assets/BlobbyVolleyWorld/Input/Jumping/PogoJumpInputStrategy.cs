using BlobbyVolleyWorld.Entities.Physics;
using BlobbyVolleyWorld.Entities.Physics.Jumping;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public class PogoJumpInputStrategy : IJumpInputStrategy
    {
        InputComponent InputComponent { get; }

        public PogoJumpInputStrategy(InputComponent inputComponent)
        {
            InputComponent = inputComponent;
        }

        public void OnJumpDown()
        {
            InputComponent.IsUpPressed = true;
        }

        public void OnJumpUp() { }
    }
}