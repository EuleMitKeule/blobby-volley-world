using BlobbyVolleyWorld.Entities.Physics;
using BlobbyVolleyWorld.Entities.Physics.Jumping;
using UnityEngine;

namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public class StandardJumpInputStrategy : IJumpInputStrategy
    {
        InputComponent InputComponent { get; }

        public StandardJumpInputStrategy(InputComponent inputComponent)
        {
            InputComponent = inputComponent;
        }

        public void OnJumpDown()
        {
            InputComponent.IsUpPressed = true;
        }

        public void OnJumpUp()
        {
            InputComponent.IsUpPressed = false;
        }
    }
}