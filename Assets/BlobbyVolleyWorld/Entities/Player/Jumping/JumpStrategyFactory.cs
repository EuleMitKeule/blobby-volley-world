using BlobbyVolleyWorld.Entities.Input;

namespace BlobbyVolleyWorld.Entities.Physics.Jumping
{
    public static class JumpStrategyFactory
    {
        public static IJumpStrategy Create(
            PlayerMovementComponent playerMovementComponent, 
            InputComponent inputComponent,
            JumpMode jumpMode)
        {
            return jumpMode switch
            {
                JumpMode.Standard => new StandardJumpStrategy(playerMovementComponent),
                JumpMode.NoJump => new NoJumpStrategy(),
                JumpMode.Pogo => new StandardJumpStrategy(playerMovementComponent),
                JumpMode.Spring => new SpringJumpStrategy(playerMovementComponent, inputComponent),
                _ => null
            };
        }
    }
}