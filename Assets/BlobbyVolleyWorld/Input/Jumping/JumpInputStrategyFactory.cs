using System;

namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public static class JumpInputStrategyFactory
    {
        public static IJumpInputStrategy Create(InputComponent inputComponent, JumpMode jumpMode) =>
            jumpMode switch
            {
                JumpMode.Standard => new StandardJumpInputStrategy(inputComponent),
                JumpMode.NoJump => new NoJumpInputStrategy(inputComponent),
                JumpMode.Pogo => new PogoJumpInputStrategy(inputComponent),
                JumpMode.Spring => new SpringJumpInputStrategy(inputComponent),
                _ => throw new ArgumentOutOfRangeException(nameof(jumpMode), jumpMode, null)
            };
    }
}